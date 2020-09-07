using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostPad : MonoBehaviour
{
    public Vector3 velocityChange;
    public bool shouldDrawGizmo;

    private void OnDrawGizmos()
    {
        if (shouldDrawGizmo)
        {
            Debug.DrawRay(transform.position, velocityChange, Color.magenta, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        if(player != null)
        {
            player.newVelocity.y = 0;
            player.newVelocity += velocityChange;
        }
    }
}
