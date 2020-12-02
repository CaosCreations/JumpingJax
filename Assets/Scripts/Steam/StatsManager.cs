using Steamworks;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
                Steamworks.Data.LeaderboardUpdate? leaderboardUpdate = await leaderboardValue.ReplaceScore((int)time.TotalMilliseconds); // We can use the return here to show the placement update on the winMenu

                if (leaderboardUpdate.HasValue)
                {
                    await CreateGhostRunData(leaderboardValue, level);
                }
                else
                {
                    Debug.LogError($"Could not save leaderboard update");
                }
            }
        }
        else
        {
            Debug.LogError("Not saving level completion, steam client is NOT valid");
        }
    }

    private static async Task CreateGhostRunData(Steamworks.Data.Leaderboard leaderboard, Level level)
    {
        // Create ghost run ugc item
        string fileTitle = $"ghost_{level.levelName}_{SteamClient.SteamId}";
        var newFileResult = await Steamworks.Ugc.Editor.NewCommunityFile
            .WithTitle(fileTitle)
            .WithDescription($"ghost_{level.levelName}_{SteamClient.SteamId}")
            //.WithContent(SteamUtil.GetGhostRunFilePath(level))
            .WithContent(Path.Combine(Application.persistentDataPath, level.levelName))
            .WithTag("Ghost")
            .WithTag("ghost")
            .WithPublicVisibility()
            .SubmitAsync();


        // Attach UGC to leaderboard item
        if (newFileResult.Success)
        {
            Debug.Log($"Uploaded new ghost run with \ntitle: {fileTitle} \nfileId: {newFileResult.FileId}");
            var attachUGCresult = await leaderboard.AttachUgc(newFileResult.FileId);
            Debug.Log($"Attach ghost UGC result: {attachUGCresult}");
            SteamFriends.OpenWebOverlay($"steam://url/CommunityFilePage/{newFileResult.FileId}");
        }
        else
        {
            Debug.LogError($"FAILED to upload new ghost run with: \ntitle {fileTitle} \nresult: {newFileResult.Result}");
        }
    }

    public static async Task GetGhostRunFileId(Steamworks.Data.Leaderboard leaderboard, Level level)
    {
        
    }

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
                var entries = await leaderboard.Value.GetScoresAsync(9);
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
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(timeInTicks);
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
