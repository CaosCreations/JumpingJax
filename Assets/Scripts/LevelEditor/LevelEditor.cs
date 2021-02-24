﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEditor : MonoBehaviour
{
    [Header("Set in Editor")]
    public GameObject levelEditorButtonPrefab;
    public Transform levelButtonParent;

    public SecondaryButton backButton;
    public SecondaryButton deleteButton;
    public SecondaryButton newButton;
    public PrimaryButton loadButton;
    public PrimaryButton publishButton;

    public Text errorText;


    [Header("Set at Runtime")]
    public MainMenuController menuController;
    public LevelEditorInfo levelEditorInfo;

    private List<Level> playerCreatedLevels;
    private List<LevelEditorButton> levelEditorButtons;

    public LevelEditorButton selectedLevel;

    private string levelEditorFolderPath;

    void Start()
    {
        levelEditorFolderPath = FilePathUtil.GetLevelEditorDataFolder();
        menuController = GetComponentInParent<MainMenuController>();
        levelEditorInfo = GetComponentInChildren<LevelEditorInfo>();
        levelEditorButtons = new List<LevelEditorButton>();
        MoveOldLevels();
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

    private void MoveOldLevels()
    {
        //check Application.persistentDataPath if old LE levels exist 
        List<string> levelFolders = Directory.EnumerateDirectories(Application.persistentDataPath).ToList();
        foreach (string levelfolder in levelFolders)
        {
            Debug.Log(levelfolder);
        }

        //if they do move them to LevelEditorFolderPath
    }

    private List<Level> GetPlayerCreatedLevels()
    {
        List<Level> toReturn = new List<Level>();

        List<string> levelFolders = Directory.EnumerateDirectories(levelEditorFolderPath).ToList();
        List<string> filePaths;

        foreach (string levelfolder in levelFolders)
        {
            filePaths = Directory.EnumerateFiles(levelfolder, "*.level").ToList();
            foreach (string filePath in filePaths)
            {
                string fileData = File.ReadAllText(filePath);
                Level newLevel = ScriptableObject.CreateInstance<Level>();
                JsonUtility.FromJsonOverwrite(fileData, newLevel);
                toReturn.Add(newLevel);
            }
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
        backButton.Init(() => menuController.Init());
        deleteButton.Init(() => DeleteLevel());
        newButton.Init(() => NewLevel());
        loadButton.Init(() => LoadLevel());
        publishButton.Init(() => Publish());
    }

    private void DeleteLevel()
    {
        if(selectedLevel == null)
        {
            return;
        }

        try
        {
            Directory.Delete(selectedLevel.level.levelEditorLevelDataFolder, true);
        }
        catch(Exception e)
        {
            Debug.LogError($"LevelEditor.DeleteLevel(): couldn't delete level {e.Message}\n{e.StackTrace}");
        }

        levelEditorButtons.Remove(selectedLevel);
        Destroy(selectedLevel.level);
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
        newLevel.levelName = "";
        newLevel.description = "";
        newLevel.levelBuildIndex = PlayerConstants.LevelEditorSceneIndex;
        newLevel.gravityMultiplier = 1;

        string currentTime = DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss-FFF");
        newLevel.levelEditorLevelDataFolder = Path.Combine(levelEditorFolderPath, currentTime);
        string scriptableObjectPath = Path.Combine(newLevel.levelEditorLevelDataFolder, currentTime + ".level");
        newLevel.levelEditorScriptableObjectPath = scriptableObjectPath;

        newLevel.levelEditorFolder = levelEditorFolderPath;

        string levelDataPath = Path.Combine(newLevel.levelEditorLevelDataFolder, currentTime + ".json");
        newLevel.levelEditorLevelDataPath = levelDataPath;

        if (!Directory.Exists(levelEditorFolderPath))
        {
            try
            {
                Directory.CreateDirectory(levelEditorFolderPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"LevelEditor.NewLevel(): couldn't create directory {e.Message}\n{e.StackTrace}");
            }
        }

        if (!Directory.Exists(newLevel.levelEditorLevelDataFolder))
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(levelEditorFolderPath, currentTime));
            }
            catch (Exception e)
            {
                Debug.LogError($"LevelEditor.NewLevel(): couldn't create directory {e.Message}\n{e.StackTrace}");
            }
        }

        try
        {
            File.WriteAllText(newLevel.levelEditorScriptableObjectPath, JsonUtility.ToJson(newLevel));
        }
        catch (Exception e)
        {
            Debug.LogError($"LevelEditor.NewLevel(): couldn't write LEVEL file {e.Message}\n{e.StackTrace}");
        }

        try
        {
            File.WriteAllText(newLevel.levelEditorLevelDataPath, JsonUtility.ToJson(""));
        }
        catch (Exception e)
        {
            Debug.LogError($"LevelEditor.NewLevel(): couldn't write LEVEL DATA file {e.Message}\n{e.StackTrace}");
        }

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

    private async void Publish()
    {
        publishButton.SetDisabled();

        if (!CanPublish()) 
        {
            publishButton.ClearDisabled();
            return;
        } //publish check has no errors then we can publish

        if (selectedLevel.level.fileId == 0)
        {
            Steamworks.Data.PublishedFileId fileId = await WorkshopManager.PublishItem(selectedLevel.level);
            if (fileId.Value != 0)
            {
                selectedLevel.level.fileId = fileId;
                try
                {
                    File.WriteAllText(selectedLevel.level.levelEditorScriptableObjectPath, JsonUtility.ToJson(selectedLevel.level));

                }
                catch (Exception e)
                {
                    Debug.LogError($"LevelEditor.Publish(): couldn't write published LEVEL file {e.Message}\n{e.StackTrace}");
                }
            }
        }
        else
        {
            await WorkshopManager.UpdateItem(selectedLevel.level);
        }

        publishButton.ClearDisabled();
    }

    private void LevelButtonClicked(LevelEditorButton button)
    {
        levelEditorInfo.Init(button.level);
        selectedLevel = button;
        errorText.text = string.Empty;
        if (!CanPublish())
        {
            publishButton.SetDisabled();
        }
        else
        {
            publishButton.ClearDisabled();
        }
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

    private bool CanPublish()
    {
        bool isValid = true;

        string levelData = File.ReadAllText(selectedLevel.level.levelEditorLevelDataPath);
        LevelEditorLevel level = new LevelEditorLevel();
        JsonUtility.FromJsonOverwrite(levelData, level);

        errorText.text = "";
        //if theres no start checkpoint
        if (level.levelObjects.FirstOrDefault(levelObject => levelObject.objectType == ObjectType.FirstCheckpoint).objectType == ObjectType.FloatingPlatform)
        {
            errorText.text += "Level must have a starting checkpoint" + '\n';
            isValid = false;
        }

        //if theres no end checkpoint
        if (level.levelObjects.FirstOrDefault(levelObject => levelObject.objectType == ObjectType.FinalCheckpoint).objectType == ObjectType.FloatingPlatform)
        {
            errorText.text += "Level must have an end checkpoint" + '\n';
            isValid = false;
        }

        //if theres no level name
        if (string.IsNullOrEmpty(selectedLevel.text.text))
        {
            errorText.text += "Level must have a name" + '\n';
            isValid = false;
        }

        //if theres no level description
        if (string.IsNullOrEmpty(selectedLevel.level.description))
        {
            errorText.text += "Level must have a description" + '\n';
            isValid = false;
        }

        return isValid;
    }
}
