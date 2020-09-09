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
        else if (level.levelName == "workshop")
        {
            tab = LevelSelectionTab.Workshop;
        }
        else
        {
            tab = LevelSelectionTab.Hop;
        }

        Button button = GetComponentInChildren<Button>();
        button.name = level.levelName;
        button.onClick.AddListener(() => OnClickLevel(level));
    }

    public void OnClickLevel(Level level)
    {
        //levelPreview.Init(level);
        GameManager.Instance.currentLevelBuildIndex = level.levelBuildIndex;
        SceneManager.LoadScene(level.levelBuildIndex);
    }
}
