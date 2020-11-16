using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamCacheManager : MonoBehaviour
{
    public static SteamCacheManager Instance { get; private set; }

    private Dictionary<string, Steamworks.Data.LeaderboardEntry> leaderboardCache;
    private Dictionary<string, Steamworks.Data.Image> leaderboardAvatarCache;
    private Dictionary<string, Steamworks.Ugc.Item> ugcCache;

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
        leaderboardCache = new Dictionary<string, Steamworks.Data.LeaderboardEntry>();
        leaderboardAvatarCache = new Dictionary<string, Steamworks.Data.Image>();
        ugcCache = new Dictionary<string, Steamworks.Ugc.Item>();
    }

    public void AddToLeaderboardCache()
    {

    }
}
