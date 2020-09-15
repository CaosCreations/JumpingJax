using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LevelDataContainer levelDataContainer;
    public static uint AppId = 1315100;
    public static int workshopLevelIndex = -1; // assumes workshop is the second to last scene (before credits)

    public Level currentLevel;
    public float currentCompletionTime;
    public bool didWinCurrentLevel;
    public bool isSteamActive;

    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        if (GameManager.Instance == null)
        {
            GameManager.Instance = this;
        }
        else if (GameManager.Instance == this)
        {
            Destroy(GameManager.Instance.gameObject);
            GameManager.Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        if (GameManager.Instance.isSteamActive == true)
        {
            StartSteam();
            Init();
        }
    }

    private void StartSteam()
    {
        workshopLevelIndex = SceneManager.sceneCountInBuildSettings - 2;
        try
        {
            if (!SteamClient.IsValid)
            {
                SteamClient.Init(AppId, true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could not connect to steam " + e.Message);
        }
    }

    void Update()
    {
        if (!didWinCurrentLevel)
        {
            currentCompletionTime += Time.deltaTime;
        }
    }

    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }

    private void OnEnable()
    {
        Debug.Log("on enable");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnLevelWasLoaded(int level)
    {
        Debug.Log("level loaded");
    }

    public static void LoadScene(int buildIndex)
    {
        if(buildIndex != PlayerConstants.BuildSceneIndex)
        {
            Instance.currentLevel = Instance.levelDataContainer.levels[buildIndex - 1];
        }

        AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(buildIndex);
        LoadingScreenManager.Instance.Show(sceneLoadOperation);
    }

    public static void LoadScene(Level workshopLevel)
    {
        Instance.currentLevel = workshopLevel;
        string sceneAssetPath = AssetBundleManager.LoadSceneFromBundle(workshopLevel.filePath);
        AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(sceneAssetPath);
        LoadingScreenManager.Instance.Show(sceneLoadOperation);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Init();
    }

    private void Init()
    {
        Scene scene = SceneManager.GetActiveScene();
        Debug.Log($"Scene loaded: {scene.name} with index {scene.buildIndex}");
        currentCompletionTime = 0;
        didWinCurrentLevel = false;

        if (scene.buildIndex == PlayerConstants.BuildSceneIndex)
        {
            AssetBundle.UnloadAllAssetBundles(true);
            return;
        }

        // Set our current level if we are loading from that scene, and not the menu
        if (Instance.currentLevel == null && scene.buildIndex > 0)
        {
            Instance.currentLevel = Instance.levelDataContainer.levels[scene.buildIndex - 1];
        }

        // Set up the workshop level to have the right number of checkpoints, since it isn't loaded on the scene
        if (Instance.currentLevel.filePath != string.Empty)
        {
            Instance.currentLevel.numberOfCheckpoints = GameObject.FindObjectsOfType<Checkpoint>().Length;
        }
    }

    public static Level GetCurrentLevel()
    {
        if(Instance.currentLevel == null)
        {
            return new Level();
        }

        return Instance.currentLevel;
    }

    public static void NextLevel()
    {
        // NOTE: The level build index starts at 1 (because main menu is at index 0)
        Instance.currentLevel = Instance.levelDataContainer.levels[Instance.currentLevel.levelBuildIndex];
    }

    public static void RestartLevel()
    {
        Instance.currentCompletionTime = 0;
        Instance.didWinCurrentLevel = false;
    }

    public static void FinishedLevel()
    {
        Instance.didWinCurrentLevel = true;
        float completionTime = Instance.currentCompletionTime;
        Level levelToUpdate = GetCurrentLevel();

        levelToUpdate.isCompleted = true;

        if (completionTime < levelToUpdate.completionTime || levelToUpdate.completionTime == 0)
        {
            levelToUpdate.completionTime = completionTime;

            if (GameManager.Instance.isSteamActive == true)
            {
                StatsManager.SaveLevelCompletion(levelToUpdate);
            }
        }
    }
}
