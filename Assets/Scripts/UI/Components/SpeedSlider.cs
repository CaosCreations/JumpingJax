using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UnitOfSpeed
{
    Mps = 0,
    Kph = 1,
    Mph = 2
}

public class SpeedSlider : MonoBehaviour
{
    public static UnitOfSpeed unitOfSpeed; 

    public Image filledSpeedbar;
    public TMP_Text speedText;

    [SerializeField]
    private AnimationCurve curve;

    private void Start()
    {
        SetUnitOfSpeed(OptionsPreferencesManager.GetUnitOfSpeed());
    }

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
            case UnitOfSpeed.Kph:
                speedText.text = System.Math.Round(speed * 3.6, 2) + "km/h";
                break;
            case UnitOfSpeed.Mph:
                speedText.text = System.Math.Round(speed * 2.237f, 2) + "mi/h";
                break;
        }
    }

    public static void SetUnitOfSpeed(int index)
    {
        // Set the current unit of speed value based on the numeric value associated with it 
        unitOfSpeed = (UnitOfSpeed)(System.Enum.GetValues(unitOfSpeed.GetType())).GetValue(index);

        OptionsPreferencesManager.SetUnitOfSpeed(index);
    }
}
