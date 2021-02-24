using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerPortalableController : MonoBehaviour
{
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

    public void SetIsInPortal(Portal inPortal, Portal outPortal, List<Collider> wallColliders)
    {
        this.inPortal = inPortal;
        this.outPortal = outPortal;

        foreach (Collider wallCollider in wallColliders)
        {
            Physics.IgnoreCollision(playerCollider, wallCollider);
        }

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

        PlayerSoundEffects.PlaySoundEffect(SoundEffectType.PortalPassThrough);
    }

    public virtual void ExitPortal(List<Collider> wallColliders)
    {
        foreach (Collider wallCollider in wallColliders)
        {
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