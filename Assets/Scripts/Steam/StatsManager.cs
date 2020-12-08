using Steamworks;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
                Steamworks.Data.LeaderboardUpdate? leaderboardUpdate = await leaderboardValue.SubmitScoreAsync((int)time.TotalMilliseconds); // We can use the return here to show the placement update on the winMenu

                // the ghost run data is erased whenever the score is changed.
                await CreateOrUpdateGhostRun(level);
            }
        }
        else
        {
            Debug.LogError("Not saving level completion, steam client is NOT valid");
        }
    }

    private static async Task CreateOrUpdateGhostRun(Level level)
    {
        var leaderboardResult = await SteamUserStats.FindOrCreateLeaderboardAsync(level.levelName, Steamworks.Data.LeaderboardSort.Ascending, Steamworks.Data.LeaderboardDisplay.TimeMilliSeconds);
        if (leaderboardResult.HasValue)
        {
            Steamworks.Data.Leaderboard leaderboard = leaderboardResult.Value;

            Steamworks.Data.LeaderboardEntry[] topEntries = await leaderboard.GetScoresAsync(1, 9);

            if(topEntries != null)
            // Only add ghost run if the run is top 10
            if(level.levelSaveData.completionTime < topEntries[0].Score)
            {
                Steamworks.Data.LeaderboardEntry[] entries = await leaderboard.GetScoresForUsersAsync(new Steamworks.SteamId[] { SteamClient.SteamId });

                if (entries != null && entries.Length > 0)
                {
                    Steamworks.Data.LeaderboardEntry myEntry = entries[0];

                    // Steam has a ulong for if an error occurred: https://partner.steamgames.com/doc/api/ISteamRemoteStorage#k_UGCFileStreamHandleInvalid
                    if (myEntry.AttachedUgcId != 18446744073709551615)
                    {
                        Debug.Log($"CreateOrUpdateGhostRun: Updating Ghost run for UGCId: {myEntry.AttachedUgcId}");
                        await UpdateGhostRun(leaderboard, myEntry, level);
                    }
                    else
                    {
                        Debug.Log($"CreateOrUpdateGhostRun: no valid UGCId found, doing nothing");
                    }
                }
                else
                {
                    await CreateNewGhostRun(leaderboard, level);
                }
            }
        }
    }

    public static async Task UpdateGhostRun(Steamworks.Data.Leaderboard leaderboard, Steamworks.Data.LeaderboardEntry myEntry, Level level)
    {
        string fileTitle = $"ghost_{level.levelName}";

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
                //await CreateNewGhostRun(leaderboard, level);
            }
        }
        else
        {
            Debug.LogError("Tried to update run data, but the leaderboard entry has no attachedUgcId");
        }
    }

    public static async Task CreateNewGhostRun(Steamworks.Data.Leaderboard leaderboard, Level level)
    {
        string fileTitle = $"ghost_{level.levelName}";
        if (SteamRemoteStorage.IsCloudEnabled)
        {
            SteamRemoteStorage.FileWrite(fileTitle, LevelToByteArray(level));
        }
        else
        {
            Debug.LogError("Steam Cloud NOT Enabled");
        }
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

    public static byte[] LevelToByteArray(Level level)
    {
        var binaryFormatter = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            binaryFormatter.Serialize(ms, level);
            return ms.ToArray();
        }
    }

    public static Level LevelFromByteArray(byte[] levelData)
    {
        using(var memoryStream = new MemoryStream())
        {
            var binaryFormatter = new BinaryFormatter();
            memoryStream.Write(levelData, 0, levelData.Length);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var obj = binaryFormatter.Deserialize(memoryStream);
            return (Level)obj;
        }
    }
}
