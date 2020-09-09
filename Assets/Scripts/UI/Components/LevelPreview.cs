using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPreview : MonoBehaviour
{
    public GameObject leaderboardItemPrefab;

    public Button playButton;
    public Image previewImage;
    public Transform leaderboard;
    public Transform scrollViewContent;


    private Level levelToPreview;

    private void Start()
    {
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(Play);
        playButton.gameObject.SetActive(false);
        previewImage.gameObject.SetActive(false);
        leaderboard.gameObject.SetActive(false);
    }

    void Play()
    {
        GameManager.Instance.currentLevelBuildIndex = levelToPreview.levelBuildIndex;
        SceneManager.LoadScene(levelToPreview.levelBuildIndex);
    }

    public void Init(Level level)
    {
        playButton.gameObject.SetActive(true);
        previewImage.gameObject.SetActive(true);
        leaderboard.gameObject.SetActive(true);
        levelToPreview = level;
        previewImage.sprite = level.previewSprite;
        PopulateLeaderboard();
    }

    void PopulateLeaderboard()
    {
        // make HTTP request from steam for leaderboard data
        Steamworks.Data.LeaderboardEntry[] entries = StatsManager.GetLevelLeaderboard(levelToPreview.levelName).Result;
        if(entries != null && entries.Length > 0)
        {
            foreach (Steamworks.Data.LeaderboardEntry entry in entries)
            {
                GameObject entryObject = Instantiate(leaderboardItemPrefab, scrollViewContent);
                LeaderboardEntry leaderboardEntry = entryObject.GetComponent<LeaderboardEntry>();
                leaderboardEntry.Init(entry);
            }
        }
    }

}
