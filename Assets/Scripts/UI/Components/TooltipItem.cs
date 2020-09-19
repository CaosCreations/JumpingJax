using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipItem : MonoBehaviour
{
    [SerializeField]
    private Text tooltipText;

    public void SetTooltipText(string text)
    {
        tooltipText.text = text;
    }
}
