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

    private LeaderboardManager leaderboardManager;

    private PlayerProgress playerProgress;

    private void Awake()
    {
        playerProgress = GetComponentInParent<PlayerProgress>();
        leaderboardManager = GetComponentInChildren<LeaderboardManager>();
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
        Level currentLevel = GameManager.GetCurrentLevel();
        levelText.text = "You found Jax on: " + currentLevel.levelName;
        if(GameManager.Instance != null)
        {
            completionTimeText.text = TimeUtils.GetTimeString(GameManager.Instance.currentCompletionTime);
        }
        
        // Use local best time for now
        bestTimeText.text = TimeUtils.GetTimeString(currentLevel.levelSaveData.completionTime);

        // Then work on getting the best time from steam
        float bestTime = await StatsManager.GetLevelCompletionTime(currentLevel.levelName);
        if(bestTime != 0)
        {
            bestTimeText.text = TimeUtils.GetTimeString(bestTime);
        }

        await leaderboardManager.InitAsync(currentLevel.levelName);
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
        leaderboardManager.SetReplayLocation();

        if(leaderboardManager.replayFileId != 0)
        {
            OptionsPreferencesManager.SetLeaderboardGhostTooltip(false);
        }
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
