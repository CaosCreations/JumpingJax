using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    public Text labelText;
    public Button button;
    public Text buttonText;
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

    public void Init(string labelText, string buttonText, Action callback, string tooltipText)
    {
        this.labelText.text = labelText;
        this.buttonText.text = buttonText;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback());

        tooltip.SetTooltipText(tooltipText);
    }
}
