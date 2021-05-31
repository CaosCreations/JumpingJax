﻿using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Portal : MonoBehaviour
{
    [Header("Set in Editor")]
    public Material defaultPortalMaterial;
    [SerializeField]
    private LayerMask placementMask;
    [SerializeField]
    private LayerMask overhangMask;
    public Renderer outlineRenderer = null;


    [Header("Set at RUNTIME")]
    public AudioSource idleAudioSource;
    public Material renderTextureMaterial;

    [SerializeField]
    private Portal otherPortal = null;

    private Renderer myRenderer;
    private Crosshair playerCrosshair;
    
    private PortalType portalType;
    private bool isPlaced = false;

    [SerializeField]
    public List<Collider> wallsPortalIsTouching;

    private PlayerPortalableController objectToWarp;

    // Used for portal positioning
    private BoxCollider boxCollider;
    private float sphereCastSize = 0.02f;
    private float bigSphereCastSize = 0.04f;
    // TODO: calculate these based off of collider extents
    private List<Vector3> overHangTestPoints = new List<Vector3>
    {
        new Vector3(-1.51f,  0, 0),
        new Vector3( 1.51f,  0, 0),
        new Vector3( 0, -1.51f, 0),
        new Vector3( 0,  1.51f, 0)
    };

    // Used for the portal alpha cutoff, gives it the blinking effect
    private const float minAlphaCutOff = 0.04f;
    private const float maxAlphaCutOff = 0.3f;
    private float cutOffValue = 0.02f;
    private const float cutOffInterval = 0.001f;
    private bool isIncrementing = true;
    private Vector3 portalBoundsExtents; //We need to set this while the collider is active, as it is Vector3.zero when inactive

    private void Awake()
    {
        idleAudioSource = GetComponent<AudioSource>();
        idleAudioSource.Play();
        boxCollider = GetComponent<BoxCollider>();
        portalBoundsExtents = boxCollider.bounds.extents;
        myRenderer = GetComponent<Renderer>();
        renderTextureMaterial = myRenderer.material;
        playerCrosshair = FindObjectOfType<Crosshair>();
        ResetPortal();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        UpdatePortalOutline();
        if(objectToWarp != null)
        {
            WarpObjects();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerPortalableController playerPortalable = other.GetComponent<PlayerPortalableController>();
        if(playerPortalable != null)
        {
            if (otherPortal.IsPlaced())
            {
                objectToWarp = playerPortalable;
                objectToWarp.SetIsInPortal(this, otherPortal, wallsPortalIsTouching);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerPortalableController portalable = other.GetComponent<PlayerPortalableController>();
        if (portalable != null)
        {
            ResetObjectInPortal(portalable);
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.up * 4, Color.green, 0f);
        Debug.DrawRay(transform.position, transform.right * 4, Color.red, 0f);
        Debug.DrawRay(transform.position, transform.forward * 4, Color.blue, 0f);
    }

    public void Init(PortalType portalType, Portal otherPortal)
    {
        this.portalType = portalType;
        this.otherPortal = otherPortal;
    }

    private void WarpObjects()
    {
        Vector3 objectPosition = transform.InverseTransformPoint(objectToWarp.transform.position);

        if (objectPosition.z > 0.0f)
        {
            objectToWarp.Warp();
            ResetObjectInPortal(objectToWarp);
        }
    }
    

    #region PortalPlacement
    public void PlacePortal(Vector3 pos, Quaternion rot)
    {
        isPlaced = true;
        if (!otherPortal.isPlaced)
        {
            boxCollider.enabled = false;
            ResetPortalMaterial();
        }
        else
        {
            myRenderer.material = renderTextureMaterial;
            otherPortal.myRenderer.material = otherPortal.renderTextureMaterial;

            boxCollider.enabled = true;
            otherPortal.boxCollider.enabled = true;
        }

        transform.position = pos;
        transform.rotation = rot;
        transform.position -= transform.forward * 0.001f;
        gameObject.SetActive(true);

        FixOverhangs();
        FixPortalOverlaps();
        GetWallColliders();
    }

    // Ensure the portal cannot extend past the edge of a surface, or intersect a corner
    private void FixOverhangs()
    {
        for (int i = 0; i < 4; ++i)
        {
            Vector3 overhangTestPosition = transform.TransformPoint(overHangTestPoints[i]);

            // If the point isn't touching anything, it overhangs
            if (!Physics.CheckSphere(overhangTestPosition, sphereCastSize, overhangMask))
            {
                Vector3 portalOverhangOffset = FindOverhangOffset(overHangTestPoints[i]);
                transform.Translate(portalOverhangOffset, Space.Self);
            }
        }
    }

    // This method finds the closest point where the object is no longer overhanging
    private Vector3 FindOverhangOffset(Vector3 testPoint)
    {
        Vector3 overhangOffset = -testPoint;

        int steps = Mathf.FloorToInt(testPoint.magnitude / sphereCastSize);

        for (int i = 0; i < steps; i++)
        {
            float interpolationFactor = (float) i / (float) steps;
            Vector3 stepPosition = Vector3.Lerp(testPoint, Vector3.zero, interpolationFactor);
            Vector3 worldSpaceStepPosition = transform.TransformPoint(stepPosition);

            if (Physics.CheckSphere(worldSpaceStepPosition, bigSphereCastSize, overhangMask))
            {
                return stepPosition - testPoint;
            }
        }

        return overhangOffset;
    }

    // If the new portal overlaps an already placed one
    // try to fix the overlap, or delete it
    private void FixPortalOverlaps()
    {
        Vector3 worldSpaceCenter = transform.TransformPoint(boxCollider.center);
        Collider[] overlappingBoxes = Physics.OverlapBox(worldSpaceCenter, boxCollider.bounds.extents);

        foreach(Collider otherCollider in overlappingBoxes)
        {
            if (otherCollider.gameObject == otherPortal.gameObject)
            {
                GetClosestOverlapFix();
            }
        }

        if (GetIsOverhanging())
        {
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.PortalRejected);
            ResetPortal();
            ResetOtherPortal();
        }
        else
        {
            playerCrosshair.CrossCheck(portalType == PortalType.Blue);
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.PortalOpen);
            idleAudioSource.Play();
        }
    }

    private bool GetIsOverhanging()
    {
        for (int i = 0; i < 4; ++i)
        {
            Vector3 overhangTestPosition = transform.TransformPoint(overHangTestPoints[i]);

            // If the point isn't touching anything, it overhangs
            if (!Physics.CheckSphere(overhangTestPosition, bigSphereCastSize, overhangMask))
            {
                return true;
            }
        }

        return false;
    }

    private void GetClosestOverlapFix()
    {
        Vector3 overlapFix = Vector3.zero;

        Vector3 localOffset = transform.InverseTransformPoint(otherPortal.transform.position);

        bool isInsideX = localOffset.x < PlayerConstants.portalWidth;
        bool isInsideY = localOffset.y < PlayerConstants.portalHeight;


        float offsetX = PlayerConstants.portalWidth - Mathf.Abs(localOffset.x);
        float offsetY = PlayerConstants.portalHeight - Mathf.Abs(localOffset.y);

        Vector3 offsetFixX = Vector3.zero;
        Vector3 offsetFixY = Vector3.zero;

        if (localOffset.x < 0)
        {
            offsetFixX = new Vector3(offsetX, 0, 0);
        }
        else {
            offsetFixX = new Vector3(-offsetX, 0, 0);
        }

        if (localOffset.y < 0)
        {
            offsetFixY = new Vector3(0, offsetY, 0);
        }
        else
        {
            offsetFixY = new Vector3(0, -offsetY, 0);
        }

        if (isInsideX && isInsideY)
        {
            if(offsetX < offsetY)
            {
                overlapFix = offsetFixX;
            }
            else
            {
                overlapFix = offsetFixY;

            }
        }
        else if (isInsideX)
        {
            overlapFix = offsetFixX;
        }
        else if (isInsideY)
        {
            overlapFix = offsetFixY;
        }

        transform.Translate(overlapFix, Space.Self);
    }

    private void GetWallColliders()
    {
        if(objectToWarp != null)
        {
            objectToWarp.ExitPortal(wallsPortalIsTouching);
        }

        wallsPortalIsTouching = new List<Collider>();

        // Need to manually calcualte this. We can't use boxCollider.bounds.center, as it doesn't update when the boxCollider isn't enabled
        Vector3 worldSpaceCenter = transform.TransformPoint(boxCollider.center);

        Collider[] overlappingBoxes = Physics.OverlapBox(worldSpaceCenter, portalBoundsExtents, transform.rotation, overhangMask);

        wallsPortalIsTouching = new List<Collider>(overlappingBoxes);
    }
    #endregion

    public void ResetPortal()
    {
        isPlaced = false;
        boxCollider.enabled = false;
        gameObject.SetActive(false);
        ResetPortalMaterial();
        ResetObjectInPortal(objectToWarp);
        idleAudioSource.Stop();
    }

    public void ResetObjectInPortal(PlayerPortalableController portalable)
    {
        if(portalable == null)
        {
            return;
        }

        if (objectToWarp == portalable)
        {
            objectToWarp = null;
            portalable.ExitPortal(wallsPortalIsTouching);
        }
    }

    private void UpdatePortalOutline()
    {
        if (cutOffValue >= maxAlphaCutOff)
        {
            isIncrementing = false;
        }

        if (cutOffValue <= minAlphaCutOff)
        {
            isIncrementing = true;
        }


        cutOffValue += isIncrementing ? cutOffInterval : -cutOffInterval;
        outlineRenderer.material.SetFloat("_Cutoff", cutOffValue);
        outlineRenderer.gameObject.transform.Rotate(Vector3.forward, 0.1f);
    }

    public void ResetOtherPortal()
    {
        otherPortal.ResetPortalMaterial();
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }

    public void ResetPortalMaterial()
    {
        myRenderer.material = defaultPortalMaterial;
    }

    public void SetTexture(RenderTexture tex)
    {
        renderTextureMaterial.mainTexture = tex;
    }

    public bool IsRendererVisible()
    {
        return myRenderer.isVisible;
    }

    public Portal GetOtherPortal()
    {
        return otherPortal;
    }
}