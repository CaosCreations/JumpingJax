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
        string folderPath = Path.Combine(Application.persistentDataPath, levelName);

        if (!Directory.Exists(folderPath))
        {
            try
            {
                Directory.CreateDirectory(folderPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
        }

        string filePath = Path.Combine(folderPath, $"{levelName}.save");
        string fileContents = JsonUtility.ToJson(levelSaveData);
        try
        {
            File.WriteAllText(filePath, fileContents);
        }
        catch (Exception e)
        {
            Debug.LogError($"{e.Message}\n{e.StackTrace}");
        }
        Debug.Log("Finished Saving level");
    }

    public void Load()
    {
        Debug.Log("loading level");
        string folderPath = Path.Combine(Application.persistentDataPath, levelName);
        string filePath = Path.Combine(folderPath, $"{levelName}.save");
        if (File.Exists(filePath))
        {
            try
            {
                string fileContents = File.ReadAllText(filePath);
                levelSaveData = JsonUtility.FromJson<PersistentLevelDataModel>(fileContents);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
        }
        Debug.Log("Finished loading level");
    }

    public void Clear()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, levelName);
        string filePath = Path.Combine(folderPath, $"/{levelName}.save");
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
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