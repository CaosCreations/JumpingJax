using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerPortalableController : MonoBehaviour
{
    public LayerMask portalLayerMask;

    public bool isInPortal = false;

    private GameObject cloneObject;

    private Portal inPortal;
    private Portal outPortal;

    private static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    private static Vector3 cloneSpawnPosition = new Vector3(-1000.0f, 1000.0f, -1000.0f); // Somewhere far off the map until needed

    private CameraMove cameraMove;
    private PlayerMovement playerMovement;
    private Collider playerCollider;

    private const float playerMinPortalSpeed = 2;
    private const float playerPortalSpeedMultiplier = 2;

    protected void Awake()
    {
        CreateClone();
        playerMovement = GetComponent<PlayerMovement>();
        cameraMove = GetComponent<CameraMove>();
    }

    private void Update()
    {
        // The player collides with the wall before that trigger executes the next frame
        // This is a temporary fix that will manually check for that collision
        CheckPlayerWillWarp();
    }

    protected void LateUpdate()
    {
        if (inPortal == null || outPortal == null)
        {
            return;
        }

        if (cloneObject.activeSelf && inPortal.IsPlaced() && outPortal.IsPlaced())
        {
            var inTransform = inPortal.transform;
            var outTransform = outPortal.transform;

            // Update position of clone.
            Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
            relativePos = halfTurn * relativePos;
            cloneObject.transform.position = outTransform.TransformPoint(relativePos);

            // Update rotation of clone.
            Quaternion playerYRotation = cameraMove.playerCamera.transform.rotation;
            playerYRotation.eulerAngles = new Vector3(0, playerYRotation.eulerAngles.y, 0);
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * playerYRotation;
            relativeRot = halfTurn * relativeRot;
            cloneObject.transform.rotation = outTransform.rotation * relativeRot;
        }
        else
        {
            cloneObject.transform.position = cloneSpawnPosition;
        }
    }

    private void CheckPlayerWillWarp()
    {
        if (isInPortal)
        {
            return;
        }

        Vector3 startPosition = gameObject.transform.position;
        Vector3 endPosition = startPosition + (playerMovement.currentVelocity * Time.deltaTime);
        Vector3 direction = playerMovement.currentVelocity.normalized;
        float maxDistance = (endPosition - startPosition).magnitude;


        RaycastHit hit;
        // If we will hit a portal next frame ignore collision with the wall behind it
        if (Physics.CapsuleCast(
            startPosition,
            endPosition,
            PlayerConstants.PlayerColliderRadius,
            direction,
            out hit,
            maxDistance + 0.1f,
            portalLayerMask,
            QueryTriggerInteraction.Collide))
        {
            Portal portalHit = hit.collider.gameObject.GetComponent<Portal>();
            if (portalHit != null)
            {
                SetIsInPortal(portalHit, portalHit.GetOtherPortal());
            }
        }
    }

    public void SetIsInPortal(Portal inPortal, Portal outPortals)
    {
        this.inPortal = inPortal;
        this.outPortal = outPortals;

        foreach (Collider wallCollider in inPortal.wallsPortalIsTouching)
        {
            Debug.Log($"now ignoring collision with: {wallCollider.name}");
            Physics.IgnoreCollision(playerCollider, wallCollider);
        }

        //foreach (Collider wallCollider in outPortal.wallsPortalIsTouching)
        //{
        //    Debug.Log($"now ignoring collision with: {wallCollider.name}");
        //    Physics.IgnoreCollision(playerCollider, wallCollider);
        //}

        cloneObject.SetActive(true);

        isInPortal = true;
    }

    private void CreateClone()
    {
        cloneObject = new GameObject();
        cloneObject.SetActive(false);
        var meshFilter = cloneObject.AddComponent<MeshFilter>();
        var meshRenderer = cloneObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = GetComponentInChildren<MeshFilter>().mesh;
        meshRenderer.materials = GetComponentInChildren<MeshRenderer>().materials;
        cloneObject.transform.localScale = transform.localScale;
        cloneObject.layer = PlayerConstants.CloneLayer;

        playerCollider = GetComponent<Collider>();
    }

    public void Warp()
    {
        var inTransform = inPortal.transform;
        var outTransform = outPortal.transform;

        // Update position of object.
        Vector3 localPosition = inTransform.InverseTransformPoint(transform.position);
        localPosition = halfTurn * localPosition;
        playerMovement.controller.enabled = false;
        transform.position = outTransform.TransformPoint(localPosition);
        playerMovement.controller.enabled = true;

        // Update rotation of object.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraMove.TargetRotation;
        relativeRot = halfTurn * relativeRot;
        cameraMove.SetTargetRotation(outTransform.rotation * relativeRot);

        // Update velocity of rigidbody.
        Vector3 relativeVel = inTransform.InverseTransformDirection(playerMovement.velocityToApply);
        relativeVel = halfTurn * relativeVel;
        playerMovement.velocityToApply = outTransform.TransformDirection(relativeVel);

        if (playerMovement.velocityToApply.magnitude < playerMinPortalSpeed)
        {
            playerMovement.velocityToApply *= playerPortalSpeedMultiplier;
        }

        // Swap portal references.
        var tmp = inPortal;
        inPortal = outPortal;
        outPortal = tmp;

        ExitPortal(inPortal);

        PlayerSoundEffects.PlaySoundEffect(SoundEffectType.PortalPassThrough);
    }

    public virtual void ExitPortal(Portal portal)
    {
        Debug.Log($"exit portal: {portal.name}, colliders: {portal.wallsPortalIsTouching.Count}");

        foreach (Collider wallCollider in portal.wallsPortalIsTouching)
        {
            Debug.Log($"now colliding with: {wallCollider.name}");
            Physics.IgnoreCollision(playerCollider, wallCollider, false);
        }

        isInPortal = false;

        if (!isInPortal)
        {
            cloneObject.SetActive(false);
        }
    }

    public bool IsInPortal()
    {
        if (isInPortal)
        {
            return isInPortal;
        }
        else
        {
            Collider[] overlappingColliders = Physics.OverlapBox(playerMovement.controller.bounds.center, playerMovement.controller.bounds.extents, Quaternion.identity);
            return overlappingColliders
                .ToList()
                .Where(col => col.tag == PlayerConstants.PortalTag)
                .Count() > 0;
        }
    }
}