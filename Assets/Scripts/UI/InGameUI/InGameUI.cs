using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUI : MonoBehaviour
{
    // Time
    public GameObject timeContainer;

    public TMP_Text completionTimeText;

    // Speed
    public SpeedSlider speedBar;

    // Crosshair
    public GameObject crosshair;

    // KeyPresses
    public GameObject keyPressed;

    // Tutorial
    public TMP_Text tutorialText;

    public GameObject tutorialPane;
    private string[] tutorialTexts;
    private int tutorialTextIndex = 0;

    public GameObject container;

    // Used for Ghost
    public static Color ghostColor = new Color(255 / 255f, 124 / 255f, 50 / 255f); // burnt orange color

    public static Color normalColor = new Color(149 / 255f, 237 / 255f, 194 / 255f); // light green color
    public static Color inactiveColor = new Color(1, 1, 1, 1); // light green color
    public bool IsGhosting = false;
    public PlayerGhostRun ghostRun;
    private Crosshair playerCrosshair;
    private Transform player;

    public float currentSpeed;
    public Vector3 positionLastFrame;

    public Image[] imagesToUpdateColor;
    public TMP_Text[] textsToUpdateColor;

    private void Start()
    {
        speedBar = GetComponentInChildren<SpeedSlider>();
        ghostRun = GetComponentInParent<PlayerGhostRun>();
        player = ghostRun.transform;
        playerCrosshair = GetComponentInParent<Crosshair>();

        SetupTutorialTexts(GameManager.GetCurrentLevel().tutorialTexts);

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

        //currentSpeed is by default set to the ghost velocity at this time
        if (!IsGhosting)
        {
            Vector3 offestSinceLastFrame = player.position - positionLastFrame;
            offestSinceLastFrame.y = 0;
            currentSpeed = offestSinceLastFrame.magnitude / Time.deltaTime;
        }

        speedBar.SetSpeed(currentSpeed);

        if (Input.GetKeyDown(PlayerConstants.NextTutorial) && !IsGhosting && tutorialPane.activeInHierarchy)
        {
            LoadNextTutorial();
        }
        else if (InputManager.GetKeyDown(PlayerConstants.ToggleUI))
        {
            ToggleUI();
        }

        positionLastFrame = player.position;
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
            tutorialText.text = tutorialTexts[tutorialTextIndex].InsertCustomHotKeys().InsertNewLines();
            tutorialText.text += "\nPress TAB to Continue";
            tutorialTextIndex++;
            tutorialPane.SetActive(true);
        }
        else
        {
            tutorialPane.SetActive(false);
        }
    }

    public void SetupTutorialTexts(string[] texts)
    {
        if (texts != null)
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

    public void ToggleUI()
    {
        container.SetActive(!container.activeInHierarchy);
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
            tutorialPane.SetActive(!tutorialPane.activeInHierarchy && tutorialTexts.Length > 0);
            OptionsPreferencesManager.SetTutorialToggle(tutorialPane.activeInHierarchy);
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
        if (tutorialTexts != null)
        {
            bool tutorialPaneActive = (OptionsPreferencesManager.GetTutorialToggle() && tutorialTexts.Length > 0) || IsGhosting;
            tutorialPane.SetActive(tutorialPaneActive);
        }
    }

    public void ToggleGhostUI()
    {
        Color UIcolor = IsGhosting ? ghostColor : normalColor;
        playerCrosshair.Init();
        playerCrosshair.crosshair.color = UIcolor;

        if (imagesToUpdateColor != null)
        {
            foreach (Image image in imagesToUpdateColor)
            {
                image.color = UIcolor;
            }
        }

        if (textsToUpdateColor != null)
        {
            foreach (TMP_Text text in textsToUpdateColor)
            {
                text.color = UIcolor;
            }
        }

        if (IsGhosting)
        {
            tutorialText.text = $"Spectating: {ghostRun.pastRunPlayerSteamName}";
            tutorialPane.SetActive(true);
        }
        else
        {
            SetupTutorialTexts(null);
        }

        tutorialText.color = UIcolor; //tutorialText.color is unable to be allocated during Start(), handled here.

        CheckElementsShouldBeActive();
    }
}