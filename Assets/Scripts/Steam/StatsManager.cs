using Steamworks;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static async Task SaveLevelCompletion(Level level)
    {
        if (SteamClient.IsValid)
        {
            var leaderboard = await SteamUserStats.FindOrCreateLeaderboardAsync(level.levelName, Steamworks.Data.LeaderboardSort.Ascending, Steamworks.Data.LeaderboardDisplay.TimeMilliSeconds);

            if (leaderboard.HasValue)
            {
                var leaderboardValue = leaderboard.Value;
                TimeSpan time = TimeSpan.FromSeconds(level.levelSaveData.completionTime);
                Debug.Log($"Leaderboard found, adding score: {time.ToString(PlayerConstants.levelCompletionTimeFormat)}");
                await leaderboardValue.ReplaceScore((int)time.Ticks);
            }
        }
        else
        {
            Debug.Log("Not saving level completion, steam client is NOT valid");
        }
    }

    // NOTE: temporary code, as there is currently no way to attach UGC to the leaderboard in FacePunch
    //private static async Task CreateGhostRunData()
    //{
        //var ghostUGCResult = await Steamworks.Ugc.Editor.NewCommunityFile
        //.WithTitle(SteamClient.SteamId + " " + level.levelName)
        //.WithDescription("ghost run")
        //.WithTag("ghost")
        //.WithContent("")
        //.SubmitAsync();

        //if (ghostUGCResult.Success)
        //{
        //    var query = Steamworks.Ugc.Query.Items.WithFileId(ghostUGCResult.FileId);
        //    var result = await query.GetPageAsync(1);
        //    if (result.HasValue)
        //    {
        //        Steamworks.Ugc.Item item = result.Value.Entries.First();
        //        leaderboardValue.AttachUgc();
        //    }
        //}
    //}

    public static async Task<Steamworks.Data.LeaderboardEntry[]> GetTopLevelLeaderboard(string levelLeaderboardName)
    {
        if (SteamClient.IsValid)
        {
            var leaderboard = await SteamUserStats.FindOrCreateLeaderboardAsync(levelLeaderboardName, Steamworks.Data.LeaderboardSort.Ascending, Steamworks.Data.LeaderboardDisplay.TimeMilliSeconds);
            if (!leaderboard.HasValue)
            {
                Debug.LogWarning($"Could not retrieve leaderboard {levelLeaderboardName} from steam");
            }
            else
            {
                var entries = await leaderboard.Value.GetScoresAsync(10);
                return entries;
            }
        }
        else
        {
            Debug.Log("Not getting level leaderboard, steam client is NOT valid");
        }

        return new Steamworks.Data.LeaderboardEntry[0];
    }

    public static async Task<Steamworks.Data.LeaderboardEntry?> GetMyLevelLeaderboard(string levelLeaderboardName)
    {
        if (SteamClient.IsValid)
        {
            var leaderboard = await SteamUserStats.FindOrCreateLeaderboardAsync(levelLeaderboardName, Steamworks.Data.LeaderboardSort.Ascending, Steamworks.Data.LeaderboardDisplay.TimeMilliSeconds);
            if (!leaderboard.HasValue)
            {
                Debug.LogWarning($"Could not retrieve leaderboard {levelLeaderboardName} from steam");
            }
            else
            {
                var leaderboardValue = leaderboard.Value;
                var entries = await leaderboardValue.GetScoresAroundUserAsync(-1, 1);
                if (entries != null)
                {
                    if (entries.Length > 0)
                    {
                        foreach (Steamworks.Data.LeaderboardEntry entry in entries)
                        {
                            if (entry.User.Id == SteamClient.SteamId)
                            {
                                return entry;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("Not getting level leaderboard, steam client is NOT valid");
        }

        return null;
    }

    public static async Task<float> GetLevelCompletionTime(string levelLeaderboardName)
    {
        Steamworks.Data.LeaderboardEntry? myEntry = await GetMyLevelLeaderboard(levelLeaderboardName);

        if (myEntry.HasValue)
        {
            int timeInTicks = myEntry.Value.Score;
            TimeSpan timeSpan = TimeSpan.FromTicks(timeInTicks);
            return (float)timeSpan.TotalSeconds;
        }
        else
        {
            return 0;
        }
        
    }

    public static async Task<int> GetMyRank(string levelLeaderboardName)
    {
        Steamworks.Data.LeaderboardEntry? myEntry = await GetMyLevelLeaderboard(levelLeaderboardName);

        if (myEntry.HasValue)
        {
            return myEntry.Value.GlobalRank;
        }
        else
        {
            return 0;
        }
    }
}
