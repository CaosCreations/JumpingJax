using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static LeaderboardManager;

public class TestLeaderboardManager : Singleton<TestLeaderboardManager>
{
    public LeaderboardEntry leaderboardItemPrefab;
    public Transform leaderboardParent;
    public Button refreshButton;

    private List<LeaderboardEntry> leaderboardEntries;

    async Awaitable Start()
    {
        leaderboardEntries = new List<LeaderboardEntry>();
        refreshButton.onClick.AddListener(async () => await RefreshLeaderboard());
        await PopulateLeaderboard();
    }

    void Update()
    {
        
    }

    private async Awaitable RefreshLeaderboard()
    {
        foreach(var leaderboardEntry in leaderboardEntries)
        {
            Destroy(leaderboardEntry.gameObject);
        }

        leaderboardEntries = new List<LeaderboardEntry>();

        Debug.Log("BEFORE Populating leaderboard...");
        await PopulateLeaderboard();
        Debug.Log("DONE Populating...");
    }

    public async Awaitable PopulateLeaderboard()
    {
        // make HTTP request from steam for leaderboard data
        if (!SteamClient.IsValid)
        {
            Debug.LogWarning("Not loading leaderboard, steam not started");
            return;
        }
        Debug.Log("Populating leaderboard...");

        Steamworks.Data.LeaderboardEntry[] entries = new Steamworks.Data.LeaderboardEntry[0];

        entries = await GetTopLevelLeaderboard(SteamTestManager.Instance.leaderboardName);

        Debug.Log($"Received {entries.Length} leaderboard entries");

        if (entries != null && entries.Length > 0)
        {
            Array.Sort(entries, new EntryComparer());
            foreach (Steamworks.Data.LeaderboardEntry entry in entries)
            {
                SetupEntry(entry);
            }
        }
    }

    public async Awaitable<Steamworks.Data.LeaderboardEntry[]> GetTopLevelLeaderboard(string levelLeaderboardName)
    {
        if (!SteamClient.IsValid)
        {
            Debug.LogWarning("Not getting top leaderboard, steam client is NOT valid");
        }

        var leaderboard = await SteamUserStats.FindOrCreateLeaderboardAsync(levelLeaderboardName, LeaderboardSort.Ascending, LeaderboardDisplay.TimeMilliSeconds);
        if (!leaderboard.HasValue)
        {
            Debug.LogWarning($"Could not retrieve leaderboard {levelLeaderboardName} from steam");
        }
        else
        {
            var entries = await leaderboard.Value.GetScoresAsync(9);
            return entries;
        }
        

        return new Steamworks.Data.LeaderboardEntry[0];
    }

    public void SetupEntry(Steamworks.Data.LeaderboardEntry entry)
    {
        bool hasAttachedUGC = entry.AttachedUgcId.HasValue && entry.AttachedUgcId.Value != 18446744073709551615;

        LeaderboardEntry leaderboardEntry = Instantiate(leaderboardItemPrefab, leaderboardParent);
        leaderboardEntry.Init(entry, () => {}, LeaderboardTab.Global, hasAttachedUGC);

        leaderboardEntries.Add(leaderboardEntry);
    }
}
