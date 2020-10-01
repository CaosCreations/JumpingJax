using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class WorkshopSceneLoader : MonoBehaviour
{
    public LevelPrefabContainer levelPrefabContainer;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void LoadSceneData()
    {
        Level currentLevel = GameManager.GetCurrentLevel();


        if (currentLevel.workshopFilePath == string.Empty)
        {
            Debug.Log($"Trying to load level: {currentLevel.levelName} but it has no path");
            return;
        }

        DirectoryInfo fileInfo = new DirectoryInfo(currentLevel.workshopFilePath);
        string scenePath = fileInfo.EnumerateFiles().First().FullName;

        if (!File.Exists(scenePath))
        {
            Debug.Log($"Trying to load level: {currentLevel.levelName} from {scenePath} but the save file doesn't exist");
            return;
        }

        string jsonData = File.ReadAllText(scenePath);
        LevelEditorLevel levelToLoad = JsonUtility.FromJson<LevelEditorLevel>(jsonData);

        foreach (ObjectData objectData in levelToLoad.levelObjects)
        {
            CreateObject(objectData);
        }
    }

    private void CreateObject(ObjectData objectData)
    {
        LevelPrefab prefabOfType = levelPrefabContainer.levelPrefabs.ToList().Where(x => x.objectType == objectData.objectType).FirstOrDefault();
        if (prefabOfType == null)
        {
            return;
        }

        GameObject newObject = Instantiate(prefabOfType.prefab);

        newObject.transform.position = objectData.position;
        newObject.transform.rotation = Quaternion.Euler(objectData.rotation);
        newObject.transform.localScale = objectData.scale;
    }
}
