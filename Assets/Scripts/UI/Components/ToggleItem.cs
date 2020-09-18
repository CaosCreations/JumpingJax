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

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        tooltip.gameObject.SetActive(false);
    }

    private void Update()
    {
        tooltip.gameObject.SetActive(TransformUtils.RectTransformContainsMouse(rectTransform));
    }

    public void Init(string text, bool isToggled, string tooltipText)
    {
        toggleNameText.text = text;
        toggle.isOn = isToggled;
        tooltip.SetTooltipText(tooltipText);
    }
}
