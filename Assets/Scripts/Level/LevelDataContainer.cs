using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TotalTimeType
{
    Hop, Portal, All
}

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/levelData")]
public class LevelDataContainer : ScriptableObject
{
    [SerializeField]
    public Level[] levels;

    public string GetTotalTime(TotalTimeType type)
    {
        float totalTime = 0;

        if (!CheckLevelCompletion(type))
        {
            return "Not all levels completed";
        }

        foreach(Level level in levels)
        {
            if (level.levelName.ToLower().Contains("portal"))
            {
                if(type == TotalTimeType.Portal || type == TotalTimeType.All)
                {
                    totalTime += level.levelSaveData.completionTime;
                }
            }
            else
            {
                if (type == TotalTimeType.Hop || type == TotalTimeType.All)
                {
                    totalTime += level.levelSaveData.completionTime;
                }
            }
        }

        return TimeUtils.GetTimeString(totalTime);
    }

    public bool CheckLevelCompletion(TotalTimeType type)
    {
        foreach (Level level in levels)
        {
            if (level.levelName.ToLower().Contains("portal"))
            {
                if (type == TotalTimeType.Portal || type == TotalTimeType.All)
                {
                    if (!level.levelSaveData.isCompleted)
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (type == TotalTimeType.Hop || type == TotalTimeType.All)
                {
                    if (!level.levelSaveData.isCompleted)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }
}