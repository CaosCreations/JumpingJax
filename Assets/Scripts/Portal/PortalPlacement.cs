﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalType
{
    Blue, Pink
}

[RequireComponent(typeof(CameraMove))]
public class PortalPlacement : MonoBehaviour
{
    private Crosshair crosshair;
    public bool showDebugGizmos = false;
    public LayerMask layerMask;

    public GameObject portalPairPrefab;
    private PortalPair portalPair = null;


    private CameraMove cameraMove;
    private PlayerPortalableController playerPortalable;

    private Quaternion flippedYRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private const float portalRaycastDistance = 250;
    
    private void Awake()
    {
        crosshair = GetComponent<Crosshair>();
        cameraMove = GetComponent<CameraMove>();
        playerPortalable = GetComponent<PlayerPortalableController>();
        portalPair = FindObjectOfType<PortalPair>();
        if(portalPair == null)
        {
            if (IsPortalLevel())
            {
                portalPair = Instantiate(portalPairPrefab).GetComponent<PortalPair>();
            }
        }
    }

    private bool IsPortalLevel()
    {
        var allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject gameObject in allObjects)
        {
            if(gameObject.layer == PlayerConstants.PortalMaterialLayer)
            {
                return true;
            }
        }

        return false;
    }

    private void Update()
    {
        if (Time.timeScale == 0 || playerPortalable.IsInPortal())
        {
            return;
        }

        if (InputManager.GetKeyDown(PlayerConstants.Portal1))
        {
            FirePortal(PortalType.Blue, cameraMove.playerCamera.transform.position, cameraMove.playerCamera.transform.forward, portalRaycastDistance);
        }
        else if (InputManager.GetKeyDown(PlayerConstants.Portal2))
        {
            FirePortal(PortalType.Pink, cameraMove.playerCamera.transform.position, cameraMove.playerCamera.transform.forward, portalRaycastDistance);
        }
    }

    private void FirePortal(PortalType portalType, Vector3 pos, Vector3 dir, float distance)
    {
        Physics.Raycast(pos, dir, out RaycastHit hit, distance, layerMask, QueryTriggerInteraction.Collide);

        if (showDebugGizmos)
        {
            Debug.DrawRay(pos, dir * distance, Color.red, 5);
            Debug.Log($"FirePortal() type: {portalType} pos: {pos} dir: {dir} distance: {distance}");
        }

        if (hit.collider != null)
        {
            // If we hit a portal, spawn a portal through this portal
            if (hit.collider.gameObject.layer == PlayerConstants.PortalLayer)
            {
                var inPortal = hit.collider.GetComponent<Portal>();

                if (inPortal == null || !inPortal.IsPlaced())
                {
                    return;
                }

                var outPortal = inPortal.GetOtherPortal();

                // Update position of raycast
                pos = outPortal.transform.position;

                // Update direction of raycast.
                Vector3 relativeDir = inPortal.transform.InverseTransformDirection(dir);
                relativeDir = flippedYRotation * relativeDir;
                dir = outPortal.transform.TransformDirection(relativeDir);

                // Subtract from the distance so the ray doesn't go on forever
                distance -= Vector3.Distance(pos, hit.point);

                FirePortal(portalType, pos, dir, distance);

                return;
            }
            else if (hit.collider.gameObject.layer == PlayerConstants.PortalMaterialLayer)
            {
                var cameraRotation = cameraMove.TargetRotation;
                var portalRight = cameraRotation * Vector3.right;

                if(Mathf.Abs(portalRight.x) >= 0)
                {
                    portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
                }
                else
                {
                    portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;
                }

                var portalForward = -hit.normal;

                var portalUp = -Vector3.Cross(portalRight, portalForward);

                if (portalForward.x != 0 || portalForward.z != 0)
                {
                    portalUp = Vector3.up;
                }

                var portalRotation = Quaternion.LookRotation(portalForward, portalUp);

                if(portalType == PortalType.Blue)
                {
                    portalPair.BluePortal.PlacePortal(hit.point, portalRotation);
                }
                else
                {
                    portalPair.PinkPortal.PlacePortal(hit.point, portalRotation);
                }
            }
        }
    }
}