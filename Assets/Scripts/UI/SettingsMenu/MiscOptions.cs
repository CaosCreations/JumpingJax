using System;
using UnityEngine;

public enum ToggleableUIElements
{
    CrosshairToggle, SpeedToggle, TimeToggle, KeyPressedToggle, TutorialToggle, GhostToggle
}

public class MiscOptions : MonoBehaviour
{
    public GameObject togglePrefab;
    public Transform scrollViewContent;

    public static event Action<ToggleableUIElements> onMiscToggle;
    public static event Action<bool> onGhostToggle;

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
            bool optionPreferenceValue = GetOptionPreference(element);
            item.Init(name, optionPreferenceValue);

            if (element != ToggleableUIElements.GhostToggle)
            {
                item.toggle.onValueChanged.AddListener((value) => onMiscToggle?.Invoke(element));

            }
            else
            {
                //item.toggle.onValueChanged.AddListener((value) => onGhostToggle?.Invoke(value));
                item.toggle.onValueChanged.AddListener((value) => OptionsPreferencesManager.SetGhostToggle(value));
            }

        }
    }

    public static bool GetOptionPreference(ToggleableUIElements element)
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
            case ToggleableUIElements.GhostToggle:
                return OptionsPreferencesManager.GetGhostToggle();
            default:
                return false;
        }
    }

    public void SetDefaults()
    {
        OptionsPreferencesManager.SetCrosshairToggle(OptionsPreferencesManager.defaultCrosshairToggle != 0);
        OptionsPreferencesManager.SetSpeedToggle(OptionsPreferencesManager.defaultSpeedToggle != 0);
        OptionsPreferencesManager.SetTimeToggle(OptionsPreferencesManager.defaultTimeToggle != 0);
        OptionsPreferencesManager.SetKeyPressedToggle(OptionsPreferencesManager.defaultKeyPressedToggle != 0);
        OptionsPreferencesManager.SetTutorialToggle(OptionsPreferencesManager.defaultTutorialToggle != 0); 
    }
}
