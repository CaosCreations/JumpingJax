﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
    [SerializeField]
    GameObject optionsMenu = null;

    [SerializeField]
    GameObject pauseMenuHome = null;

    [SerializeField]
    GameObject pauseMenuContainer = null;

    [SerializeField]
    Text levelName = null;

    public bool isPaused;

    private DeveloperConsole console;


    private void Start()
    {
        pauseMenuContainer.SetActive(false);
        console = transform.parent.GetComponentInChildren<DeveloperConsole>();
    }

    void Update()
    {
        // Don't let the player pause the game if they are in the win menu
        // This would let the player unpause and play during the win menu
        if (GameManager.Instance.didWinCurrentLevel)
        {
            return;
        }

        // Also don't let the player pause while the dev console is open or it will throw off the time scale
        if (console != null && console.consoleIsActive)
        {
            return;
        }

        if (Input.GetKeyDown(PlayerConstants.PauseMenu))
        {
            if (isPaused)
            {
                UnPause();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        isPaused = true;
        pauseMenuContainer.SetActive(true);
        pauseMenuHome.SetActive(true);
        optionsMenu.SetActive(false);
        levelName.text = SceneManager.GetActiveScene().name;

        // If we aren't in the main menu
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
        }
        else
        {
            ToggleOptionsMenu();
        }
    }

    public void UnPause()
    {
        isPaused = false;
        pauseMenuContainer.SetActive(false);

        // If we aren't in the main menu
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }

    public void ToggleOptionsMenu() {
        pauseMenuHome.SetActive(!pauseMenuHome.activeSelf);
        optionsMenu.SetActive(!optionsMenu.activeSelf);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        GameManager.LoadScene(PlayerConstants.BuildSceneIndex);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
