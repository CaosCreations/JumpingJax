using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static async Task SaveLevelCompletion(string levelName, float newCompletionTime)
    {
        var leaderboard = await SteamUserStats.FindOrCreateLeaderboardAsync(levelName, Steamworks.Data.LeaderboardSort.Ascending, Steamworks.Data.LeaderboardDisplay.TimeMilliSeconds);
        
        if (leaderboard.HasValue)
        {
            var leaderboardValue = leaderboard.Value;
            TimeSpan time = TimeSpan.FromSeconds(newCompletionTime);
            Debug.Log($"Leaderboard found, adding score: {time.ToString(PlayerConstants.levelCompletionTimeFormat)}");
            await leaderboardValue.ReplaceScore((int) time.Ticks);
        }
    }

    public static async Task<Steamworks.Data.LeaderboardEntry[]> GetLevelLeaderboard(string levelLeaderboardName)
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

        return new Steamworks.Data.LeaderboardEntry[0];
    }

    public static async Task<float> GetLevelCompletionTime(string levelName)
    {
        var leaderboard = await SteamUserStats.FindOrCreateLeaderboardAsync(levelName, Steamworks.Data.LeaderboardSort.Ascending, Steamworks.Data.LeaderboardDisplay.TimeMilliSeconds);
        if (leaderboard.HasValue)
        {
            var leaderboardValue = leaderboard.Value;
            var entries = await leaderboardValue.GetScoresAroundUserAsync(-1, 0);
            if(entries != null)
            {
                if(entries.Length > 0)
                {
                    int timeInTicks = entries[0].Score;
                    TimeSpan timeSpan = TimeSpan.FromTicks(timeInTicks);
                    return (float) timeSpan.TotalSeconds;
                }
            }
        }

        return 0;
    }
}
