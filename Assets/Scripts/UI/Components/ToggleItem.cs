using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleItem : MonoBehaviour
{
    public Text toggleNameText;
    public Toggle toggle;
    public TooltipItem tooltip;

    public void Init(string text, bool isToggled, string tooltipText)
    {
        toggleNameText.text = text;
        toggle.isOn = isToggled;
        tooltip.SetTooltipText(tooltipText);
    }
}
