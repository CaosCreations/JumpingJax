using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RayCastUtils
{
   public static Trace TracePlayerBBox(BoxCollider collider, Vector3 end, int layersToIgnore)
    {
        float distance = (end - collider.transform.position).magnitude;
        Vector3 direction = (end - collider.transform.position).normalized;

        RaycastHit[] hits = Physics.BoxCastAll(
            center: collider.bounds.center,
            halfExtents: collider.bounds.extents,
            direction: direction,
            Quaternion.identity,
            maxDistance: distance,
            layerMask: layersToIgnore,
            QueryTriggerInteraction.Ignore);

        Trace trace = new Trace();
        trace.start = collider.transform.position;
        trace.destination = end;


        if (hits.Length > 0)
        {
            List<RaycastHit> orderedHits = hits.OrderBy(x => x.distance).ToList();
            RaycastHit closestHit = orderedHits.First();
            Vector3 hitOffset = collider.bounds.extents;
            hitOffset.y = 0;
            closestHit.point -= hitOffset; //boxcast is farther away than the max distance. also the y value isn't the same as the center
            trace.SetHitData(hits[0], collider, layersToIgnore);
        }
        else
        {
            RaycastHit hit = new RaycastHit();
            hit.point = end;
            trace.SetHitData(hit, collider, layersToIgnore);
        }

        return trace;
    }

    public static Trace TraceBBoxFrom(BoxCollider collider, Vector3 start, Vector3 end, int layersToIgnore)
    {
        float distance = (end - start).magnitude;
        Vector3 direction = (end - start).normalized;

        RaycastHit[] hits = Physics.BoxCastAll(
            center: start,
            halfExtents: collider.bounds.extents,
            direction: direction,
            Quaternion.identity,
            maxDistance: distance,
            layerMask: layersToIgnore,
            QueryTriggerInteraction.Ignore);

        Trace trace = new Trace();
        trace.start = start;
        trace.destination = end;

        if (hits.Length > 0)
        {
            trace.SetHitData(hits[0], collider, layersToIgnore);
        }
        else
        {
            RaycastHit hit = new RaycastHit();
            hit.point = end;
            trace.SetHitData(hit, collider, layersToIgnore);
        }

        return trace;
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
    public bool allSolid;

    public void SetHitData(RaycastHit hit, BoxCollider myCollider, int layersToIgnore)
    {
        this.hit = hit;
        hitPoint = hit.point;
        Vector3 endDifference = destination - start;
        Vector3 hitDifference = hitPoint - start;
        fraction = hitDifference.magnitude / endDifference.magnitude;
        distanceTraveled = hitDifference.magnitude;

        // TODO: make this more accurate. if the ray is at an angle it could have a distance longer than extents.x, but less that extends.magnitude
        var overlaps = Physics.OverlapBox(myCollider.bounds.center, myCollider.bounds.extents, Quaternion.identity, layersToIgnore, QueryTriggerInteraction.Ignore);
        if(overlaps.Length > 0)
        {
            allSolid = true;
        }
        else
        {
            allSolid = false;
        }
    }
}
