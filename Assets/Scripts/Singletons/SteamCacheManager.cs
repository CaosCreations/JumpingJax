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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

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
