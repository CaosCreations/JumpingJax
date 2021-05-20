using System;
using UnityEngine;
using UnityEngine.EventSystems;
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
            CreatePrimaryButton("Continue", mainMenuController.Continue, PlayerConstants.navMode, true);
            CreateSecondaryButton("New Game", mainMenuController.NewGame, PlayerConstants.navMode);
        }
        else
        {
            CreatePrimaryButton("New Game", mainMenuController.NewGame, PlayerConstants.navMode, true);
        }

        CreateSecondaryButton("Level Select", mainMenuController.LevelSelection, PlayerConstants.navMode);
        CreateSecondaryButton("Level Editor", mainMenuController.LevelEditor, PlayerConstants.navMode);
        CreateSecondaryButton("Options", mainMenuController.Options, PlayerConstants.navMode);
        CreateSecondaryButton("Quit", mainMenuController.Quit, PlayerConstants.navMode);
        CreateDiscordButton();
    }

    void CreatePrimaryButton(string text, Action func, Navigation.Mode navMode, bool isSelectedOnStart = false)
    {
        GameObject buttonObject = Instantiate(primaryButtonPrefab, buttonContainer);
        PrimaryButton primaryButton = buttonObject.GetComponent<PrimaryButton>();
        primaryButton.Init(text, func, GetNavigation(navMode));

        if (isSelectedOnStart)
        {
            EventSystem.current.SetSelectedGameObject(buttonObject, new BaseEventData(EventSystem.current));
        }
    }

    void CreateSecondaryButton(string text, Action func, Navigation.Mode navMode)
    {
        GameObject buttonObject = Instantiate(secondaryButtonPrefab, buttonContainer);
        SecondaryButton secondaryButton = buttonObject.GetComponent<SecondaryButton>();
        secondaryButton.Init(text, func, GetNavigation(navMode));
    }

    private Navigation GetNavigation(Navigation.Mode navigationMode)
    {
        Navigation navigation = new Navigation { mode = navigationMode };

        return navigation;
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
