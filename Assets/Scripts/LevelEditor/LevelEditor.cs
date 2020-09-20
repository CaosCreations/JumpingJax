using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    public GameObject levelEditorButtonPrefab;
    public Transform levelButtonParent;

    public MainMenuController menuController;
    public LevelEditorInfo levelEditorInfo;

    public Button backButton;
    public Button deleteButton;
    public Button newButton;
    public Button loadButton;
    public Button publishButton;

    private List<Level> playerCreatedLevels;
    private List<LevelEditorButton> levelEditorButtons;

    public LevelEditorButton selectedLevel;

    void Start()
    {
        menuController = GetComponentInParent<MainMenuController>();
        levelEditorInfo = GetComponentInChildren<LevelEditorInfo>();
        levelEditorButtons = new List<LevelEditorButton>();
        playerCreatedLevels = GetPlayerCreatedLevels();
        CreateLevelButtons();
        SetupNavButtons();
        
        if(levelEditorButtons.Count > 0)
        {
            selectedLevel = levelEditorButtons[0];
            levelEditorInfo.Init(selectedLevel.level);
        }

        LevelEditorInfo.onLevelNameUpdated += UpdateLevelNames;
    }

    private List<Level> GetPlayerCreatedLevels()
    {
        List<Level> toReturn = new List<Level>();

        List<string> filePaths = Directory.EnumerateFiles(Application.persistentDataPath, "*.level").ToList();
        foreach (string filePath in filePaths)
        {
            string fileData = File.ReadAllText(filePath);
            Level newLevel = ScriptableObject.CreateInstance<Level>();
            JsonUtility.FromJsonOverwrite(fileData, newLevel);
            toReturn.Add(newLevel);
        }
        return toReturn;
    }

    private void CreateLevelButtons()
    {
        foreach(Level level in playerCreatedLevels)
        {
            GameObject newLevelButton = Instantiate(levelEditorButtonPrefab, levelButtonParent);
            LevelEditorButton levelEditorButton = newLevelButton.GetComponent<LevelEditorButton>();
            levelEditorButton.Init(level);
            levelEditorButton.button.onClick.RemoveAllListeners();
            levelEditorButton.button.onClick.AddListener(() => LevelButtonClicked(levelEditorButton));
            levelEditorButtons.Add(levelEditorButton);
        }
    }

    private void SetupNavButtons()
    {
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => menuController.Init());

        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(() => DeleteLevel());

        newButton.onClick.RemoveAllListeners();
        newButton.onClick.AddListener(() => NewLevel());

        loadButton.onClick.RemoveAllListeners();
        loadButton.onClick.AddListener(() => LoadLevel());

        publishButton.onClick.RemoveAllListeners();
        publishButton.onClick.AddListener(() => Publish());
    }

    private void DeleteLevel()
    {
        if(selectedLevel == null)
        {
            return;
        }

        File.Delete(selectedLevel.level.levelEditorScriptableObjectPath);
        RefreshEditorProjectWindow();
        levelEditorButtons.Remove(selectedLevel);
        ScriptableObject.Destroy(selectedLevel.level);
        Destroy(selectedLevel.gameObject);

        if(levelEditorButtons.Count > 0)
        {
            selectedLevel = levelEditorButtons[0];
            levelEditorInfo.Init(selectedLevel.level);
        }
        else
        {
            selectedLevel = null;
            levelEditorInfo.Init(null);
        }
    }

    private void NewLevel()
    {
        Level newLevel = ScriptableObject.CreateInstance<Level>();
        newLevel.levelName = "new level";
        newLevel.description = "new level description";
        newLevel.levelBuildIndex = PlayerConstants.LevelEditorSceneIndex;
        newLevel.gravityMultiplier = 1;

        string folder = Path.Combine(Application.persistentDataPath, DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss-FFF"));
        string scriptableObjectPath = Path.Combine(Application.persistentDataPath, "scriptableObject.level");
        string levelDataPath = Path.Combine(folder, "levelData.json");
        newLevel.levelEditorScriptableObjectPath = scriptableObjectPath;
        newLevel.levelEditorScenePath = levelDataPath;
        newLevel.levelEditorFolder = folder;

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(newLevel.levelEditorScriptableObjectPath, JsonUtility.ToJson(newLevel));
        File.WriteAllText(newLevel.levelEditorScenePath, JsonUtility.ToJson(""));

        GameObject newLevelButton = Instantiate(levelEditorButtonPrefab, levelButtonParent);
        LevelEditorButton levelEditorButton = newLevelButton.GetComponent<LevelEditorButton>();

        levelEditorButton.Init(newLevel);

        levelEditorButton.button.onClick.RemoveAllListeners();
        levelEditorButton.button.onClick.AddListener(() => LevelButtonClicked(levelEditorButton));

        levelEditorButtons.Add(levelEditorButton);

        levelEditorInfo.Init(newLevel);
        selectedLevel = levelEditorButton;
    }

    private void LoadLevel()
    {
        if(selectedLevel != null)
        {
            GameManager.LoadScene(selectedLevel.level);
        }
    }

    private void Publish()
    {
        WorkshopManager.PublishItem(selectedLevel.level);
    }

    private void LevelButtonClicked(LevelEditorButton button)
    {
        levelEditorInfo.Init(button.level);
        selectedLevel = button;
    }

    // When the currently selected level has its name updated
    // we need to rename the button
    private void UpdateLevelNames(string name)
    {
        // If the player hits "load" before clicking off of the level, the update happens during scene transition
        // so this no longer exists
        if(selectedLevel == null)
        {
            return;
        }
        selectedLevel.text.text = name;
    }

    // After files are added or deleted, the project window needs refreshed
    void RefreshEditorProjectWindow()
    {
        #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
        #endif
    }
}
