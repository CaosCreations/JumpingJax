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
        var leaderboard = await SteamUserStats.FindOrCreateLeaderboardAsync(level.levelName, Steamworks.Data.LeaderboardSort.Ascending, Steamworks.Data.LeaderboardDisplay.TimeMilliSeconds);
        
        if (leaderboard.HasValue)
        {
            var leaderboardValue = leaderboard.Value;
            TimeSpan time = TimeSpan.FromSeconds(level.completionTime);
            Debug.Log($"Leaderboard found, adding score: {time.ToString(PlayerConstants.levelCompletionTimeFormat)}");
            await leaderboardValue.ReplaceScore((int) time.Ticks);
        }
    }

    // NOTE: temporary code, as there is currently no way to attach UGC to the leaderboard in FacePunch
    private static async Task CreateGhostRunData()
    {
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
            // TODO replace with Leaderboard.etScoresForUsersAsync with the new version of FacePunch
            var entries = await leaderboardValue.GetScoresAroundUserAsync(-1, 1);
            if(entries != null)
            {
                if(entries.Length > 0)
                {
                    Steamworks.Data.LeaderboardEntry myEntry = new Steamworks.Data.LeaderboardEntry();
                    foreach (Steamworks.Data.LeaderboardEntry entry in entries)
                    {
                        if(entry.User.Id == SteamClient.SteamId)
                        {
                            myEntry = entry;
                        }
                    }
                    int timeInTicks = myEntry.Score;
                    TimeSpan timeSpan = TimeSpan.FromTicks(timeInTicks);
                    return (float) timeSpan.TotalSeconds;
                }
            }
        }

        return 0;
    }
}
