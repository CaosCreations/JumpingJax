﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursivePortalCamera : MonoBehaviour
{
    [SerializeField]
    private Portal[] portals = new Portal[2];

    [SerializeField]
    private Camera portalCamera = null;

    private RenderTexture tempTexture1;
    private RenderTexture tempTexture2;

    private Camera mainCamera;

    private int portalRecursions = 2;
    private const int renderTextureDepth = 24;

    private Quaternion flippedYRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private bool isPortalLevel = false;

    private void Awake()
    {
        portalRecursions = OptionsPreferencesManager.GetPortalRecursion();
        mainCamera = Camera.main;

        tempTexture1 = new RenderTexture(Screen.width, Screen.height, renderTextureDepth, RenderTextureFormat.ARGB32);
        tempTexture2 = new RenderTexture(Screen.width, Screen.height, renderTextureDepth, RenderTextureFormat.ARGB32);
    }

    public void UpdatePortalRecursion(int recursionLevel)
    {
        portalRecursions = recursionLevel;
    }

    private void Start()
    {
        isPortalLevel = GameManager.GetCurrentLevel().isPortalLevel;
        if (Time.timeScale == 0 || !isPortalLevel)
        {
            return;
        }
        portals[0].SetTexture(tempTexture1);
        portals[1].SetTexture(tempTexture2);
    }

    private void OnPreRender()
    {
        if (Time.timeScale == 0 || !isPortalLevel)
        {
            return;
        }

        if (!portals[0].IsPlaced() || !portals[1].IsPlaced())
        {
            return;
        }

        if (portals[0].IsRendererVisible())
        {
            portalCamera.targetTexture = tempTexture1;
            for (int i = portalRecursions - 1; i >= 0; --i)
            {
                RenderCamera(portals[0], portals[1], i);
            }
        }

        if (portals[1].IsRendererVisible())
        {
            portalCamera.targetTexture = tempTexture2;
            for (int i = portalRecursions - 1; i >= 0; --i)
            {
                RenderCamera(portals[1], portals[0], i);
            }
        }
    }

    private void RenderCamera(Portal inPortal, Portal outPortal, int recursionId)
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

        var newMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.projectionMatrix = newMatrix;

        // Render the camera to its render target.
        portalCamera.Render();
    }
}