using UnityEngine;
using System;

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
    public Collectible[] collectibles;

    [SerializeField]
    public bool isCompleted;

    [SerializeField]
    public float completionTime;

    [SerializeField]
    public Vector3[] ghostRunPositions;

    [SerializeField]
    public Vector3[] ghostRunCameraRotations; 

    [SerializeField]
    public KeysPressed[] ghostRunKeys;

    public int GetNumberOfTimeBones()
    {
        if (!isCompleted)
        {
            return 0;
        }

        if(completionTime < bone3Time)
        {
            return 3;
        }

        if (completionTime < bone2Time)
        {
            return 2;
        }

        if (completionTime < bone1Time)
        {
            return 1;
        }

        return 0;
    }
}