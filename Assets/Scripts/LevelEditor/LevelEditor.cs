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

    public bool sCheckFlag;
    public bool eCheckFlag;
    public bool nameFlag;
    public bool descFlag;
    public bool imgFlag;

    private List<Level> playerCreatedLevels;
    private List<LevelEditorButton> levelEditorButtons;

    public LevelEditorButton selectedLevel;

    private string levelEditorFolderPath;

    void Start()
    {
        levelEditorFolderPath = Path.Combine(Application.persistentDataPath, "levelEditor");
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

        if (!Directory.Exists(levelEditorFolderPath))
        {
            try
            {
                Directory.CreateDirectory(levelEditorFolderPath);
            }catch(Exception e)
            {
                Debug.LogError($"LevelEditor.GetPlayerCreatedLevels(): couldn't create levelEditorFolderPath {e.Message}\n{e.StackTrace}");
            }
        }
        List<string> filePaths = Directory.EnumerateFiles(levelEditorFolderPath, "*.level").ToList();
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

        try
        {
            File.Delete(selectedLevel.level.levelEditorScriptableObjectPath);

        }catch(Exception e)
        {
            Debug.LogError($"LevelEditor.DeleteLevel(): couldn't delete level {e.Message}\n{e.StackTrace}");
        }

        //Directory.Delete(selectedLevel.level.levelEditorFolder);
        //RefreshEditorProjectWindow();
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

        string currentTime = DateTime.Now.ToString("MM-dd-yyyy_hh-mm-ss-FFF");
        string scriptableObjectPath = Path.Combine(levelEditorFolderPath, currentTime + ".level");
        newLevel.levelEditorScriptableObjectPath = scriptableObjectPath;

        newLevel.levelEditorFolder = levelEditorFolderPath;

        string levelDataPath = Path.Combine(levelEditorFolderPath, currentTime + ".json");
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
        publishButton.interactable = false;
        Debug.Log("Process has begun");

        if (!publishCheck()) 
        { 
            // TODO some sort of UI Validation, showing the user there's an issue
            // return;
        } //after the publish check is all good then we publish

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

        publishButton.interactable = true;
        Debug.Log("Process has ended");
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

    private bool publishCheck()
    {
        //if theres no start checkpoint
        if (sCheckFlag)
        {
            Debug.Log("Level must have a starting checkpoint");
        }
        //if theres no end checkpoint
        if (eCheckFlag)
        {
            Debug.Log("Level must have an end checkpoint");
        }
        //if theres no level name
        if (nameFlag)
        {
            Debug.Log("Level must have a name");
        }
        //if theres no level description
        if (descFlag)
        {
            Debug.Log("Level must have a description");
        }
        //if theres no picture (I don't know about this one)
        if (imgFlag)
        {
            Debug.Log("Level must have an image");
        }

        if (!sCheckFlag && !eCheckFlag && !nameFlag && !descFlag && !imgFlag) { return true; }
        else { return false; }
    }
}
