using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinMenu : MonoBehaviour
{
    public Text levelText;
    public Text completionTimeText;
    public Text bestTimeText;

    public Button retryButton;
    public Button menuButton;
    public Button nextButton;

    private PlayerProgress playerProgress;

    private void Start()
    {
        playerProgress = GetComponentInParent<PlayerProgress>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(PlayerConstants.WinMenu_MainMenu)){
            GoToMainMenu();
        }

        if (Input.GetKeyDown(PlayerConstants.WinMenu_NextLevel)){
            NextLevel();
        }

        if (Input.GetKeyDown(PlayerConstants.WinMenu_RetryLevel)){
            Retry();
        }
    }

    private void OnEnable()
    {
        // Only show the "next" button if it is NOT a workshop map
        nextButton.gameObject.SetActive(GameManager.GetCurrentLevel().filePath == string.Empty);
    }

    public void Retry()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = false;
        playerProgress.ResetPlayer();
    }

    public void NextLevel()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;

        Level currentLevel = GameManager.GetCurrentLevel();
        // Load credits scene
        if (currentLevel.levelBuildIndex >= GameManager.Instance.levelDataContainer.levels.Length)
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }

        GameManager.NextLevel();
        GameManager.LoadScene(currentLevel.levelBuildIndex + 1);
    }

    public void GoToMainMenu()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = true;
        GameManager.LoadScene(PlayerConstants.BuildSceneIndex);
    }
}
