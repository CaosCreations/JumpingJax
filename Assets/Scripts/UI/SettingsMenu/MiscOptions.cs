using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum ToggleableUIElements
{
    CrosshairToggle, SpeedToggle, TimeToggle, KeyPressedToggle, TutorialToggle
}

public class MiscOptions : MonoBehaviour
{
    public GameObject togglePrefab;
    public Transform scrollViewContent;

    public delegate void OnToggle(ToggleableUIElements element);
    public static event OnToggle onToggle;

    void Awake()
    {
        Populate();
    }

    private void Populate()
    {
        foreach (ToggleableUIElements element in Enum.GetValues(typeof(ToggleableUIElements)))
        {
            GameObject newToggle = Instantiate(togglePrefab, scrollViewContent);
            string name = element.ToString();
            newToggle.name = name;
            ToggleItem item = newToggle.GetComponent<ToggleItem>();
            int optionPreferenceValue = GetOptionPreference(element);
            item.Init(name, optionPreferenceValue == 1 ? true : false);
            item.toggle.onValueChanged.AddListener((value) => onToggle?.Invoke(element));
        }
    }

    public static int GetOptionPreference(ToggleableUIElements element)
    {
        switch (element)
        {
            case ToggleableUIElements.CrosshairToggle:
                return OptionsPreferencesManager.GetCrosshairToggle();
            case ToggleableUIElements.SpeedToggle:
                return OptionsPreferencesManager.GetSpeedToggle();
            case ToggleableUIElements.TimeToggle:
                return OptionsPreferencesManager.GetTimeToggle();
            case ToggleableUIElements.KeyPressedToggle:
                return OptionsPreferencesManager.GetKeyPressedToggle();
            case ToggleableUIElements.TutorialToggle:
                return OptionsPreferencesManager.GetTutorialToggle();
            default:
                return 0;
        }
    }

    public void SetDefaults()
    {

    }

    
}
