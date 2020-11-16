using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SteamUtil
{
    public static void WipeAllStats()
    {
        StartSteam();
        SteamUserStats.ResetAll(true); // true = wipe achivements too
        SteamUserStats.StoreStats();
        SteamUserStats.RequestCurrentStats();
        StopSteam();

        Debug.Log("stats have been wiped");
    }

    public static void StartSteam()
    {
        try
        {
            if (!SteamClient.IsValid)
            {
                SteamClient.Init(GameManager.AppId, true);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could not connect to steam " + e.Message);
        }
    }

    public static void StopSteam()
    {
        SteamClient.Shutdown();
    }

    public static async Task<Steamworks.Data.Image?> GetSteamFriendAvatar(SteamId steamId)
    {
        Steamworks.Data.Image? avatar = await SteamFriends.GetMediumAvatarAsync(steamId);

        if (avatar.HasValue)
        {
            return avatar.Value;
        }

        return null;
    }
}
