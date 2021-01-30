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

    public SecondaryButton retryButton;
    public SecondaryButton menuButton;
    public PrimaryButton nextButton;

    private PlayerProgress playerProgress;

    private void Awake()
    {
        playerProgress = GetComponentInParent<PlayerProgress>();
        SetupButtons();
    }

    private void Update()
    {
        if(Input.GetKeyDown(PlayerConstants.WinMenu_MainMenu)){
            GoToMainMenu();
        }

        if (Input.GetKeyDown(PlayerConstants.WinMenu_NextLevel)){
            NextLevel();
        }

        if (Input.GetKeyDown(PlayerConstants.WinMenu_RetryLevel) || InputManager.GetKeyDown(PlayerConstants.ResetLevel))
        {
            Retry();
        }
    }

    private async void OnEnable()
    {
        levelText.text = "You found Jax on: " + GameManager.GetCurrentLevel().levelName;
        if(GameManager.Instance != null)
        {
            completionTimeText.text = TimeUtils.GetTimeString(GameManager.Instance.currentCompletionTime);
        }
        float bestTime = await StatsManager.GetLevelCompletionTime(GameManager.GetCurrentLevel().levelName);
        bestTimeText.text = TimeUtils.GetTimeString(bestTime);
    }

    private void SetupButtons()
    {
        retryButton.Init(Retry);
        menuButton.Init(GoToMainMenu);
        nextButton.Init(NextLevel);
    }

    public void Retry()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        playerProgress.ResetPlayer();
        gameObject.SetActive(false);
    }

    public void NextLevel()
    {
        GameManager.Instance.ReplayFileLocation = string.Empty;
        Level currentLevel = GameManager.GetCurrentLevel();

        Debug.Log($"Win Menu: Going to next level, just finished: {currentLevel.levelName}");

        if (currentLevel.workshopFilePath != string.Empty || currentLevel.levelEditorLevelDataPath != string.Empty)
        {
            Time.timeScale = 1;
            GameManager.LoadScene(PlayerConstants.MainMenuSceneIndex);
        }
        else
        {
            Time.timeScale = 1;

            // Load credits scene
            if (currentLevel.levelBuildIndex >= GameManager.Instance.levelDataContainer.levels.Length)
            {
                Cursor.visible = true;
                GameManager.LoadScene(PlayerConstants.CreditsSceneIndex);
            }
            else
            {
                Cursor.visible = false;
                GameManager.NextLevel();
                GameManager.LoadScene(currentLevel.levelBuildIndex + 1);
            }
            gameObject.SetActive(false);
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        Cursor.visible = true;
        GameManager.LoadScene(PlayerConstants.MainMenuSceneIndex);
        gameObject.SetActive(false);
    }
}
