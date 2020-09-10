using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;
using System.Runtime.InteropServices;

public class LevelButton : MonoBehaviour
{
    public Text levelName;
    public Text levelTime;
    public Button button;
    public LevelSelectionTab tab;

    public void SetupButton(Level level)
    {
        levelName.text = level.levelName;

        if (level.isCompleted)
        {
            TimeSpan time = TimeSpan.FromSeconds(level.completionTime);
            String timeString = time.ToString(PlayerConstants.levelCompletionTimeFormat);
            levelTime.text = "Best Time: " + timeString;
        }
        else
        {
            levelTime.text = "Not Completed";
        }

        if (level.isPortalLevel)
        {
            tab = LevelSelectionTab.Portal;
        } 
        else if (level.filePath != string.Empty)
        {
            tab = LevelSelectionTab.Workshop;
        }
        else
        {
            tab = LevelSelectionTab.Hop;
        }

        button = GetComponentInChildren<Button>();
        button.name = level.levelName;
    }
}
