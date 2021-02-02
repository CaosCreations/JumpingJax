using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum ToggleableUIElements
{
    CrosshairToggle, SpeedToggle, TimeToggle, KeyPressedToggle, TutorialToggle, GhostToggle, ConsoleToggle
}

public class MiscOptions : MonoBehaviour
{
    // Parent transform 
    public Transform scrollViewContent;

    // Toggle-related
    public GameObject togglePrefab;
    public static event Action<ToggleableUIElements> onMiscToggle;
    public static event Action<bool> onGhostToggle;
    public static event Action<bool> onConsoleToggle;
    private List<ToggleItem> toggleItems;

    // Dropdown-related
    public GameObject dropdownItemPrefab;
    public SpeedSlider speedSlider;
    private DropdownItem unitOfSpeedDropdown;

    // Button-related
    public GameObject deleteDataButtonPrefab;
    public OptionsButton deleteDataButtonInstance;
    public OptionsButton deleteLevelEditorButtonInstance;

    void Awake()
    {
        toggleItems = new List<ToggleItem>();
        PopulateToggles();
        SetupUnitOfSpeedDropdown();
        SetupDeleteButtons();
    }

    private void PopulateToggles()
    {
        foreach (ToggleableUIElements element in Enum.GetValues(typeof(ToggleableUIElements)))
        {
            GameObject newToggle = Instantiate(togglePrefab, scrollViewContent);
            string name = element.ToString();
            newToggle.name = name;
            ToggleItem item = newToggle.GetComponent<ToggleItem>();
            item.Init(name, GetToggleableOptionPreference(element), GetTooltip(element));

            if (element == ToggleableUIElements.GhostToggle)
            {
                item.toggle.onValueChanged.AddListener((value) => onGhostToggle?.Invoke(value));
            }
            else if (element == ToggleableUIElements.ConsoleToggle)
            {
                item.toggle.onValueChanged.AddListener((value) => onConsoleToggle?.Invoke(value));
            }
            else
            {
                item.toggle.onValueChanged.AddListener((value) => onMiscToggle?.Invoke(element));
            }

            toggleItems.Add(item);
        }
    }

    private void SetupUnitOfSpeedDropdown()
    {
        GameObject newDropdown = Instantiate(dropdownItemPrefab, scrollViewContent);
        unitOfSpeedDropdown = newDropdown.GetComponent<DropdownItem>();
        unitOfSpeedDropdown.Init
        (
            "Unit of speed", OptionsPreferencesManager.GetUnitOfSpeed(), PlayerConstants.UnitOfSpeedOptions,
            SpeedSlider.SetUnitOfSpeed, PlayerConstants.UnitOfSpeedTooltip
        );
    }

    public static bool GetToggleableOptionPreference(ToggleableUIElements element)
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
            case ToggleableUIElements.ConsoleToggle:
                return OptionsPreferencesManager.GetConsoleToggle();
            default:
                return false;
        }
    }

    public static string GetTooltip(ToggleableUIElements element)
    {
        switch (element)
        {
            case ToggleableUIElements.CrosshairToggle:
                return PlayerConstants.CrosshairTooltip;
            case ToggleableUIElements.SpeedToggle:
                return PlayerConstants.SpeedTooltip;
            case ToggleableUIElements.TimeToggle:
                return PlayerConstants.TimeTooltip;
            case ToggleableUIElements.KeyPressedToggle:
                return PlayerConstants.KeyPressedTooltip;
            case ToggleableUIElements.TutorialToggle:
                return PlayerConstants.TutorialTooltip;
            case ToggleableUIElements.GhostToggle:
                return PlayerConstants.GhostTooltip;
            case ToggleableUIElements.ConsoleToggle:
                return PlayerConstants.ConsoleTooltip;
            default:
                return string.Empty;
        }
    }

    private void SetupDeleteButtons()
    {
        GameObject saveDataObject = Instantiate(deleteDataButtonPrefab, scrollViewContent);
        deleteDataButtonInstance = saveDataObject.GetComponent<OptionsButton>();
        deleteDataButtonInstance.Init("Delete save data", "Delete", DeleteCompletionSaveData, PlayerConstants.DeleteLevelDataTooltip);

        GameObject editorDataObject = Instantiate(deleteDataButtonPrefab, scrollViewContent);
        deleteLevelEditorButtonInstance = editorDataObject.GetComponent<OptionsButton>();
        deleteLevelEditorButtonInstance.Init("Delete level editor data", "Delete", DeleteLevelEditorSaveData, PlayerConstants.DeleteLevelEditorDataTooltip);
    }

    public void DeleteCompletionSaveData()
    {
        FilePathUtil.DeleteLevelData();
    }

    public void DeleteLevelEditorSaveData()
    {
        FilePathUtil.DeleteLevelEditorData();
    }

    public void SetDefaults()
    {
        // Toggle defaults
        OptionsPreferencesManager.SetCrosshairToggle(OptionsPreferencesManager.defaultCrosshairToggle != 0);
        OptionsPreferencesManager.SetSpeedToggle(OptionsPreferencesManager.defaultSpeedToggle != 0);
        OptionsPreferencesManager.SetTimeToggle(OptionsPreferencesManager.defaultTimeToggle != 0);
        OptionsPreferencesManager.SetKeyPressedToggle(OptionsPreferencesManager.defaultKeyPressedToggle != 0);
        OptionsPreferencesManager.SetTutorialToggle(OptionsPreferencesManager.defaultTutorialToggle != 0); 
        OptionsPreferencesManager.SetGhostToggle(OptionsPreferencesManager.defaultGhostToggle != 0); 
        OptionsPreferencesManager.SetConsoleToggle(OptionsPreferencesManager.defaultConsoleToggle != 1);

        foreach(ToggleItem item in toggleItems)
        {
            item.toggle.isOn = true;
        }

        // Dropdown defaults
        OptionsPreferencesManager.SetUnitOfSpeed(OptionsPreferencesManager.defaultUnitOfSpeed);
        unitOfSpeedDropdown.dropdown.value = OptionsPreferencesManager.defaultUnitOfSpeed;
        SpeedSlider.SetUnitOfSpeed(OptionsPreferencesManager.defaultUnitOfSpeed);
    }
}
