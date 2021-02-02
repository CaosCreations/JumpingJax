using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FilePathUtil : MonoBehaviour
{
    public static string levelDataFolder = "LevelData";
    public static string levelEditorFolder = "LevelEditor";

    #region LevelData
    public static string GetLevelDataFilePath(string levelName)
    {
        string levelDataParentFolder = Path.Combine(Application.persistentDataPath, levelDataFolder);
        EnsureDirectoryExists(levelDataParentFolder);
        string levelSpecificFolder = Path.Combine(levelDataParentFolder, levelName);
        EnsureDirectoryExists(levelSpecificFolder);
        string levelFilePath = Path.Combine(levelSpecificFolder, $"levelName.save");
        return levelFilePath;
    }

    public static void DeleteLevelData()
    {
        string directory = Path.Combine(Application.persistentDataPath, levelDataFolder);
        try
        {
            Directory.Delete(directory, true);
        }
        catch(Exception e)
        {
            Debug.LogError($"Could not delete directory:\n{directory}\n{e.Message}\n{e.StackTrace}");
        }

        EnsureDirectoryExists(directory);
    }
    #endregion

    #region LevelEditor
    public static void GetLevelEditorDataFolder()
    {

    }

    public static void DeleteLevelEditorData()
    {
        string directory = Path.Combine(Application.persistentDataPath, levelEditorFolder);
        try
        {
            Directory.Delete(directory, true);
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not delete directory:\n{directory}\n{e.Message}\n{e.StackTrace}");
        }

        EnsureDirectoryExists(directory);
    }
    #endregion


    public static void EnsureDirectoryExists(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Could not create directory:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
