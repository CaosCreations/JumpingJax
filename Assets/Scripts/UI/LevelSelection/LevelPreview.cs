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
    private Steamworks.Data.PublishedFileId replayFileId;
    private List<LeaderboardEntry> leaderboardEntries;

    private void Start()
    {
        replayFileId = new Steamworks.Data.PublishedFileId();
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(Play);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(Back);
    }

    async void Play()
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

        if (replayFileId.Value != 0)
        {
            Debug.Log($"Downloading ghost file UGC with ID: {replayFileId}");
            GameManager.Instance.replayFileLocation = await WorkshopManager.DownloadGhostRun(replayFileId).ConfigureAwait(false);
        }
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


        if (levelToPreview.levelSaveData.isCompleted)
        {
            TimeSpan time = TimeSpan.FromSeconds(levelToPreview.levelSaveData.completionTime);
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
        leaderboardEntries = new List<LeaderboardEntry>();
        foreach(Transform child in leaderboardParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in myLeaderboardParent)
        {
            Destroy(child.gameObject);
        }
    }

    async Task PopulateMyStats()
    {
        Steamworks.Data.LeaderboardEntry? myEntry = await StatsManager.GetMyLevelLeaderboard(levelToPreview.levelName);

        if (myEntry.HasValue)
        {
            Steamworks.Data.LeaderboardEntry entry = myEntry.Value;
            rankText.text = entry.GlobalRank.ToString();

            GameObject entryObject = Instantiate(leaderboardItemPrefab, myLeaderboardParent);
            LeaderboardEntry leaderboardEntry = entryObject.GetComponent<LeaderboardEntry>();
            leaderboardEntry.Init(entry, () => SetReplay(entry));
            leaderboardEntries.Add(leaderboardEntry);
        }
        else
        {
            rankText.text = "N/A";
        }
    }

    async Task PopulateLeaderboard()
    {
        // make HTTP request from steam for leaderboard data
        if (GameManager.Instance.shouldUseSteam)
        {
            Steamworks.Data.LeaderboardEntry[] entries;
            entries = await StatsManager.GetTopLevelLeaderboard(levelToPreview.levelName);

            if (entries != null && entries.Length > 0)
            {
                Array.Sort(entries, new EntryComparer());
                Debug.Log($"Leaderboard found, populating preview with {entries.Count()} entries");
                foreach (Steamworks.Data.LeaderboardEntry entry in entries)
                {
                    GameObject entryObject = Instantiate(leaderboardItemPrefab, leaderboardParent);
                    LeaderboardEntry leaderboardEntry = entryObject.GetComponent<LeaderboardEntry>();
                    leaderboardEntry.Init(entry, () => SetReplay(entry));
                    leaderboardEntries.Add(leaderboardEntry);
                }
            }
        }
    }

    public void SetReplay(Steamworks.Data.LeaderboardEntry entry)
    {
        // Steam has a ulong for if an error occurred: https://partner.steamgames.com/doc/api/ISteamRemoteStorage#k_UGCFileStreamHandleInvalid
        if (entry.AttachedUgcId.HasValue && entry.AttachedUgcId.Value != 18446744073709551615)
        {
            Debug.Log($"Set replay, for ugc Id: {entry.AttachedUgcId}");
            replayFileId = entry.AttachedUgcId.Value;
        }
        else
        {
            Debug.LogError($"No UGC Attached for: {entry.User.Name}");
            replayFileId = new Steamworks.Data.PublishedFileId();
        }

        foreach(LeaderboardEntry leaderboardEntry in leaderboardEntries)
        {
            leaderboardEntry.replayButton.ClearActive();
        }
    }

    public class EntryComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            Steamworks.Data.LeaderboardEntry object1 = (Steamworks.Data.LeaderboardEntry)x;
            Steamworks.Data.LeaderboardEntry object2 = (Steamworks.Data.LeaderboardEntry)y;

            return object1.Score.CompareTo(object2.Score);
        }
    }

}
