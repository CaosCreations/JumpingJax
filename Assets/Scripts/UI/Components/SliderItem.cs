using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SliderItem : MonoBehaviour
{
    public Text label;
    public Slider slider;
    public InputField input;
    public TooltipItem tooltip;

    public void Init(string labelText, float value, UnityAction<float> setSensitivity, float minValue, float maxValue, bool isInt, string tooltipText)
    {
        label.text = labelText;

        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(setSensitivity);
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.wholeNumbers = isInt;
        slider.value = value;

        input.text = value.ToString();

        tooltip.SetTooltipText(tooltipText);
    }

    public void SetLabel(string text)
    {
        label.text = text;
    }

    public void SetSliderValue(float value)
    {
        slider.value = value;
    }

    public void SetInput(string text)
    {
        input.text = text;
    }
}
