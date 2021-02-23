using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPortal : MonoBehaviour
{
    [Header("Set in Editor")]
    public Material defaultPortalMaterial;
    private PortalType portalType;
    [SerializeField]
    private LayerMask placementMask;
    [SerializeField]
    private LayerMask overhangMask;
    public Renderer outlineRenderer = null;

    [Header("Set at RUNTIME")]

    [SerializeField]
    private GhostPortal otherPortal = null;

    private bool isPlaced = false;

    public Material renderTextureMaterial;
    private Renderer myRenderer;
    private Crosshair playerCrosshair;

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

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
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
    }

    public void Init(PortalType portalType, GhostPortal otherPortal)
    {
        this.portalType = portalType;
        this.otherPortal = otherPortal;
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

    public void SetTexture(RenderTexture tex)
    {
        renderTextureMaterial.mainTexture = tex;
    }

    public bool IsRendererVisible()
    {
        return myRenderer.isVisible;
    }

    public GhostPortal GetOtherPortal()
    {
        return otherPortal;
    }

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
            float interpolationFactor = (float)i / (float)steps;
            Vector3 stepPosition = Vector3.Lerp(testPoint, Vector3.zero, interpolationFactor);
            Vector3 worldSpaceStepPosition = transform.TransformPoint(stepPosition);

            if (Physics.CheckSphere(worldSpaceStepPosition, bigSphereCastSize, overhangMask))
            {
                return stepPosition - testPoint;
            }
        }

        return overhangOffset;
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

    // If the new portal overlaps an already placed one
    // try to fix the overlap, or delete it
    private void FixPortalOverlaps()
    {
        Vector3 worldSpaceCenter = transform.TransformPoint(boxCollider.center);
        Collider[] overlappingBoxes = Physics.OverlapBox(worldSpaceCenter, boxCollider.bounds.extents);

        foreach (Collider otherCollider in overlappingBoxes)
        {
            if (otherCollider.gameObject == otherPortal.gameObject)
            {
                GetClosestOverlapFix();
            }
        }

        if (GetIsOverhanging())
        {
            ResetPortal();
            ResetOtherPortal();
        }
        else
        {
            playerCrosshair.CrossCheck(portalType == PortalType.Blue);
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.PortalOpen);
        }
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
        else
        {
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
            if (offsetX < offsetY)
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

    public void ResetPortal()
    {
        gameObject.SetActive(false);
        isPlaced = false;
        boxCollider.enabled = false;
        ResetPortalMaterial();
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
}
