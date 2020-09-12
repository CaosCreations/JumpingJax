using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    // Time
    public Text completionTimeText;

    // Speed
    public SpeedSlider speed;

    // Crosshair 
    public GameObject crosshair;

    // KeyPresses 
    public KeyPressed keyPressed;

    // Tutorial
    public Text tutorialText;
    public Text tutorialNextText;
    public GameObject tutorialPane;
    private string[] tutorialTexts;
    private int tutorialTextIndex = 0;


    public GameObject container;
    public PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        speed = GetComponentInChildren<SpeedSlider>();
        keyPressed = GetComponentInChildren<KeyPressed>();

        tutorialTexts = GameManager.GetCurrentLevel().tutorialTexts;
        LoadNextTutorial();

        SetToggleStartingValues();
        MiscOptions.onToggle += ToggleIndividual;
    }

    void Update()
    {
        if (GameManager.Instance != null)
        {
            TimeSpan time = TimeSpan.FromSeconds(GameManager.Instance.currentCompletionTime);
            completionTimeText.text = time.ToString("hh':'mm':'ss");
        }

        //fpsText.text = "FPS: " + Mathf.Round(1 / Time.deltaTime);
        Vector2 directionalSpeed = new Vector2(playerMovement.newVelocity.x, playerMovement.newVelocity.z);
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
        if (tutorialTexts == null)
        {
            return;
        }

        if (tutorialTextIndex < tutorialTexts.Length)
        {
            tutorialPane.SetActive(true);
            tutorialText.text = tutorialTexts[tutorialTextIndex].Replace("<br>", "\n");
            Invoke("UpdateParentLayoutGroup", 0.1f);
            tutorialTextIndex++;
        }
        else
        {
            tutorialPane.SetActive(false);
        }
    }

    void UpdateParentLayoutGroup()
    {
        tutorialText.gameObject.SetActive(false);
        tutorialText.gameObject.SetActive(true);

        tutorialNextText.gameObject.SetActive(false);
        tutorialNextText.gameObject.SetActive(true);
    }
    private void ToggleUI()
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
                completionTimeText.gameObject.SetActive(!completionTimeText.gameObject.activeSelf);
                OptionsPreferencesManager.SetTimeToggle(completionTimeText.gameObject.activeSelf);
                break;
            case ToggleableUIElements.SpeedToggle:
                speed.gameObject.SetActive(!speed.gameObject.activeSelf);
                OptionsPreferencesManager.SetSpeedToggle(speed.gameObject.activeSelf);
                break;
            case ToggleableUIElements.TutorialToggle:
                tutorialText.gameObject.SetActive(!tutorialText.gameObject.activeSelf);
                OptionsPreferencesManager.SetCrosshairToggle(tutorialText.gameObject.activeSelf);
                break;
            case ToggleableUIElements.KeyPressedToggle:
                keyPressed.gameObject.SetActive(!keyPressed.gameObject.activeSelf);
                OptionsPreferencesManager.SetCrosshairToggle(keyPressed.gameObject.activeSelf);
                break;
        }
    }
    private void SetToggleStartingValues()
    {
        crosshair.SetActive(OptionsPreferencesManager.GetCrosshairToggle() == 1 ? true : false);
        speed.gameObject.SetActive(OptionsPreferencesManager.GetSpeedToggle() == 1 ? true : false);
        completionTimeText.gameObject.SetActive(OptionsPreferencesManager.GetTutorialToggle() == 1 ? true : false);
        keyPressed.gameObject.SetActive(OptionsPreferencesManager.GetKeyPressedToggle() == 1 ? true : false);
        tutorialText.gameObject.SetActive(OptionsPreferencesManager.GetTutorialToggle() == 1 ? true : false);
    }
}
