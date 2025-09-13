using Steamworks;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamTestManager : Singleton<SteamTestManager>
{
    public TMP_InputField recordInput;
    public Button submitRecordButton;

    public string leaderboardName = "TestLeaderboard1";

    public static uint AppId = 1315100;

    void Start()
    {
        StartSteam();
        submitRecordButton.onClick.AddListener(async () =>
        {
            Debug.Log("Submitting record");
            string recordText = recordInput.text;
            if (float.TryParse(recordText, out float record))
            {
                await SubmitScore(record);
            }
        });
    }

    void StartSteam()
    {
        try
        {
            if (!SteamClient.IsValid)
            {
                Debug.Log("Starting Steam client...");
                SteamClient.Init(AppId, false);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Could not connect to steam " + e.Message);
        }
    }

    void Update()
    {
        SteamClient.RunCallbacks();
    }

    private void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }


    public async Task SubmitScore(float score)
    {
        if (!SteamClient.IsValid)
        {
            Debug.LogError("Not saving level completion, steam client is NOT valid");
            return;
        }

        var leaderboard = await SteamUserStats.FindOrCreateLeaderboardAsync(
            leaderboardName,
            Steamworks.Data.LeaderboardSort.Ascending,
            Steamworks.Data.LeaderboardDisplay.TimeMilliSeconds
            );

        if (leaderboard.HasValue)
        {
            Debug.Log($"Leaderboard found, adding score: {score}");
            var leaderboardValue = leaderboard.Value;

            TimeSpan time = TimeSpan.FromSeconds(score);
            Debug.Log($"Leaderboard found, adding score: {time.ToString(PlayerConstants.levelCompletionTimeFormat)}");

            Steamworks.Data.LeaderboardUpdate? leaderboardUpdate = await leaderboardValue.SubmitScoreAsync((int)time.TotalMilliseconds); // We can use the return here to show the placement update on the winMenu

            if (leaderboardUpdate.HasValue)
            {
                Steamworks.Data.LeaderboardUpdate leaderboardUpdateValue = leaderboardUpdate.Value;
                Debug.Log($"Leaderboard Update for {leaderboardName}: Changed?: {leaderboardUpdateValue.Changed} rankChange: {leaderboardUpdateValue.RankChange} new Score: {leaderboardUpdateValue.Score}");
            }
            else
            {
                Debug.LogError($"Leaderboard NOT updated for {leaderboardName} with score {score}");
            }
            // The ghost run data is erased whenever the score is changed, so we HAVE to add it AFTER submitting a score.
            await CreateNewGhostRun(leaderboardValue, leaderboardName, score);
        }
        else
        {
            Debug.LogError($"Leaderboard NOT found for {leaderboardName}");
        }
    }

    public static async Task CreateNewGhostRun(Steamworks.Data.Leaderboard leaderboard, string leaderboardName, float score)
    {
        string fileTitle = $"ghost_{leaderboardName}";

        // Create ghost run ugc item
        var newFileResult = await Steamworks.Ugc.Editor.NewGameManagedFile
            .WithTitle(fileTitle)
            .WithContent("C:\\Users\\ambid\\AppData\\LocalLow\\Caos Creations\\jumpingjax\\LevelData\\Portal 1\\Portal 1.save")
            .WithDescription($"Run time: {score}")
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
            var attachUgcResult = await leaderboard.AttachUgc(newFileResult.FileId);
            Debug.Log($"Attach ghost UGC result: {attachUgcResult}");
        }
        else
        {
            Debug.LogError($"FAILED to upload new ghost run with: \ntitle {fileTitle} \nresult: {newFileResult.Result}");
        }
    }
}
