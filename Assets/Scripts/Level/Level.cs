using UnityEngine;
using System;
using System.IO;

[CreateAssetMenu(fileName = "Level X", menuName = "ScriptableObjects/level")]
public class Level : ScriptableObject
{
    [Header("Set in Editor")]
    [SerializeField]
    public int levelBuildIndex;

    [SerializeField]
    public string levelName;

    [SerializeField]
    public float gravityMultiplier;

    [SerializeField]
    public string[] tutorialTexts;

    [SerializeField]
    public Sprite previewSprite;

    [SerializeField]
    public float bone1Time;

    [SerializeField]
    public float bone2Time;

    [SerializeField]
    public float bone3Time;

    [Header("Workshop data")]
    [SerializeField]
    public string description;

    [SerializeField]
    public string workshopFilePath;

    [SerializeField]
    public string levelEditorFolder;

    [SerializeField]
    public string levelEditorScriptableObjectPath;

    [SerializeField]
    public string levelEditorScenePath;

    [SerializeField]
    public string previewImagePath;

    [SerializeField]
    public ulong fileId;

    [Header("Set in Game")]
    [SerializeField]
    public PersistentLevelDataModel levelSaveData;

    public int GetNumberOfTimeBones()
    {
        if (!levelSaveData.isCompleted)
        {
            return 0;
        }

        if(levelSaveData.completionTime < bone3Time)
        {
            return 3;
        }

        if (levelSaveData.completionTime < bone2Time)
        {
            return 2;
        }

        if (levelSaveData.completionTime < bone1Time)
        {
            return 1;
        }

        return 0;
    }

    public void Save()
    {
        Debug.Log($"Saving level {levelName}");
        string filePath = Application.persistentDataPath + $"/{levelName}.save";
        string fileContents = JsonUtility.ToJson(levelSaveData);
        File.WriteAllText(filePath, fileContents);
    }

    public void Load()
    {
        Debug.Log($"Loading level {levelName}");
        string filePath = Application.persistentDataPath + $"/{levelName}.save";
        if (File.Exists(filePath))
        {
            string fileContents = File.ReadAllText(filePath);
            levelSaveData = JsonUtility.FromJson<PersistentLevelDataModel>(fileContents);
        }
    }

    public void Clear()
    {
        string filePath = Application.persistentDataPath + $"/{levelName}.save";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    public async void InitFromWorkshopItem(Steamworks.Ugc.Item item)
    {
        if(levelSaveData == null)
        {
            levelSaveData = new PersistentLevelDataModel();
        }

        levelBuildIndex = GameManager.workshopLevelIndex;
        workshopFilePath = item.Directory;
        levelName = item.Title;
        fileId = item.Id.Value;
        gravityMultiplier = 1;

        levelSaveData.completionTime = await StatsManager.GetLevelCompletionTime(item.Title);
        if (levelSaveData.completionTime > 0)
        {
            levelSaveData.isCompleted = true;
        }

        if (item.PreviewImageUrl != null && item.PreviewImageUrl != string.Empty)
        {
            Texture2D texture = await SteamCacheManager.GetUGCPreviewImage(item.PreviewImageUrl);
            previewSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}