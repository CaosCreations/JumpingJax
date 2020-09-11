using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssetBundleManager : MonoBehaviour
{
    public static AssetBundleManager Instance { get; private set; }
    static private Dictionary<string, AssetBundle> loadedAssetBundles;

    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        if (AssetBundleManager.Instance == null)
        {
            AssetBundleManager.Instance = this;
        }
        else if (AssetBundleManager.Instance == this)
        {
            Destroy(AssetBundleManager.Instance.gameObject);
            AssetBundleManager.Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public static void UnloadAllBundles()
    {
        foreach(KeyValuePair<string, AssetBundle> entry in loadedAssetBundles)
        {
            entry.Value.Unload(false);
        }

        loadedAssetBundles = new Dictionary<string, AssetBundle>();
    }

    private static AssetBundle GetAssetBundle(string key)
    {
        if(loadedAssetBundles == null)
        {
            loadedAssetBundles = new Dictionary<string, AssetBundle>();
        }

        AssetBundle toReturn = null;
        if(loadedAssetBundles.TryGetValue(key, out toReturn))
        {
            return toReturn;
        }
        else
        {
            AssetBundle newBundle = AssetBundle.LoadFromFile(key);
            loadedAssetBundles.Add(key, newBundle);
            return newBundle;
        }
    }

    public static void LoadSceneFromBundle(string path)
    {
        DirectoryInfo fileInfo = new DirectoryInfo(path);
        string scenePath = fileInfo.EnumerateFiles().First().FullName;

        AssetBundle bundle = GetAssetBundle(scenePath);
        //AssetBundle bundle = AssetBundle.LoadFromFile(scenePath);
        if (bundle == null)
        {
            Debug.LogError($"failed to load asset bundle {scenePath}");
            return;
        }
        string[] scenes = bundle.GetAllScenePaths();
        Debug.Log($"loading bundle from scenepath: {scenes[0]}");
        string scene = Path.GetFileNameWithoutExtension(scenes[0]);
        SceneManager.LoadScene(scene);
    }
}
