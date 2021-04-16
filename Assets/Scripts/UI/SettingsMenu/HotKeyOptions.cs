using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotKeyOptions : MonoBehaviour
{
    private Transform scrollViewContent;
    public GameObject hotKeySelectionPrefab;
    public GameObject sliderPrefab;
    public GameObject togglePrefab;


    string keyToRebind = null;
    Dictionary<string, Text> buttonKeyCodeTexts;
    CameraMove playerAiming;
    PlayerMovement playerMovement;
    SliderItem currentSliderItem;
    ToggleItem currentToggleItem;

    public static event Action onSetDefaults;

    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerAiming = playerMovement.GetComponentInChildren<CameraMove>();
        }

        scrollViewContent = GetComponentInChildren<ContentSizeFitter>().transform;
        ReloadUI();
    }

    private void Update()
    {
        if(keyToRebind != null)
        {
            if (Input.anyKeyDown)
            {
                // Loop through all possible keys and see if it was pressed down
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        HotKeyManager.Instance.SetButtonForKey(keyToRebind, keyCode);
                        buttonKeyCodeTexts[keyToRebind].text = keyCode.ToString();
                        keyToRebind = null;
                        break;
                    }
                }
            }
        }
    }

    void ReloadUI()
    {
        currentSliderItem = null;
        currentToggleItem = null;
        CleanScrollView();
        PopulateHotkeys();
        SetupSensitivitySlider();
        SetupJumpOnScrollCheckbox();
    }

    void StartRebindFor(string keyName)
    {
        Debug.Log("StartRebindFor: " + keyName);

        keyToRebind = keyName;
    }

    private void CleanScrollView()
    {
        foreach(Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void PopulateHotkeys()
    {
        Dictionary<string, KeyCode> keys = HotKeyManager.Instance.GetHotKeys();
        Dictionary<string, string> tooltips = HotKeyManager.Instance.GetTooltips();

        buttonKeyCodeTexts = new Dictionary<string, Text>();
        foreach(string hotkey in keys.Keys)
        {
            GameObject newItem = Instantiate(hotKeySelectionPrefab);
            newItem.transform.SetParent(scrollViewContent, false);

            HotKeyItem item = newItem.GetComponentInChildren<HotKeyItem>();
            item.SetItemText(hotkey);
            item.SetButtonText(keys[hotkey].ToString());
            item.tooltip.SetTooltipText(tooltips[hotkey]);
            item.itemButton.onClick.AddListener(() => StartRebindFor(hotkey.ToString()));

            buttonKeyCodeTexts.Add(hotkey, item.GetButtonText());
        }
    }

    private void SetupSensitivitySlider()
    {
        if(currentSliderItem == null)
        {
            GameObject sliderObject = Instantiate(sliderPrefab, scrollViewContent);
            currentSliderItem = sliderObject.GetComponent<SliderItem>();
        }

        currentSliderItem.Init(OptionsPreferencesManager.sensitivityKey, OptionsPreferencesManager.GetSensitivity(), SetSensitivity, 0.01f, 1, false, PlayerConstants.SensitivityTooltip);
        string inputText = Mathf.RoundToInt(OptionsPreferencesManager.GetSensitivity() * 100) + "%";
        currentSliderItem.input.text = inputText;
    }

    private void SetupJumpOnScrollCheckbox()
    {
        if (currentToggleItem == null)
        {
            GameObject toggleObject = Instantiate(togglePrefab, scrollViewContent);
            currentToggleItem = toggleObject.GetComponent<ToggleItem>();
        }
        currentToggleItem.Init("Jump on Scroll", OptionsPreferencesManager.GetJumpOnScroll(), SetJumpOnScroll, PlayerConstants.JumpOnScrollTooltip);
    }

    public void SetJumpOnScroll(bool shouldJumpOnScroll)
    {
        if(playerMovement != null)
        {
            playerMovement.shouldJumpOnScroll = shouldJumpOnScroll;
        }

        OptionsPreferencesManager.SetJumpOnScroll(shouldJumpOnScroll);
    }

    public void SetSensitivity(float sensitivity)
    {
        if (playerAiming != null)
        {
            playerAiming.sensitivityMultiplier = sensitivity;
        }

        int percentage = Mathf.RoundToInt(sensitivity * 100);
        currentSliderItem.SetInput(percentage + "%");
        currentSliderItem.SetSliderValue(sensitivity);

        OptionsPreferencesManager.SetSensitivity(sensitivity);
    }

    public void SetDefaults()
    {
        Debug.Log("set hotkey defaults");
        HotKeyManager.Instance.SetDefaults();
        onSetDefaults?.Invoke();
        currentSliderItem.SetSliderValue(OptionsPreferencesManager.defaultSensitivity);
        ReloadUI();
    }
}
