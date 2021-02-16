using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

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
            Debug.LogWarning("Could not connect to steam " + e.Message);
        }
    }

    public static void StopSteam()
    {
        SteamClient.Shutdown();
    }

    public static async Task<Texture2D> GetSteamFriendAvatar(SteamId steamId)
    {
        Friend friend = new Friend(steamId);
        await friend.RequestInfoAsync();

        Steamworks.Data.Image? avatarImage = await friend.GetMediumAvatarAsync();

        if (avatarImage.HasValue)
        {
            return LoadTextureFromImage(avatarImage.Value);
        }

        return null;
    }

    public static Texture2D LoadTextureFromImage(Steamworks.Data.Image img)
    {
        var texture = new Texture2D((int)img.Width, (int)img.Height);

        for (int x = 0; x < img.Width; x++)
            for (int y = 0; y < img.Height; y++)
            {
                var p = img.GetPixel(x, y);

                texture.SetPixel(x, (int)img.Height - y, new UnityEngine.Color32(p.r, p.g, p.b, p.a));
            }

        texture.Apply();

        return texture;
    }

    public static async Task<Texture2D> LoadTextureFromUrl(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url, true);

        var r = request.SendWebRequest();

        while (!r.isDone)
        {
            await Task.Delay(10);
        }

        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError($"Error downloading texture from url: {url}");
            return new Texture2D(100, 100);
        }

        DownloadHandlerTexture dh = request.downloadHandler as DownloadHandlerTexture;
        dh.texture.name = url;

        return dh.texture;
    }
}
