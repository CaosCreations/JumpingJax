using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Crosshair : MonoBehaviour
{
    public Image crosshair;
    public Sprite crosshairDefault;
    public Sprite crosshairLeft;
    public Sprite crosshairRight;
    public Sprite crosshairLeftTop;
    public Sprite crosshairRightTop;
    private bool isLeftPlaced;
    private bool isRightPlaced;

    public PortalPair portalPair;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        crosshair.sprite = crosshairDefault;
        isLeftPlaced = false;
        isRightPlaced = false;
    }

    public void CrossCheck(bool isLeft)
    {
        if (isLeft)
        {
            isLeftPlaced = true;
            if (isRightPlaced)
            {
                crosshair.sprite = crosshairLeftTop;
            }
            else
            {
                crosshair.sprite = crosshairLeft;
            }
        }
        else
        {
            isRightPlaced = true;
            if (isLeftPlaced)
            {
                crosshair.sprite = crosshairRightTop;
            }
            else
            {
                crosshair.sprite = crosshairRight;
            }
        }
    }
}
