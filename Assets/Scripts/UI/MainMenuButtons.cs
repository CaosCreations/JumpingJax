using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
    public Button newGameButton;
    public Button levelSelectButton;
    public Button levelEditorButton;
    public Button optionsButton;
    public Button quitButton;

    public Image swoosh;

    public MainMenuController mainMenuController;
    void Start()
    {
        mainMenuController = GetComponentInParent<MainMenuController>();
        SetupButtons();
    }

    void SetupButtons()
    {
        newGameButton.onClick.RemoveAllListeners();
        newGameButton.onClick.AddListener(mainMenuController.NewGame);

        levelSelectButton.onClick.RemoveAllListeners();
        levelSelectButton.onClick.AddListener(mainMenuController.LevelSelection);

        levelEditorButton.onClick.RemoveAllListeners();
        levelEditorButton.onClick.AddListener(mainMenuController.LevelEditor);

        optionsButton.onClick.RemoveAllListeners();
        optionsButton.onClick.AddListener(mainMenuController.Options);

        quitButton.onClick.RemoveAllListeners();
        quitButton.onClick.AddListener(mainMenuController.Quit);
    }

    void Update()
    {
        
    }
}
