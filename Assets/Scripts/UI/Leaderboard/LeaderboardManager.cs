using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public enum LeaderboardTab
    {
        Global, Friends
    }
    public GameObject leaderboardItemPrefab;
    public Transform leaderboardParent;
    public Transform myLeaderboardParent;

    public Button globalButton;
    public Button friendsButton;

    public Steamworks.Data.PublishedFileId replayFileId;
    private List<LeaderboardEntry> leaderboardEntries;

    void Start()
    {
        replayFileId = new Steamworks.Data.PublishedFileId();
        SetupButtons();
    }

    private void SetupButtons()
    {
        globalButton.onClick.RemoveAllListeners();
        globalButton.onClick.AddListener(() => SwitchTab(LeaderboardTab.Global));
        friendsButton.onClick.RemoveAllListeners();
        friendsButton.onClick.AddListener(() => SwitchTab(LeaderboardTab.Friends));
    }

    private void SwitchTab(LeaderboardTab tab)
    {
        CleanScrollView();
    }

    public async Task<string> PopulateMyStats(string levelName)
    {
        Steamworks.Data.LeaderboardEntry? myEntry = await StatsManager.GetMyLevelLeaderboard(levelName);

        if (myEntry.HasValue)
        {
            Steamworks.Data.LeaderboardEntry entry = myEntry.Value;

            GameObject entryObject = Instantiate(leaderboardItemPrefab, myLeaderboardParent);
            LeaderboardEntry leaderboardEntry = entryObject.GetComponent<LeaderboardEntry>();
            leaderboardEntry.Init(entry, () => SetReplay(entry));
            leaderboardEntries.Add(leaderboardEntry);

            return entry.GlobalRank.ToString();
        }

        return "N/A";
    }

    public async Task PopulateLeaderboard(string levelName)
    {
        // make HTTP request from steam for leaderboard data
        if (GameManager.Instance.shouldUseSteam)
        {
            Steamworks.Data.LeaderboardEntry[] entries;
            entries = await StatsManager.GetTopLevelLeaderboard(levelName);

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

        foreach (LeaderboardEntry leaderboardEntry in leaderboardEntries)
        {
            leaderboardEntry.ClearActive();
        }
    }

    public async void SetReplayLevelLoad()
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
        leaderboardEntries = new List<LeaderboardEntry>();
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
