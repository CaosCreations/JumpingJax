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
            center:                     collider.bounds.center,
            halfExtents:                collider.bounds.extents,
            direction:                  direction,
            orientation:                Quaternion.identity,
            maxDistance:                distance,
            layerMask:                  layersToIgnore,
            queryTriggerInteraction:    QueryTriggerInteraction.Ignore);

        Trace trace = new Trace();
        trace.start = collider.transform.position;
        trace.destination = end;


        if (hits.Length > 0)
        {
            //boxcast is farther away than the max distance. also the y value isn't the same as the center
            List<RaycastHit> orderedHits = hits.OrderBy(x => x.distance).ToList();
            RaycastHit closestHit = orderedHits.First();
            Vector3 hitOffset = collider.bounds.extents;
            closestHit.point += collider.bounds.extents;


            trace.SetHitData(closestHit, collider, layersToIgnore);
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
            center:                     start,
            halfExtents:                collider.bounds.extents,
            direction:                  direction,
            orientation:                Quaternion.identity,
            maxDistance:                distance,
            layerMask:                  layersToIgnore,
            queryTriggerInteraction:    QueryTriggerInteraction.Ignore);

        Trace trace = new Trace();
        trace.start = start;
        trace.destination = end;

        if (hits.Length > 0)
        {
            List<RaycastHit> orderedHits = hits.OrderBy(x => x.distance).ToList();
            RaycastHit closestHit = orderedHits.First();
            Vector3 newHitPoint = closestHit.point;

            Vector3 hitDiff = start - closestHit.point;
            Vector3 projected = Vector3.Project(hitDiff, direction);
            Vector3 verticalProjected = Vector3.Project(hitDiff, Vector3.up);

            closestHit.point += projected + verticalProjected;

            trace.SetHitData(closestHit, collider, layersToIgnore);
        }
        else
        {
            RaycastHit hit = new RaycastHit();
            hit.point = end;
            trace.SetHitData(hit, collider, layersToIgnore);
        }

        return trace;
    }

    public static Trace StayOnGroundTrace(BoxCollider collider, Vector3 start, Vector3 end, int layersToIgnore)
    {
        float distance = (end - start).magnitude;
        Vector3 direction = (end - start).normalized;

        RaycastHit[] hits = Physics.BoxCastAll(
            center:                     start,
            halfExtents:                collider.bounds.extents,
            direction:                  direction,
            orientation:                Quaternion.identity,
            maxDistance:                distance,
            layerMask:                  layersToIgnore,
            queryTriggerInteraction:    QueryTriggerInteraction.Ignore);

        Trace trace = new Trace();
        trace.start = start;
        trace.destination = end;

        if (hits.Length > 0)
        {
            List<RaycastHit> orderedHits = hits.OrderBy(x => x.distance).ToList();
            RaycastHit closestHit = orderedHits.First();
            Vector3 newHitPoint = closestHit.point;

            // The hit point is point distance from the center of the collider
            // so we need to add back in the height to find the actual height we can move
            newHitPoint.y += collider.bounds.extents.y;
            // The RaycastHit that's returned isn't guaranteed to have the same x/z as the player
            // Correct this so that our distance is accurate
            newHitPoint.x = collider.transform.position.x;
            newHitPoint.z = collider.transform.position.z;

            closestHit.point = newHitPoint;


            trace.SetHitData(closestHit, collider, layersToIgnore);
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

    public void SetHitData(RaycastHit hit, BoxCollider myCollider, int layersToIgnore)
    {
        this.hit = hit;
        hitPoint = hit.point;
        Vector3 endDifference = destination - start;
        Vector3 hitDifference = hitPoint - start;
        fraction = hitDifference.magnitude / endDifference.magnitude;
        distanceTraveled = hitDifference.magnitude;
    }
}
