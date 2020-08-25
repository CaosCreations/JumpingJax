using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : MonoBehaviour
{
    public RectTransform mask;
    public Text speedText;

    [SerializeField]
    private AnimationCurve curve;

    public void SetSpeed(float speed)
    {
        float ratio = speed / PlayerConstants.MaxVelocity;
        mask.anchorMax = new Vector2(curve.Evaluate(ratio), 1);

        speedText.text = Mathf.Round(speed * 100) / 100 + "m/s";
    }
}
