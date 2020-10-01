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
            levelTime.text = "Best Time: N/A";
        }


        if (level.workshopFilePath != string.Empty)
        {
            tab = LevelSelectionTab.Workshop;
        }
        else if (level.levelName.ToLower().Contains("portal"))
        {
            tab = LevelSelectionTab.Portal;
        }
        else
        {
            tab = LevelSelectionTab.Hop;
        }
    }
}
