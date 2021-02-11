using System;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    // Time
    public GameObject timeContainer;
    public Text completionTimeText;

    // Speed
    public SpeedSlider speed;

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

    public bool IsGhost;
    public PlayerGhostRun ghostRun;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        speed = GetComponentInChildren<SpeedSlider>();
        ghostRun = GameObject.FindObjectOfType<PlayerGhostRun>();

        tutorialTexts = GameManager.GetCurrentLevel().tutorialTexts;
        LoadNextTutorial();

        SetStartingValues();
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

        Vector2 directionalSpeed;
        if (!IsGhost)
        {
            directionalSpeed = new Vector2(playerMovement.velocityToApply.x, playerMovement.velocityToApply.z);
        }
        else
        {
            directionalSpeed = ghostRun.GetGhostVelocity();
        }

        speed.SetSpeed(directionalSpeed.magnitude);

        if (Input.GetKeyDown(PlayerConstants.NextTutorial))
        {
            LoadNextTutorial();
        }
        else if (InputManager.GetKeyUp(PlayerConstants.ToggleUI))
        {
            ToggleUI();
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
        tutorialTexts = texts;
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
                speed.gameObject.SetActive(!speed.gameObject.activeSelf);
                OptionsPreferencesManager.SetSpeedToggle(speed.gameObject.activeSelf);
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

    private void SetStartingValues()
    {
        crosshair.SetActive(OptionsPreferencesManager.GetCrosshairToggle());
        speed.gameObject.SetActive(OptionsPreferencesManager.GetSpeedToggle());
        timeContainer.gameObject.SetActive(OptionsPreferencesManager.GetTimeToggle());
        keyPressed.SetActive(OptionsPreferencesManager.GetKeyPressedToggle());
        if (IsGhost)
        {
            tutorialPane.SetActive(false);
        }
        else
        {
            tutorialPane.SetActive(OptionsPreferencesManager.GetTutorialToggle() && tutorialTexts.Length > 0);
        }
    }

    public void SetColors()
    {
        Color UIcolor;
        if (IsGhost)
        {
            UIcolor = new Color(204 / 255f, 85 / 255f, 0); // burnt orange color
        } else
        {
            UIcolor = new Color(1, 1, 1);
        }
        crosshair.GetComponent<Image>().color = UIcolor;
        foreach (Image SpeedImage in speed.GetComponentsInChildren<Image>())
        {
            SpeedImage.color = UIcolor;
        }
        speed.GetComponentInChildren<Text>().color = UIcolor;
        timeContainer.GetComponentInChildren<Text>().color = UIcolor;
        timeContainer.GetComponentInChildren<Image>().color = UIcolor;
        foreach (Image KeyPressImage in keyPressed.GetComponents<Image>())
        {
            KeyPressImage.color = UIcolor;
        }
        foreach (Text TutorialText in tutorialPane.GetComponents<Text>())
        {
            TutorialText.color = UIcolor;
        }
        SetStartingValues();
    }
}
