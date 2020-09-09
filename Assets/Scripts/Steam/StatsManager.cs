using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static void SaveLevelCompletion(string levelName, float newCompletionTime)
    {
        var levelCompletionTime = Steamworks.SteamUserStats.GetStatFloat(levelName);

        if(newCompletionTime < levelCompletionTime || levelCompletionTime == 0)
        {
            Steamworks.SteamUserStats.SetStat(levelName, newCompletionTime);
        }

        Steamworks.SteamUserStats.StoreStats();
    }

    public static async Task<Steamworks.Data.LeaderboardEntry[]> GetLevelLeaderboard(string levelLeaderboardName)
    {
        var leaderboard = await SteamUserStats.FindLeaderboardAsync(levelLeaderboardName);
        if (!leaderboard.HasValue)
        {
            Debug.LogError($"Could not retrieve leaderboard {levelLeaderboardName} from steam");
        }
        else
        {
            var entries = await leaderboard.Value.GetScoresAsync(10);
            return entries;
        }

        return new Steamworks.Data.LeaderboardEntry[0];
    }
}
