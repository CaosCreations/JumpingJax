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
        GameManager.Instance.currentLevel = levelToPreview;
        SceneManager.LoadScene(levelToPreview.levelBuildIndex);
    }

    public async void Init(Level level)
    {
        playButton.gameObject.SetActive(true);
        previewImage.gameObject.SetActive(true);
        leaderboard.gameObject.SetActive(true);
        levelToPreview = level;
        previewImage.sprite = level.previewSprite;
        PopulateLeaderboard();
    }

    async void PopulateLeaderboard()
    {
        // make HTTP request from steam for leaderboard data
        if (GameManager.Instance.isSteamActive)
        {
            Steamworks.Data.LeaderboardEntry[] entries = await StatsManager.GetLevelLeaderboard(levelToPreview.levelName);
            if (entries != null && entries.Length > 0)
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

}
