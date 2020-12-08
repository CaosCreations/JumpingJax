using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Threading.Tasks;
using System.IO;

[CreateAssetMenu(fileName = "New TestSteam Command", menuName = "Developer Console/Commands/TestSteam Command")]
public class CommandTestSteam : ConsoleCommand
{
    public override async void Process(string[] args)
    {
        Debug.Log(await GetMyLevelLeaderboard());
    }

    private static async Task<string> GetMyLevelLeaderboard()
    {
        string levelLeaderboardName = "Hop 1";
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
                        return entries[0].AttachedUgcId.ToString();
                    }
                }
            }
        }
        else
        {
            Debug.Log("Not getting level leaderboard, steam client is NOT valid");
        }

        return "";
    }

    private static async Task CreateOrUpdateGhostRun(Level level)
    {
        string levelName = "Hop 1";
        if (SteamClient.IsValid)
        {
            var leaderboardResult = await SteamUserStats.FindOrCreateLeaderboardAsync(levelName, Steamworks.Data.LeaderboardSort.Ascending, Steamworks.Data.LeaderboardDisplay.TimeMilliSeconds);
            if (leaderboardResult.HasValue)
            {
                Steamworks.Data.Leaderboard leaderboard = leaderboardResult.Value;
                Steamworks.Data.LeaderboardEntry[] entries = await leaderboard.GetScoresForUsersAsync(new Steamworks.SteamId[] { SteamClient.SteamId });

                if (entries != null && entries.Length > 0)
                {
                    Steamworks.Data.LeaderboardEntry myEntry = entries[0];
                    // Steam has a ulong for if an error occurred: https://partner.steamgames.com/doc/api/ISteamRemoteStorage#k_UGCFileStreamHandleInvalid
                    if (myEntry.AttachedUgcId != 18446744073709551615)
                    {
                        Debug.Log($"updating ghost run with Id: {myEntry.AttachedUgcId}");
                        await UpdateGhostRun(leaderboard, myEntry, levelName);
                    }
                    else
                    {
                        Debug.Log($"doing nothing, invalid ugc id");
                    }
                }
                else
                {
                    await CreateNewGhostRun(leaderboard, levelName);
                }
            }
        }
    }

    public static async Task UpdateGhostRun(Steamworks.Data.Leaderboard leaderboard, Steamworks.Data.LeaderboardEntry myEntry, string levelName)
    {
        string fileTitle = $"ghost_{levelName}_{SteamClient.SteamId}";

        if (myEntry.AttachedUgcId.HasValue)
        {
            var updateResult = await new Steamworks.Ugc.Editor(myEntry.AttachedUgcId.Value)
            .WithTitle(fileTitle)
            .WithDescription("no description")
            .WithContent(Path.Combine(Application.persistentDataPath, levelName))
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

    public static async Task CreateNewGhostRun(Steamworks.Data.Leaderboard leaderboard, string levelName)
    {
        string fileTitle = $"ghost_{levelName}_{SteamClient.SteamId}";

        // Create ghost run ugc item
        var newFileResult = await Steamworks.Ugc.Editor.NewCommunityFile
            .WithTitle(fileTitle)
            .WithContent(Path.Combine(Application.persistentDataPath, levelName))
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

}