using System;
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
    public Transform leaderboardParent;
    public Transform myLeaderboardParent;

    public Button playButton;
    public Button backButton;

    public Image previewImage;
    public Sprite noImageSprite;

    public Text levelNameText;
    public Text bestTimeText;
    public Text rankText;

    public Image singleBoneImage;
    public Image doubleBoneImage;
    public Image tripleBoneImage;

    public Sprite boneEmptySprite;
    public Sprite boneFilledSprite;

    public Text singleBoneText;
    public Text doubleBoneText;
    public Text tripleBoneText;

    private Level levelToPreview;
    private Dictionary<string, Steamworks.Data.LeaderboardEntry[]> leaderboardCache;

    private void Start()
    {
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(Play);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(Back);
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

        gameObject.SetActive(false);
    }

    void Back()
    {
        gameObject.SetActive(false);
    }

    public async void Init(Level level)
    {
        levelToPreview = level;

        UpdateDetailPane();

        CleanScrollView();
        await PopulateLeaderboard();
        await PopulateMyStats();
    }

    private void UpdateDetailPane()
    {
        levelNameText.text = levelToPreview.levelName;

        // We need this check in case the workshop dev forgot to upload a screen shot
        if (levelToPreview.previewSprite == null)
        {
            previewImage.sprite = noImageSprite;
        }
        else
        {
            previewImage.sprite = levelToPreview.previewSprite;
        }
        previewImage.preserveAspect = true;


        if (levelToPreview.isCompleted)
        {
            TimeSpan time = TimeSpan.FromSeconds(levelToPreview.completionTime);
            string timeString = time.ToString(PlayerConstants.levelCompletionTimeFormat);
            bestTimeText.text = timeString;
        }
        else
        {
            bestTimeText.text = "N/A";
        }

        SetBoneImages();
        singleBoneText.text = TimeSpan.FromSeconds(levelToPreview.bone1Time).ToString(PlayerConstants.levelCompletionTimeFormat);
        doubleBoneText.text = TimeSpan.FromSeconds(levelToPreview.bone2Time).ToString(PlayerConstants.levelCompletionTimeFormat);
        tripleBoneText.text = TimeSpan.FromSeconds(levelToPreview.bone3Time).ToString(PlayerConstants.levelCompletionTimeFormat);
    }

    void SetBoneImages()
    {
        int numberOfBones = levelToPreview.GetNumberOfTimeBones();

        singleBoneImage.sprite = boneEmptySprite;
        doubleBoneImage.sprite = boneEmptySprite;
        tripleBoneImage.sprite = boneEmptySprite;

        if (numberOfBones >= 1)
        {
            singleBoneImage.sprite = boneFilledSprite;
        }

        if (numberOfBones >= 2)
        {
            doubleBoneImage.sprite = boneFilledSprite;
        }

        if (numberOfBones == 3)
        {
            tripleBoneImage.sprite = boneFilledSprite;
        }
    }

    private void CleanScrollView()
    {
        foreach(Transform child in leaderboardParent)
        {
            Destroy(child.gameObject);
        }
    }

    async Task PopulateMyStats()
    {
        Steamworks.Data.LeaderboardEntry myEntry = await StatsManager.GetMyLevelLeaderboard(levelToPreview.levelName);

        rankText.text = myEntry.GlobalRank.ToString();

        GameObject entryObject = Instantiate(leaderboardItemPrefab, myLeaderboardParent);
        LeaderboardEntry leaderboardEntry = entryObject.GetComponent<LeaderboardEntry>();
        leaderboardEntry.Init(myEntry);
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
                entries = await StatsManager.GetTopLevelLeaderboard(levelToPreview.levelName);
                leaderboardCache.Add(levelToPreview.levelName, entries);
            }

            if (entries != null && entries.Length > 0)
            {
                Debug.Log($"Leaderboard found, populating preview with {entries.Count()} entries");
                foreach (Steamworks.Data.LeaderboardEntry entry in entries)
                {
                    GameObject entryObject = Instantiate(leaderboardItemPrefab, leaderboardParent);
                    LeaderboardEntry leaderboardEntry = entryObject.GetComponent<LeaderboardEntry>();
                    leaderboardEntry.Init(entry);
                }
            }
        }
    }

}
