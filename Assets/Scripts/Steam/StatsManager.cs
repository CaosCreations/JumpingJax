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
                    await CreateOrUpdateGhostRun(leaderboardValue, level);
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

    private static async Task CreateOrUpdateGhostRun(Steamworks.Data.Leaderboard leaderboard, Level level)
    {
        string fileTitle = $"ghost_{level.levelName}_{SteamClient.SteamId}";

        Steamworks.Data.LeaderboardEntry[] entries = await leaderboard.GetScoresForUsersAsync(new Steamworks.SteamId[] { SteamClient.SteamId });

        bool runExists = entries.Length > 0;


        if (runExists)
        {
            Steamworks.Data.LeaderboardEntry myEntry = entries[0];
            await UpdateGhostRun(leaderboard, myEntry, level);
        }
        else
        {
            await CreateNewGhostRun(leaderboard, level);
        }
    }

    public static async Task UpdateGhostRun(Steamworks.Data.Leaderboard leaderboard, Steamworks.Data.LeaderboardEntry myEntry, Level level)
    {
        string fileTitle = $"ghost_{level.levelName}_{SteamClient.SteamId}";

        if (myEntry.AttachedUgcId.HasValue)
        {
            var updateResult = await new Steamworks.Ugc.Editor(myEntry.AttachedUgcId.Value)
            .WithTitle(fileTitle)
            .WithDescription("no description")
            .WithContent(Path.Combine(Application.persistentDataPath, level.levelName))
            .WithTag("ghostRuns")
            .WithPublicVisibility()
            .SubmitAsync();

            if (updateResult.Success)
            {
                Debug.Log($"Updated ghost run with \ntitle: {fileTitle} \nfileId: {updateResult.FileId}");
            }
            else
            {
                Debug.Log($"FAILED to update ghost run with: \ntitle {fileTitle} \nfileId {myEntry.AttachedUgcId.Value} \nresult: {updateResult.Result} \n Creating NEW run");
                await CreateNewGhostRun(leaderboard, level);
            }
        }
        else
        {
            Debug.LogError("Tried to update run data, but the leaderboard entry has no attachedUgcId");
        }
    }

    public static async Task CreateNewGhostRun(Steamworks.Data.Leaderboard leaderboard, Level level)
    {
        string fileTitle = $"ghost_{level.levelName}_{SteamClient.SteamId}";

        // Create ghost run ugc item
        var newFileResult = await Steamworks.Ugc.Editor.NewCommunityFile
            .WithTitle(fileTitle)
            .WithContent(Path.Combine(Application.persistentDataPath, level.levelName))
            .WithDescription("no description")
            .WithTag("ghostRuns")
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
                var entries = await leaderboardValue.GetScoresForUsersAsync(new Steamworks.SteamId[] { SteamClient.SteamId });
                if (entries != null)
                {
                    if (entries.Length > 0)
                    {
                        return entries[0];   
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
