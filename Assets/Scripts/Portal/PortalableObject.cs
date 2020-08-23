﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalableObject : MonoBehaviour
{
    protected GameObject cloneObject;

    protected int inPortalCount = 0;
    
    protected Portal inPortal;
    protected Portal outPortal;

    private new Rigidbody rigidbody;
    protected new Collider collider;

    protected static readonly Quaternion halfTurn = Quaternion.Euler(0.0f, 180.0f, 0.0f);
    protected static Vector3 cloneSpawnPosition = new Vector3(-1000.0f, 1000.0f, -1000.0f); // Somewhere far off the map until needed

    protected virtual void Awake()
    {
        cloneObject = new GameObject();
        cloneObject.SetActive(false);
        var meshFilter = cloneObject.AddComponent<MeshFilter>();
        var meshRenderer = cloneObject.AddComponent<MeshRenderer>();

        meshFilter.mesh = GetComponent<MeshFilter>().mesh;
        meshRenderer.materials = GetComponent<MeshRenderer>().materials;
        cloneObject.transform.localScale = transform.localScale;

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    protected virtual void LateUpdate()
    {
        if(inPortal == null || outPortal == null)
        {
            return;
        }

        if(cloneObject.activeSelf && inPortal.IsPlaced() && outPortal.IsPlaced())
        {
            var inTransform = inPortal.transform;
            var outTransform = outPortal.transform;

            // Update position of clone.
            Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
            relativePos = halfTurn * relativePos;
            cloneObject.transform.position = outTransform.TransformPoint(relativePos);

            // Update rotation of clone.
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
            relativeRot = halfTurn * relativeRot;
            cloneObject.transform.rotation = outTransform.rotation * relativeRot;
        }
        else
        {
            cloneObject.transform.position = cloneSpawnPosition;
        }
    }

    public virtual void SetIsInPortal(Portal inPortal, Portal outPortal, List<Collider> wallColliders)
    {
        this.inPortal = inPortal;
        this.outPortal = outPortal;

        foreach(Collider wallCollider in wallColliders)
        {
            Physics.IgnoreCollision(collider, wallCollider);
        }

        cloneObject.SetActive(true);

        ++inPortalCount;
    }

    public virtual void Warp()
    {
        var inTransform = inPortal.transform;
        var outTransform = outPortal.transform;

        // Update position of object.
        Vector3 relativePos = inTransform.InverseTransformPoint(transform.position);
        relativePos = halfTurn * relativePos;
        transform.position = outTransform.TransformPoint(relativePos);

        // Update rotation of object.
        Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * transform.rotation;
        relativeRot = halfTurn * relativeRot;
        transform.rotation = outTransform.rotation * relativeRot;

        // Update velocity of rigidbody.
        Vector3 relativeVel = inTransform.InverseTransformDirection(rigidbody.velocity);
        relativeVel = halfTurn * relativeVel;
        rigidbody.velocity = outTransform.TransformDirection(relativeVel);

        // Swap portal references.
        var tmp = inPortal;
        inPortal = outPortal;
        outPortal = tmp;
    }

    public virtual void ExitPortal(List<Collider> wallColliders)
    {
        foreach (Collider wallCollider in wallColliders)
        {
            Physics.IgnoreCollision(collider, wallCollider, false);

        }

        if (--inPortalCount == 0)
        {
            cloneObject.SetActive(false);
        }
    }
}
