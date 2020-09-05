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
        // The maximum velocity most players could get to is 30 in a normal map
        // we can look at adding a color change or jiggle to the UI if they break 30
        float ratio = speed / PlayerConstants.MaxReasonableVelocity;
        mask.anchorMax = new Vector2(curve.Evaluate(ratio), 1);

        speedText.text = Mathf.Round(speed * 100) / 100 + "m/s";
    }
}
