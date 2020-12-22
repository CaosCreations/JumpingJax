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

        if (InputManager.GetKeyDown(PlayerConstants.ResetLevel))
        { 
            Retry();
        }
    }

    private void OnEnable()
    {
        levelText.text = "You found Jax on: " + GameManager.GetCurrentLevel().levelName;
        completionTimeText.text = TimeUtils.GetTimeString(GameManager.Instance.currentCompletionTime);
        bestTimeText.text = TimeUtils.GetTimeString(GameManager.GetCurrentLevel().levelSaveData.completionTime);
    }

    private void SetupButtons()
    {
        retryButton.Init(Retry);
        menuButton.Init(GoToMainMenu);
        nextButton.Init(NextLevel);
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
        GameManager.Instance.replayFileLocation = string.Empty;
        Level currentLevel = GameManager.GetCurrentLevel();

        if (currentLevel.workshopFilePath != string.Empty || currentLevel.levelEditorScenePath != string.Empty)
        {
            Time.timeScale = 1;
            GameManager.LoadScene(PlayerConstants.MainMenuSceneIndex);
            
        }
        else
        {
            gameObject.SetActive(false);
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
        }
    }

    public void GoToMainMenu()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.visible = true;
        GameManager.LoadScene(PlayerConstants.MainMenuSceneIndex);
    }
}
