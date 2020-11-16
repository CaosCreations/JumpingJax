using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SteamCacheManager : MonoBehaviour
{
    public static SteamCacheManager Instance { get; private set; }

    private LRUMemoryCache<Texture2D> userAvatarCache;
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
        userAvatarCache = new LRUMemoryCache<Texture2D>();
        ugcPreviewImageCache = new LRUMemoryCache<Texture2D>();
    }

    public static async Task<Texture2D> GetUGCPreviewImage(string url)
    {
        return await Instance.ugcPreviewImageCache.GetOrCreate(url, async () => await SteamUtil.LoadTextureFromUrl(url));
    }

    public static async Task<Texture2D> GetUserAvatar(Steamworks.SteamId steamId)
    {
        Texture2D texture = await Instance.userAvatarCache.GetOrCreate(steamId, async () => await SteamUtil.GetSteamFriendAvatar(steamId));
        return texture;
    }
}
