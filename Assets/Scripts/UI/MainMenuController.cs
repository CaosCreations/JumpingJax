using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject levelSelectPanel;
    public GameObject levelEditorPanel;
    public GameObject achievementsPanel;
    public PauseMenu pauseMenu;
    public GameObject alphaPanel;
    public Button alphaContinueButton;

    private void Start()
    {
        alphaContinueButton.onClick.RemoveAllListeners();
        alphaContinueButton.onClick.AddListener(AlphaMenuContinue);
        Init();
    }

    
    public void Init()
    {
        alphaPanel.SetActive(OptionsPreferencesManager.GetAlphaToggle());
        homePanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        levelEditorPanel.SetActive(false);
        achievementsPanel.SetActive(false);
        pauseMenu.UnPause();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void NewGame()
    {
        GameManager.LoadScene(1);
    }

    public void Continue()
    {
        int latestLevelIndex = 1;

        foreach (Level level in GameManager.Instance.levelDataContainer.levels)
        {
            if (level.levelSaveData.isCompleted)
            {
                latestLevelIndex = level.levelBuildIndex;
            }
        }
        if (latestLevelIndex < GameManager.Instance.levelDataContainer.levels.Length - 1)
        {
            GameManager.LoadScene(latestLevelIndex + 1);
        }
        else
        {
            GameManager.LoadScene(latestLevelIndex);
        }
    }

    public void LevelSelection()
    {
        homePanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        levelEditorPanel.SetActive(false);
        achievementsPanel.SetActive(false);
        alphaPanel.SetActive(false);
    }

    public void Achievements()
    {
        homePanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        levelEditorPanel.SetActive(false);
        achievementsPanel.SetActive(true);
        alphaPanel.SetActive(false);
    }

    public void LevelEditor()
    {
        homePanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        levelEditorPanel.SetActive(true);
        achievementsPanel.SetActive(false);
        alphaPanel.SetActive(false);
    }

    public void Options()
    {
        pauseMenu.Pause();
    }

    public void AlphaMenuContinue()
    {
        OptionsPreferencesManager.SetAlphaToggle(false);
        Init();
    }

    public void Quit()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
