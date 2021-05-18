using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class RecursivePortalCamera : MonoBehaviour
{
    [Header("Set in Editor")]
    public Camera portalCamera = null;

    [Header("Set at RUNTIME")]
    public PortalPair portalPair;

    public RenderTexture blueTempRenderTexture;
    public RenderTexture pinkTempRenderTexture;

    public Camera myCamera;

    private int portalRecursions;
    private bool isPortalLevel = false;

    private const int renderTextureDepth = 24;
    private Quaternion flippedYRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

    private void Awake()
    {
        portalRecursions = OptionsPreferencesManager.GetPortalRecursion();
        myCamera = GetComponent<Camera>();

        blueTempRenderTexture = new RenderTexture(Screen.width, Screen.height, renderTextureDepth, RenderTextureFormat.ARGB32);
        pinkTempRenderTexture = new RenderTexture(Screen.width, Screen.height, renderTextureDepth, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        // These HAVE to be in Start(), because the portalPair is instantiated in PortalPlacement:Awake()
        PortalPlacement portalPlacement = GetComponentInParent<PortalPlacement>();
        if(portalPlacement != null && portalPlacement.portalPair != null)
        {
            portalPair = portalPlacement.portalPair;
            portalPair.SetRenderTextures(blueTempRenderTexture, pinkTempRenderTexture);
            isPortalLevel = true;
        }
        else
        {
            isPortalLevel = false;
        }
    }

    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }

    public void UpdatePortalRecursion(int recursionLevel)
    {
        portalRecursions = recursionLevel;
    }

    void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        if (Time.timeScale == 0 || !isPortalLevel)
        {
            return;
        }

        if (!portalPair.BluePortal.IsPlaced() || !portalPair.PinkPortal.IsPlaced())
        {
            return;
        }

        if (portalPair.BluePortal.IsRendererVisible())
        {
            portalCamera.targetTexture = blueTempRenderTexture;
            for (int i = portalRecursions - 1; i >= 0; --i)
            {
                RenderCamera(portalPair.BluePortal, portalPair.PinkPortal, i, SRC);
            }
        }

        if (portalPair.PinkPortal.IsRendererVisible())
        {
            portalCamera.targetTexture = pinkTempRenderTexture;
            for (int i = portalRecursions - 1; i >= 0; --i)
            {
                RenderCamera(portalPair.PinkPortal, portalPair.BluePortal, i, SRC);
            }
        }
    }

    private void RenderCamera(Portal inPortal, Portal outPortal, int recursionId, ScriptableRenderContext SRC)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 0; i <= recursionId; ++i)
        {
            // Position the camera behind the other portal.
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = flippedYRotation * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // Rotate the camera to look through the other portal.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = flippedYRotation * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        // Set the camera's oblique view frustum.
        Plane p = new Plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlane = new Vector4(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.Inverse(portalCamera.worldToCameraMatrix)) * clipPlane;

        var newMatrix = myCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
    }
}