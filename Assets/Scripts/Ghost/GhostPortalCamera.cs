using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPortalCamera : MonoBehaviour
{
    [Header("Set in Editor")]
    public Camera portalCamera = null;

    [Header("Set at RUNTIME")]
    public GhostPortalPair portalPair;

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
        GhostPortalPlacement portalPlacement = GetComponentInParent<GhostPortalPlacement>();
        if (portalPlacement != null)
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

    public void UpdatePortalRecursion(int recursionLevel)
    {
        portalRecursions = recursionLevel;
    }

    private void OnPreRender()
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
                RenderCamera(portalPair.BluePortal, portalPair.PinkPortal, i);
            }
        }

        if (portalPair.PinkPortal.IsRendererVisible())
        {
            portalCamera.targetTexture = pinkTempRenderTexture;
            for (int i = portalRecursions - 1; i >= 0; --i)
            {
                RenderCamera(portalPair.PinkPortal, portalPair.BluePortal, i);
            }
        }
    }

    private void RenderCamera(GhostPortal inPortal, GhostPortal outPortal, int recursionId)
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
        portalCamera.Render();
    }
}
