using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : MonoBehaviour
{
    public enum UnitOfSpeed
    {
        Mps = 1,
        Mph = 2,
        Kph = 3
    }

    public UnitOfSpeed unitOfSpeed = UnitOfSpeed.Mps;

    public Image filledSpeedbar;
    public Text speedText;

    [SerializeField]
    private AnimationCurve curve;

    public void SetSpeed(float speed)
    {
        // The maximum velocity most players could get to is 30 in a normal map
        // we can look at adding a color change or jiggle to the UI if they break 30
        float ratio = speed / PlayerConstants.MaxReasonableVelocity;
        filledSpeedbar.fillAmount = curve.Evaluate(ratio);

        switch (unitOfSpeed)
        {
            case UnitOfSpeed.Mps:
                speedText.text = System.Math.Round(speed, 2) + "m/s";
                break;
            case UnitOfSpeed.Mph:
                speedText.text = System.Math.Round(speed * 2.237f, 2) + "m/h";
                break;
            case UnitOfSpeed.Kph:
                speedText.text = System.Math.Round(speed * 3.6, 2) + "km/h";
                break;
        }
    }
}
