using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalPair : MonoBehaviour
{
    public Portal BluePortal { private set; get; }
    public Portal PinkPortal { private set; get; }

    public GameObject bluePortalPrefab;
    public GameObject pinkPortalPrefab;

    private void Awake()
    {
        GameObject tempBluePortal = Instantiate(bluePortalPrefab, transform);
        BluePortal = tempBluePortal.GetComponent<Portal>();

        GameObject tempPinkPortal = Instantiate(pinkPortalPrefab, transform);
        PinkPortal = tempPinkPortal.GetComponent<Portal>();

        BluePortal.Init(PortalType.Blue, PinkPortal);
        PinkPortal.Init(PortalType.Pink, BluePortal);
    }

    public void ResetPortals()
    {
        BluePortal.ResetPortal();
        PinkPortal.ResetPortal();
    }

    public void SetRenderTextures(RenderTexture blueTexture, RenderTexture pinkTexture)
    {
        BluePortal.SetTexture(blueTexture);
        PinkPortal.SetTexture(pinkTexture);
    }
}