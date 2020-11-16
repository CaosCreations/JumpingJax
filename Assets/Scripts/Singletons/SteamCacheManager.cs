using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamCacheManager : MonoBehaviour
{
    public static SteamCacheManager Instance { get; private set; }

    private LRUMemoryCache<Steamworks.Data.LeaderboardEntry> leaderboardCache;
    private LRUMemoryCache<Steamworks.Data.Image> leaderboardAvatarCache;
    private LRUMemoryCache<Texture2D> ugcPreviewImageCache;

    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        if (SteamCacheManager.Instance == null)
        {
            SteamCacheManager.Instance = this;
        }
        else if (SteamCacheManager.Instance == this)
        {
            Destroy(SteamCacheManager.Instance.gameObject);
            SteamCacheManager.Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        Init();
    }

    void Init()
    {
        leaderboardCache = new LRUMemoryCache<Steamworks.Data.LeaderboardEntry>();
        leaderboardAvatarCache = new LRUMemoryCache<Steamworks.Data.Image>();
        ugcPreviewImageCache = new LRUMemoryCache<Texture2D>();
    }

    public static async void GetUGCPreviewImage(string url)
    {
        await Instance.ugcPreviewImageCache.GetOrCreate(url, async () => await SteamUtil.LoadTextureFromUrl(url));
    }
}
