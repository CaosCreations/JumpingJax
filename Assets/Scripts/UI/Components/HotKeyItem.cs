using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotKeyItem : MonoBehaviour
{
    public Text itemText;
    public Button itemButton;
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

    public void SetItemText(string text)
    {
        itemText.text = text;
    }

    public void SetButtonText(string text)
    {
        buttonText.text = text;
    }

    public Text GetButtonText()
    {
        return buttonText;
    }
}
