using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    // Time
    public GameObject timeContainer;
    public Text completionTimeText;

    // Speed
    public SpeedSlider speedBar;

    // Crosshair 
    public GameObject crosshair;

    // KeyPresses 
    public GameObject keyPressed;

    // Tutorial
    public Text tutorialText;
    public Text tutorialNextText;
    public GameObject tutorialPane;
    private string[] tutorialTexts;
    private int tutorialTextIndex = 0;


    public GameObject container;
    public PlayerMovement playerMovement;

    // Used for Ghost
    public Color ghostColor = new Color(255 / 255f, 124 / 255f, 50 / 255f); // burnt orange color
    public Color normalColor = new Color(149 / 255f, 237 / 255f, 194 / 255f); // light green color
    public bool IsGhosting = false;
    public PlayerGhostRun ghostRun;

    public float currentSpeed;

    private List<Image> imagesToUpdateColor;
    private List<Text> textsToUpdateColor;


    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        speedBar = GetComponentInChildren<SpeedSlider>();
        ghostRun = GetComponentInParent<PlayerGhostRun>();

        tutorialTexts = GameManager.GetCurrentLevel().tutorialTexts;
        LoadNextTutorial();

        GetUIForColors();
        CheckElementsShouldBeActive();
        ToggleGhostUI();
        MiscOptions.onMiscToggle += ToggleIndividual;

        HotKeyManager.Instance.onHotKeySet += UpdateTutorialTextHotkey;
        HotKeyOptions.onSetDefaults += ResetTutorialText;
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(GameManager.Instance.currentCompletionTime);
            completionTimeText.text = time.ToString("hh':'mm':'ss");
        }

        if (!IsGhosting)
        {
            currentSpeed = new Vector2(playerMovement.velocityToApply.x, playerMovement.velocityToApply.z).magnitude;
        }

        speedBar.SetSpeed(currentSpeed);

        if (Input.GetKeyDown(PlayerConstants.NextTutorial) && !IsGhosting)
        {
            LoadNextTutorial();
        }
        else if (InputManager.GetKeyDown(PlayerConstants.ToggleUI))
        {
            ToggleUI();
        }
    }

    private void GetUIForColors()
    {
        if(imagesToUpdateColor == null)
        {
            imagesToUpdateColor = GetComponentsInChildren<Image>().ToList();
        }

        if(textsToUpdateColor == null)
        {
            textsToUpdateColor = GetComponentsInChildren<Text>().ToList();
        }
    }

    private void LoadNextTutorial()
    {
        if (tutorialTexts == null || tutorialTexts.Length == 0)
        {
            tutorialPane.SetActive(false);
            return;
        }

        if (tutorialTextIndex < tutorialTexts.Length)
        {
            tutorialPane.SetActive(true);
            tutorialText.text = tutorialTexts[tutorialTextIndex].InsertCustomHotKeys().InsertNewLines();
            Invoke("UpdateParentLayoutGroup", 0.1f);
            tutorialTextIndex++;
        }
        else
        {
            tutorialPane.SetActive(false);
        }
    }

    public void SetupTutorialTexts(string[] texts)
    {
        if(texts != null)
        {
            tutorialTexts = texts;
        }
        tutorialTextIndex = 0;
        LoadNextTutorial();
    }

    private void UpdateTutorialTextHotkey(KeyCode oldKeyCode, KeyCode newKeyCode)
    {
        tutorialText.text = tutorialText.text.InsertSpecificHotKey(oldKeyCode, newKeyCode);
    }

    private void ResetTutorialText()
    {
        if (tutorialTexts != null && tutorialTextIndex >= 1)
        {
            tutorialText.text = tutorialTexts[tutorialTextIndex - 1].InsertNewLines();
        }
    }
	
    void UpdateParentLayoutGroup()
    {
        tutorialText.gameObject.SetActive(false);
        tutorialText.gameObject.SetActive(true);

        tutorialNextText.gameObject.SetActive(false);
        tutorialNextText.gameObject.SetActive(true);
    }

    public void ToggleUI()
    {
        container.SetActive(!container.activeSelf);
    }

    private void ToggleIndividual(ToggleableUIElements element)
    {
        switch (element)
        {
            case ToggleableUIElements.CrosshairToggle:
                crosshair.SetActive(!crosshair.activeSelf);
                OptionsPreferencesManager.SetCrosshairToggle(crosshair.activeSelf);
                break;
            case ToggleableUIElements.TimeToggle:
                timeContainer.gameObject.SetActive(!timeContainer.activeSelf);
                OptionsPreferencesManager.SetTimeToggle(timeContainer.activeSelf);
                break;
            case ToggleableUIElements.SpeedToggle:
                speedBar.gameObject.SetActive(!speedBar.gameObject.activeSelf);
                OptionsPreferencesManager.SetSpeedToggle(speedBar.gameObject.activeSelf);
                break;
            case ToggleableUIElements.TutorialToggle:
                tutorialPane.SetActive(!tutorialPane.activeSelf && tutorialTexts.Length > 0);
                OptionsPreferencesManager.SetTutorialToggle(tutorialPane.activeSelf);
                Invoke("UpdateParentLayoutGroup", 0.1f);
                break;
            case ToggleableUIElements.KeyPressedToggle:
                keyPressed.SetActive(!keyPressed.activeSelf);
                OptionsPreferencesManager.SetKeyPressedToggle(keyPressed.gameObject.activeSelf);
                break;
        }
    }

    private void CheckElementsShouldBeActive()
    {
        crosshair.SetActive(OptionsPreferencesManager.GetCrosshairToggle());
        speedBar.gameObject.SetActive(OptionsPreferencesManager.GetSpeedToggle());
        timeContainer.gameObject.SetActive(OptionsPreferencesManager.GetTimeToggle());
        keyPressed.SetActive(OptionsPreferencesManager.GetKeyPressedToggle());
        tutorialPane.SetActive(OptionsPreferencesManager.GetTutorialToggle() && tutorialTexts.Length > 0);
    }

    public void ToggleGhostUI()
    {
        Color UIcolor = IsGhosting ? ghostColor : normalColor;

        if(imagesToUpdateColor != null)
        {
            foreach (Image image in imagesToUpdateColor)
            {
                image.color = UIcolor;
            }
        }
        
        if(textsToUpdateColor != null)
        {
            foreach (Text text in textsToUpdateColor)
            {
                text.color = UIcolor;
            }
        }

        if (IsGhosting)
        {
            tutorialPane.SetActive(true);
            tutorialText.text = $"Spectating: {ghostRun.pastRunPlayerSteamName}";
            tutorialText.color = UIcolor; //tutorialText.color is unable to be allocated during Start(), handled here.
            tutorialNextText.gameObject.SetActive(false);
        }
        else
        {
            SetupTutorialTexts(null);
            tutorialNextText.gameObject.SetActive(true);
        }

        CheckElementsShouldBeActive();
    }
}
