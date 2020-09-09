using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEditor;

public class WorkshopManager : MonoBehaviour
{
    AssetReference sceneFolder;
    void Start()
    {
        //
        // Log unhandled exceptions created in Async Tasks so we know when something has gone wrong
        //
        TaskScheduler.UnobservedTaskException += (_, e) => { Debug.LogError($"{e.Exception}\n{e.Exception.Message}\n{e.Exception.StackTrace}"); };



        //GetAllByCreationDate();
        //GetAllItemsByTrend();
        //Steamworks.Data.PublishedFileId id = new Steamworks.Data.PublishedFileId();
        //id = 2223679777;
        //DownloadItem(id);
        //LoadSceneFromBundle("D:/SteamLibrary/steamapps/workshop/content/1315100/2223679777/Hop1.unity");
        PublishAssetBundle("testBundle", "descr", "Assets/Resources/Workshop/Uploads");
        //LoadSceneFromBundle("Hop1");
        //HandleAsyncCalls();
    }

    async void HandleAsyncCalls()
    {
        DirectoryInfo directory = new DirectoryInfo("C:/Users/ambid/Documents/GitHub/JumpingJax/Assets/Resources/Workshop/Uploads/Hop1.unity");
        await PublishWorkshopFile("test", "test description", directory);
        await WorkshopDownload();
    }

    void Update()
    {
        
    }

    public static async void GetAllByCreationDate()
    {
        var query = Steamworks.Ugc.Query.All.SortByCreationDate();
        var result = await query.GetPageAsync(1);
        Debug.Log($"ResultCount: {result?.ResultCount}");

    }

    public static async void GetAllItemsByTrend()
    {
        var query = Steamworks.Ugc.Query.Items.RankedByTrend();
        var result = await query.GetPageAsync(1);
        Debug.Log($"ResultCount: {result?.ResultCount}");
    }

    // Get all levels the player has subscribed to
    public static async void GetSubscribedLevels()
    {
        var query = Steamworks.Ugc.Query.All.WhereUserFollowed(SteamClient.SteamId);

        var result = await query.GetPageAsync(1);

        Debug.Log($"ResultCount: {result?.ResultCount}");

        foreach(Steamworks.Ugc.Item entry in result.Value.Entries)
        {
            Debug.Log($"found: {entry.Title}");
        }

        // TODO: convert to level array and return the levels
    }



    public static async Task PublishWorkshopFile(string title, string description, DirectoryInfo directory)
    {
        var result = await Steamworks.Ugc.Editor.NewCommunityFile
            .WithTitle(title)
            .WithDescription(description)
            .WithContent(directory)
            .SubmitAsync();

        if (result.Success)
        {
            Debug.Log($"published : {title}");
            // See this for more info: https://partner.steamgames.com/doc/features/workshop/implementation#Legal
            if (result.NeedsWorkshopAgreement)
            {
                SteamFriends.OpenWebOverlay($"steam://url/CommunityFilePage/{result.FileId}");
            }
        }
        else
        {
            Debug.LogError($"could not publish: {title}, error: {result.ToString()}");
        }
    }

    public static async Task PublishAssetBundle(string title, string description, string path)
    {

        //var result = await Steamworks.Ugc.Editor.NewCommunityFile
        //    .WithTitle(title)
        //    .WithDescription(description)
        //    .WithContent(directory)
        //    .SubmitAsync();

        //if (result.Success)
        //{
        //    Debug.Log($"published : {title}");
        //    // See this for more info: https://partner.steamgames.com/doc/features/workshop/implementation#Legal
        //    if (result.NeedsWorkshopAgreement)
        //    {
        //        SteamFriends.OpenWebOverlay($"steam://url/CommunityFilePage/{result.FileId}");
        //    }
        //}
        //else
        //{
        //    Debug.LogError($"could not publish: {title}, error: {result.ToString()}");
        //}
    }


    public static async void DownloadItem(Steamworks.Data.PublishedFileId fileId)
    {
        var result = await SteamUGC.QueryFileAsync(fileId);
        if (result.HasValue)
        {
            Steamworks.Ugc.Item item = result.Value;
            item.Download();
            Debug.Log($"Downloaded {item.Title}");
        }
        else
        {
            Debug.LogError($"Could not download {fileId.Value}");
        }
    }

    public async Task WorkshopDownload()
    {
        //var q = Steamworks.Ugc.Query.Items.SortByCreationDateAsc();
        var result = await SteamUGC.QueryFileAsync(2223679777);
        if (!result.HasValue)
        {
            return;
        }
        Steamworks.Ugc.Item file = result.Value;
        //var page = await q.GetPageAsync(1);
        //var file = page.Value.Entries.Where(x => !x.IsInstalled).FirstOrDefault();

        Debug.Log($"Found {file.Title}..");

        if (!file.Download(true))
        {
            Debug.Log($"Download returned false!?");
            return;
        }

        Debug.Log($"Downloading..");

        while (file.NeedsUpdate)
        {
            await Task.Delay(100);

            Debug.Log($"Downloading... ({file.DownloadAmount:0.000}) [{file.DownloadBytesDownloaded}/{file.DownloadBytesTotal}]");
        }

        while (!file.IsInstalled)
        {
            await Task.Delay(100);

            Debug.Log($"Installing...");
        }

        Debug.Log($"Installed to {file.Directory}");

        var dir = new System.IO.DirectoryInfo(file.Directory);

        Debug.Log($"");

        foreach (var f in dir.EnumerateFiles())
        {
            Debug.Log($"{f.FullName}");
        }
    }

    public void LoadSceneFromBundle(string path)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        string[] scenes = bundle.GetAllScenePaths();
        Debug.Log($"scenepath: {scenes[0]}");
        string scene = Path.GetFileNameWithoutExtension(scenes[0]);
        SceneManager.LoadScene(scene);
    }
}
