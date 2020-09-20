using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject levelSelectPanel;
    public GameObject levelEditorPanel;
    public GameObject achievementsPanel;
    public PauseMenu pauseMenu;

    private void Start()
    {
        Init();
    }

    
    public void Init()
    {
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

    public void LevelSelection()
    {
        homePanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        levelEditorPanel.SetActive(false);
        achievementsPanel.SetActive(false);
    }

    public void Achievements()
    {
        homePanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        levelEditorPanel.SetActive(false);
        achievementsPanel.SetActive(true);
    }

    public void LevelEditor()
    {
        homePanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        levelEditorPanel.SetActive(true);
        achievementsPanel.SetActive(false);
    }

    public void Options()
    {
        pauseMenu.Pause();
    }

    public void Quit()
    {
        if (Application.isEditor)
        {
            EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
