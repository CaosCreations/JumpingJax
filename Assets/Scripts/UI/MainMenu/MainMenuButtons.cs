using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject primaryButtonPrefab;
    public GameObject secondaryButtonPrefab;

    public MainMenuController mainMenuController;
    public Transform buttonContainer;

    public Button discordButton;
    public Text gameVersionText;

    void Start()
    {
        mainMenuController = GetComponentInParent<MainMenuController>();
        SetupButtons();
        SetupVersionText();
    }

    void SetupButtons()
    {
        bool canContinue = ShouldAddContinueButton();
        if (canContinue)
        {
            CreatePrimaryButton("Continue", mainMenuController.Continue);
            CreateSecondaryButton("New Game", mainMenuController.NewGame);
        }
        else
        {
            CreatePrimaryButton("New Game", mainMenuController.NewGame);
        }

        CreateSecondaryButton("Level Select", mainMenuController.LevelSelection);
        CreateSecondaryButton("Level Editor", mainMenuController.LevelEditor);
        CreateSecondaryButton("Options", mainMenuController.Options);
        CreateSecondaryButton("Quit", mainMenuController.Quit);
        CreateDiscordButton();
    }

    void CreatePrimaryButton(string text, Action func)
    {
        GameObject buttonObject = Instantiate(primaryButtonPrefab, buttonContainer);
        PrimaryButton primaryButton = buttonObject.GetComponent<PrimaryButton>();
        primaryButton.Init(text, func);
    }

    void CreateSecondaryButton(string text, Action func)
    {
        GameObject buttonObject = Instantiate(secondaryButtonPrefab, buttonContainer);
        SecondaryButton secondaryButton = buttonObject.GetComponent<SecondaryButton>();
        secondaryButton.Init(text, func);
    }

    void CreateDiscordButton()
    {
        discordButton.onClick.RemoveAllListeners();
        discordButton.onClick.AddListener(OpenDiscordLink);
    }

    private void OpenDiscordLink()
    {
        Application.OpenURL(PlayerConstants.DiscordURL);
    }

    private void SetupVersionText()
    {
        gameVersionText.text = Application.version; 
    }

    bool ShouldAddContinueButton()
    {
        Level[] allLevels = GameManager.Instance.levelDataContainer.levels;

        foreach (Level level in allLevels)
        {
            if (level.levelSaveData.isCompleted)
            {
                return true;
            }
        }
        return false;
    }
}
