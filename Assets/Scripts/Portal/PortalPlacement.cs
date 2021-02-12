using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PortalType
{
    Blue, Pink
}

[RequireComponent(typeof(CameraMove))]
public class PortalPlacement : MonoBehaviour
{
    [Header("Set in Editor")]
    public bool showDebugGizmos = true;
    public LayerMask layerMask;
    public GameObject portalPairPrefab;

    [Header("Set at RUNTIME")]
    public PortalPair portalPair = null;
    public Camera ghostCamera;

    private CameraMove cameraMove;
    private PlayerPortalableController playerPortalable;
    private Quaternion flippedYRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private bool isPortalLevel;
    
    private void Awake()
    {
        cameraMove = GetComponent<CameraMove>();
        playerPortalable = GetComponent<PlayerPortalableController>();
        isPortalLevel = IsPortalLevel();
        if(isPortalLevel)
        {
            portalPair = Instantiate(portalPairPrefab).GetComponent<PortalPair>();
        }
    }

    private bool IsPortalLevel()
    {
        GameObject tempPortalWall = GameObject.FindWithTag(PlayerConstants.PortalWallTag);
        if(tempPortalWall == null)
        {
            return false;
        }
        return true;
    }

    private void Update()
    {
        if (Time.timeScale == 0 || playerPortalable.IsInPortal() || ghostCamera.enabled || !isPortalLevel)
        {
            return;
        }

        if (InputManager.GetKeyDown(PlayerConstants.Portal1))
        {
            FirePortal(PortalType.Blue, cameraMove.playerCamera.transform.position, cameraMove.playerCamera.transform.forward, PlayerConstants.PortalRaycastDistance, cameraMove.transform);
        }
        else if (InputManager.GetKeyDown(PlayerConstants.Portal2))
        {
            FirePortal(PortalType.Pink, cameraMove.playerCamera.transform.position, cameraMove.playerCamera.transform.forward, PlayerConstants.PortalRaycastDistance, cameraMove.transform);
        }
        else if (InputManager.GetKeyDown(PlayerConstants.ResetPortals))
        {
            portalPair.ResetPortals();
        }
    }
    
    public void FirePortal(PortalType portalType, Vector3 pos, Vector3 dir, float distance, Transform camera)
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

                FirePortal(portalType, pos, dir, distance, camera);

                return;
            }
            else if (hit.collider.gameObject.layer == PlayerConstants.PortalMaterialLayer)
            {
                var cameraRotation = camera.rotation;
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