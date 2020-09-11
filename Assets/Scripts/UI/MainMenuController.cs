using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject homePanel;
    public GameObject levelSelectPanel;
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
        achievementsPanel.SetActive(false);
        pauseMenu.UnPause();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void LevelSelection()
    {
        homePanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        achievementsPanel.SetActive(false);
    }

    public void Achievements()
    {
        homePanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        achievementsPanel.SetActive(true);
    }

    public void Options()
    {
        pauseMenu.Pause();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
