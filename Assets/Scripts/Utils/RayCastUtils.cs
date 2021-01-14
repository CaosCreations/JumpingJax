using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastUtils
{
   public static Trace TracePlayerBBox(BoxCollider collider, Vector3 end, int layersToIgnore)
    {
        RaycastHit[] hits = Physics.BoxCastAll(
            center: collider.bounds.center,
            halfExtents: collider.bounds.extents,
            direction: end.normalized,
            Quaternion.identity,
            maxDistance: end.magnitude,
            layerMask: layersToIgnore);

        Trace trace = new Trace();
        trace.start = collider.bounds.center;
        trace.destination = end;

        if (hits.Length > 0)
        {
            trace.SetHitData(hits[0], collider);
        }
        else
        {
            RaycastHit hit = new RaycastHit();
            hit.point = end;
            trace.SetHitData(hit, collider);
        }

        return trace;
    }
    
}

public class SourceEngineRay
{
    Vector3 delta;
    Vector3 extents;
    Vector3 startOffset;
    Vector3 startCenter;

    public void Init(Vector3 start, Vector3 end, Vector3 minExtents, Vector3 maxExtents)
    {
        delta = end - start;
        extents = maxExtents - minExtents;
        startOffset = minExtents - maxExtents;
        startOffset *= 0.5f;

        // Set the startCenter to the center of the box
        startCenter = start - startOffset;
        startOffset *= -1;
    }
}

public class Trace
{
    public Vector3 start;
    public Vector3 destination;

    public RaycastHit hit;
    public Vector3 hitPoint;
    public float fraction;
    public float distanceTraveled;
    public bool didLeaveBoundingBox;

    public void SetHitData(RaycastHit hit, BoxCollider myCollider)
    {
        this.hit = hit;
        hitPoint = hit.point;
        Vector3 endDifference = destination - start;
        Vector3 hitDifference = hitPoint - start;
        fraction = hitDifference.magnitude / endDifference.magnitude;
        distanceTraveled = hitDifference.magnitude;

        // TODO: make this more accurate. if the ray is at an angle it could have a distance longer than extents.x, but less that extends.magnitude
        if (distanceTraveled <= myCollider.bounds.extents.magnitude)
        {
            didLeaveBoundingBox = false;
        }
    }
}
