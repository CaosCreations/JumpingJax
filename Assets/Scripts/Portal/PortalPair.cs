using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPair : MonoBehaviour
{
    public Portal[] Portals { private set; get; }

    private void Awake()
    {
        Portals = gameObject.GetComponentsInChildren<Portal>(true);

        if (Portals.Length != 2)
        {
            Debug.LogError("PortalPair children must contain exactly two Portal components in total.");
        }
    }

    public void ResetPortals()
    {
        if(Portals == null)
        {
            return;
        }

        if (Portals[0] == null)
        {
            return;
        }

        Portals[0].ResetPortal();
        Portals[1].ResetPortal();
    }
}