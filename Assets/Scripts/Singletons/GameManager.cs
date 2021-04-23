using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public bool hasLeftFirstCheckpoint;
    public bool shouldUseSteam;
    public bool isLoadingScene;

    private string replayFileLocation;
    public string ReplayFileLocation
    {
        get
        {
            return replayFileLocation;
        }
        set
        {
            AsyncTaskReporter.Instance.ghostDownloadRunning = false;
            replayFileLocation = value;
        }
    }

    public Steamworks.Data.PublishedFileId CurrentReplayFileId;

    private bool shiftPressed;
    private bool tabPressed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (GameManager.Instance.shouldUseSteam == true)
        {
            StartSteam();
        }

        Init();
        LoadLevelData();
        //
        // Log unhandled exceptions created in Async Tasks so we know when something has gone wrong
        //
        TaskScheduler.UnobservedTaskException += (_, e) => { Debug.LogError($"{e.Exception}\n{e.Exception.Message}\n{e.Exception.StackTrace}"); };
    }

    private void StartSteam()
    {
        workshopLevelIndex = SceneUtils.GetFixedSceneIndex(FixedScene.LevelEditor);
        try
        {
            if (!SteamClient.IsValid)
            {
                SteamClient.Init(AppId, false);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Could not connect to steam " + e.Message);
        }
    }

    private void LoadLevelData()
    {
        Debug.Log("Loading all level data");
        foreach(Level level in levelDataContainer.levels)
        {
            level.Load();
        }
    }

    void Update()
    {
        SteamClient.RunCallbacks();

        if (!didWinCurrentLevel && hasLeftFirstCheckpoint)
        {
            currentCompletionTime += Time.deltaTime;
        }

        CheckSteamOverlay();
    }

    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public static void LoadScene(int buildIndex)
    {
        Debug.Log($"GameManager.LoadScene(): loading scene with index {buildIndex}");
        Instance.isLoadingScene = true;

        if (buildIndex == SceneUtils.GetFixedSceneIndex(FixedScene.Credits))
        {
            Instance.currentLevel = ScriptableObject.CreateInstance<Level>();
            Instance.currentLevel.levelName = "Credits";
        }
        else if (buildIndex == SceneUtils.GetFixedSceneIndex(FixedScene.MainMenu))
        {
            Instance.currentLevel = ScriptableObject.CreateInstance<Level>();
            Instance.currentLevel.levelName = "Main Menu";
        }
        else if (buildIndex == SceneUtils.GetFixedSceneIndex(FixedScene.LevelEditor))
        {
            Instance.currentLevel = ScriptableObject.CreateInstance<Level>();
            Instance.currentLevel.levelName = "Level Editor";
        }
        else
        {
            Instance.currentLevel = Instance.levelDataContainer.levels[buildIndex - 1];
        }

        AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(buildIndex);
        LoadingScreenManager.Instance.Show(sceneLoadOperation);
    }

    public static void LoadScene(Level workshopLevel)
    {
        Debug.Log($"GameManager.LoadScene(): loading workshop scene with index {workshopLevel.levelName}");
        Instance.currentLevel = workshopLevel;
        int levelEditorSceneIndex = SceneUtils.GetFixedSceneIndex(FixedScene.LevelEditor);
        AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(levelEditorSceneIndex);
        LoadingScreenManager.Instance.Show(sceneLoadOperation);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Init();
    }

    private void Init()
    {
        Scene scene = SceneManager.GetActiveScene();
        Debug.Log($"GameManager.Init(): Scene loaded: {scene.name} with index {scene.buildIndex}");
        currentCompletionTime = 0;
        didWinCurrentLevel = false;
        hasLeftFirstCheckpoint = false;

        if (!SceneUtils.SceneUsesReplay(scene.buildIndex))
        {
            replayFileLocation = string.Empty;
            return;
        }

        // Set our current level if we are loading from that scene in the editor, and not the menu
        if (Instance.currentLevel == null && SceneUtils.SceneIsLevel(scene.buildIndex))
        {
            Instance.currentLevel = Instance.levelDataContainer.levels[scene.buildIndex - 1];
        }

        // Set up the workshop level to have the right number of checkpoints, since it isn't loaded on the scene
        if (Instance.currentLevel.workshopFilePath != string.Empty || Instance.currentLevel.levelEditorScriptableObjectPath != string.Empty)
        {
            LevelEditorHUD levelEditorHUD = FindObjectOfType<LevelEditorHUD>();
            levelEditorHUD.LoadSceneData();
        }
    }

    public static Level GetCurrentLevel()
    {
        if(Instance == null)
        {
            return ScriptableObject.CreateInstance<Level>();
        }

        if(Instance.currentLevel == null)
        {
            return ScriptableObject.CreateInstance<Level>();
        }

        return Instance.currentLevel;
    }

    public static bool GetDidFinishLevel()
    {
        if (Instance == null)
        {
            return false;
        }

        if (Instance.currentLevel == null)
        {
            return false;
        }

        return Instance.didWinCurrentLevel;
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
        Instance.hasLeftFirstCheckpoint = false;
    }

    public static void LeftFirstCheckpoint()
    {
        Instance.hasLeftFirstCheckpoint = true;
    }

    public static async void FinishedLevel()
    {
        Instance.didWinCurrentLevel = true;
        float completionTime = Instance.currentCompletionTime;
        Level levelToUpdate = GetCurrentLevel();

        Debug.Log($"GameManager.FinishedLevel(), completed {levelToUpdate.levelName}");

        if (completionTime < levelToUpdate.levelSaveData.completionTime || !levelToUpdate.levelSaveData.isCompleted)
        {
            levelToUpdate.levelSaveData.isCompleted = true;

            levelToUpdate.levelSaveData.completionTime = completionTime;
            levelToUpdate.Save();

            if (ShouldUseSteam())
            {
                Debug.Log($"saving level {GetCurrentLevel().levelName} completion to Steam");
                await StatsManager.SaveLevelCompletion(levelToUpdate);
                Debug.Log("finished saving level completion to Steam");
            }
        }
    }

    public static bool ShouldUseSteam()
    {
        return GameManager.Instance.shouldUseSteam == true && SteamClient.IsValid;
    }

    public void CheckSteamOverlay()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            shiftPressed = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            shiftPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tabPressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tabPressed = false;
        }

        if(shiftPressed && tabPressed)
        {
            SteamFriends.OpenOverlay("friends");
        }
    }
}
