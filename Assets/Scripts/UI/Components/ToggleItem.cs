using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleItem : MonoBehaviour
{
    public Text toggleNameText;
    public Toggle toggle;

    public void Init(string text, bool isToggled)
    {
        toggleNameText.text = text;
        toggle.isOn = isToggled;
    }
}
