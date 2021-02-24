using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum LeaderboardTab
{
    Global, Friends, Mine
}

public class LeaderboardManager : MonoBehaviour
{
    
    public GameObject leaderboardItemPrefab;
    public Transform leaderboardParent;
    public Transform myLeaderboardParent;

    public Button globalButton;
    public Text globalButtonText;
    public Button friendsButton;
    public Text friendsButtonText;

    public Color normalColor = new Color(149 / 255f, 237 / 255f, 194 / 255f); // light green color
    public Color activeColor = new Color(1, 1, 1); // white color

    public Steamworks.Data.PublishedFileId replayFileId;
    private List<LeaderboardEntry> leaderboardEntries;
    private string currentLevelName;

    public string currentRank;
    public GameObject tooltip;
    private RectTransform myRectTransform;

    void Start()
    {
        replayFileId = new Steamworks.Data.PublishedFileId();

        globalButton.onClick.RemoveAllListeners();
        globalButton.onClick.AddListener(() => SwitchTab(LeaderboardTab.Global));
        friendsButton.onClick.RemoveAllListeners();
        friendsButton.onClick.AddListener(() => SwitchTab(LeaderboardTab.Friends));

        myRectTransform = GetComponent<RectTransform>();
        tooltip.SetActive(false);
    }
    public async Task InitAsync(string levelName)
    {
        leaderboardEntries = new List<LeaderboardEntry>();
        currentLevelName = levelName;
        CleanScrollView();

        await PopulateMyStats();
        await PopulateLeaderboard(LeaderboardTab.Global);
        await PopulateLeaderboard(LeaderboardTab.Friends);
        SwitchTab(LeaderboardTab.Global);
    }

    private void SwitchTab(LeaderboardTab tab)
    {
        globalButtonText.color = tab == LeaderboardTab.Global ? activeColor : normalColor;
        friendsButtonText.color = tab == LeaderboardTab.Friends ? activeColor : normalColor;
        foreach (LeaderboardEntry entry in leaderboardEntries)
        {
            entry.gameObject.SetActive(entry.leaderboardTab == tab || entry.leaderboardTab == LeaderboardTab.Mine);
        }
    }

    public async Task PopulateLeaderboard(LeaderboardTab tab)
    {
        // make HTTP request from steam for leaderboard data
        if (GameManager.Instance.shouldUseSteam)
        {
            Steamworks.Data.LeaderboardEntry[] entries = new Steamworks.Data.LeaderboardEntry[0];

            if(tab == LeaderboardTab.Global)
            {
                entries = await StatsManager.GetTopLevelLeaderboard(currentLevelName);
            }
            else if(tab == LeaderboardTab.Friends)
            {
                entries = await StatsManager.GetFriendsLeaderboard(currentLevelName);
            }

            if (entries != null && entries.Length > 0)
            {
                Array.Sort(entries, new EntryComparer());
                foreach (Steamworks.Data.LeaderboardEntry entry in entries)
                {
                    SetupEntry(entry, tab);
                }
            }
        }
    }

    public async Task PopulateMyStats()
    {
        Steamworks.Data.LeaderboardEntry? myEntry = await StatsManager.GetMyLevelLeaderboard(currentLevelName);

        if (myEntry.HasValue)
        {
            Steamworks.Data.LeaderboardEntry entry = myEntry.Value;
            SetupEntry(entry, LeaderboardTab.Mine);
            currentRank =  entry.GlobalRank.ToString();
        }
        else
        {
            currentRank = "N/A";
        }
    }

    public void SetupEntry(Steamworks.Data.LeaderboardEntry entry, LeaderboardTab tab)
    {
        bool hasAttachedUGC = entry.AttachedUgcId.HasValue && entry.AttachedUgcId.Value != 18446744073709551615;
        Transform parent = tab == LeaderboardTab.Mine ? myLeaderboardParent  : leaderboardParent;

        GameObject entryObject = Instantiate(leaderboardItemPrefab, parent);

        LeaderboardEntry leaderboardEntry = entryObject.GetComponent<LeaderboardEntry>();
        leaderboardEntry.Init(entry, () => SelectedReplayButton(leaderboardEntry), tab, hasAttachedUGC);

        leaderboardEntries.Add(leaderboardEntry);
    }

    public void SelectedReplayButton(LeaderboardEntry entry)
    {
        bool isChecked = entry.replayCheck.activeInHierarchy;

        foreach (LeaderboardEntry entryToClear in leaderboardEntries)
        {
            entryToClear.ClearActive();
        }

        if (!isChecked)
        {
            Steamworks.Data.LeaderboardEntry leaderboardEntry = entry.leaderboardEntry;
            // Steam has a ulong for if an error occurred: https://partner.steamgames.com/doc/api/ISteamRemoteStorage#k_UGCFileStreamHandleInvalid
            if (leaderboardEntry.AttachedUgcId.HasValue && leaderboardEntry.AttachedUgcId.Value != 18446744073709551615)
            {
                Debug.Log($"Set replay, for ugc Id: {leaderboardEntry.AttachedUgcId}");
                replayFileId = leaderboardEntry.AttachedUgcId.Value;
                tooltip.SetActive(OptionsPreferencesManager.GetLeaderboardGhostTooltip());
                entry.SetCheckboxActive();
            }
            else
            {
                Debug.LogWarning($"No UGC Attached for: {leaderboardEntry.User.Name}");
                replayFileId = new Steamworks.Data.PublishedFileId();
            }
        }
        else
        {
            tooltip.SetActive(false);
            replayFileId = new Steamworks.Data.PublishedFileId();
        }
    }

    public async void SetReplayLocation()
    {
        if (replayFileId.Value != 0)
        {
            Debug.Log($"Downloading ghost file UGC with ID: {replayFileId}");
            AsyncTaskReporter.Instance.ghostDownloadRunning = true;
            GameManager.Instance.ReplayFileLocation = await WorkshopManager.DownloadGhostRun(replayFileId).ConfigureAwait(false);
        }
    }

    public void CleanScrollView()
    {
        foreach (Transform child in leaderboardParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in myLeaderboardParent)
        {
            Destroy(child.gameObject);
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
