using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelPreview : MonoBehaviour
{
    public GameObject leaderboardItemPrefab;

    public Button playButton;
    public Image previewImage;
    public Text leaderboardNameText;
    public Transform leaderboard;
    public Transform scrollViewContent;
    public Sprite noImageSprite;

    private Level levelToPreview;
    private Dictionary<string, Steamworks.Data.LeaderboardEntry[]> leaderboardCache;

    private void Start()
    {
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(Play);
        playButton.gameObject.SetActive(false);
        previewImage.gameObject.SetActive(false);
        leaderboard.gameObject.SetActive(false);
        leaderboardNameText.gameObject.SetActive(false);
        leaderboardCache = new Dictionary<string, Steamworks.Data.LeaderboardEntry[]>();
    }

    void Play()
    {
        // If this is a workshop map
        if(levelToPreview.workshopFilePath != string.Empty) 
        {
            GameManager.LoadScene(levelToPreview);
        }
        else
        {
            GameManager.LoadScene(levelToPreview.levelBuildIndex);
        }
    }

    public async void Init(Level level)
    {
        playButton.gameObject.SetActive(true);
        previewImage.gameObject.SetActive(true);
        leaderboard.gameObject.SetActive(true);
        leaderboardNameText.gameObject.SetActive(true);
        levelToPreview = level;

        // We need this check in case the workshop dev forgot to upload a screen shot
        if(level.previewSprite == null)
        {
            previewImage.sprite = noImageSprite;
        }
        else
        {
            previewImage.sprite = level.previewSprite;
        }
        previewImage.preserveAspect = true;
        leaderboardNameText.text = $"{level.levelName} leaderboard";
        CleanScrollView();
        await PopulateLeaderboard();
    }

    private void CleanScrollView()
    {
        foreach(Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }
    }

    async Task PopulateLeaderboard()
    {
        // make HTTP request from steam for leaderboard data
        if (GameManager.Instance.isSteamActive)
        {
            Steamworks.Data.LeaderboardEntry[] entries;
            if (leaderboardCache.ContainsKey(levelToPreview.levelName))
            {
                entries = leaderboardCache[levelToPreview.levelName];
            }
            else
            {
                entries = await StatsManager.GetLevelLeaderboard(levelToPreview.levelName);
                leaderboardCache.Add(levelToPreview.levelName, entries);
            }

            if (entries != null && entries.Length > 0)
            {
                Debug.Log($"Leaderboard found, populating preview with {entries.Count()} entries");
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
