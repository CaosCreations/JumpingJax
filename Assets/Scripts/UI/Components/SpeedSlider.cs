using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : MonoBehaviour
{
    public RectTransform mask;
    public Text speedText;
    private float basePosition = -525;
    
    public void SetSpeed(float speed)
    {
        float ratio = speed / PlayerConstants.MaxVelocity;
        float xOffset = basePosition * ratio;
        mask.anchorMax = new Vector2(ratio, 1);


        speedText.text = Mathf.Round(speed * 100) / 100 + "m/s";
    }
}
