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

                // The ghost run data is erased whenever the score is changed, so we HAVE to add it AFTER submitting a score.
                await CheckGhostRunCreate(leaderboardValue, level);
            }
        }
        else
        {
            Debug.LogError("Not saving level completion, steam client is NOT valid");
        }
    }

    private static async Task CheckGhostRunCreate(Steamworks.Data.Leaderboard leaderboard, Level level)
    {
        Steamworks.Data.LeaderboardEntry[] topEntries = await leaderboard.GetScoresAsync(10, 1);

        // Only add ghost run if the run is top 10
        if (topEntries == null)
        {
            Debug.Log("Adding first entry to leaderboard");
            await CreateNewGhostRun(leaderboard, level);
        }
        if (topEntries.Length < 10) 
        {
            Debug.Log($"Adding new ghost run for entry number {topEntries.Length}");
            await CreateNewGhostRun(leaderboard, level);
        }else if(level.levelSaveData.completionTime < topEntries[9].Score)
        {
            Debug.Log($"Adding ghost run, replacing top 10 score");
            await CreateNewGhostRun(leaderboard, level);
        }
    }

    public static async Task CreateNewGhostRun(Steamworks.Data.Leaderboard leaderboard, Level level)
    {
        string fileTitle = $"ghost_{level.levelName}";

        // Create ghost run ugc item
        var newFileResult = await Steamworks.Ugc.Editor.NewGameManagedFile
            .WithTitle(fileTitle)
            .WithContent(Path.Combine(Application.persistentDataPath, level.levelName))
            .WithDescription("no description")
            .WithTag("ghostRuns")
            .WithPublicVisibility()
            .SubmitAsync();

        if (newFileResult.NeedsWorkshopAgreement)
        {
            Debug.Log("When creating ghost run, player needs workshop agreement");
            SteamFriends.OpenWebOverlay($"steam://url/CommunityFilePage/{newFileResult.FileId}");
        }

        // Attach UGC to leaderboard item
        if (newFileResult.Success)
        {
            Debug.Log($"Uploaded new ghost run with \ntitle: {fileTitle} \nfileId: {newFileResult.FileId}");
            var attachUGCresult = await leaderboard.AttachUgc(newFileResult.FileId);
            Debug.Log($"Attach ghost UGC result: {attachUGCresult}");
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
