using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Net;

public class WorkshopManager : MonoBehaviour
{
    public delegate List<Steamworks.Ugc.Item> DownloadCompleteEventHandler();
    public DownloadCompleteEventHandler callback;
    void Start()
    {
        //
        // Log unhandled exceptions created in Async Tasks so we know when something has gone wrong
        //
        TaskScheduler.UnobservedTaskException += (_, e) => { Debug.LogError($"{e.Exception}\n{e.Exception.Message}\n{e.Exception.StackTrace}"); };

        HandleAsyncCalls();
    }

    async void HandleAsyncCalls()
    {
        List<Steamworks.Ugc.Item> items = await DownloadAllSubscribedItems();


        //List<Steamworks.Ugc.Item> items = DownloadAllSubscribedItems().Result;
        //LoadSceneFromBundle("C:/Users/ambid/Downloads/hop10.caos");
    }

    public static async Task<List<Steamworks.Ugc.Item>> DownloadAllSubscribedItems()
    {
        var query = Steamworks.Ugc.Query.All.WhereUserSubscribed(SteamClient.SteamId.AccountId);
        //var query = Steamworks.Ugc.Query.All.SortByCreationDate();
        var result = await query.GetPageAsync(1);

        Debug.Log($"Found {result.Value.TotalCount} subscribed items");

        List<Steamworks.Ugc.Item> toReturn = new List<Steamworks.Ugc.Item>();
        foreach (Steamworks.Ugc.Item entry in result.Value.Entries)
        {
            Debug.Log($"Found Item: {entry.Title}");
            if (!entry.IsInstalled)
            {
                await DownloadWorkshopFile(entry.Id);
            }
            toReturn.Add(entry);
        }

        return toReturn;
    }

    public static async Task<string> DownloadWorkshopFile(Steamworks.Data.PublishedFileId id)
    {
        //var q = Steamworks.Ugc.Query.Items.SortByCreationDateAsc();
        //var page = await q.GetPageAsync(1);
        //var file = page.Value.Entries.Where(x => !x.IsInstalled).FirstOrDefault();
        var result = await SteamUGC.QueryFileAsync(id);
        if (!result.HasValue)
        {
            return string.Empty;
        }
        Steamworks.Ugc.Item file = result.Value;

        Debug.Log($"Found File: {file.Title}..");
        if (file.IsInstalled)
        {
            Debug.Log($"{file.Title} is already installed to {file.Directory}");
            return file.Directory;
        }

        if (!file.Download(true))
        {
            Debug.Log($"Download file with id: {id.Value} doesn't exist");
            return string.Empty;
        }

        Debug.Log($"Starting download for {file.Title}..");

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

        var dir = new DirectoryInfo(file.Directory);

        Debug.Log($"");

        string fileToReturn = "";
        foreach (var f in dir.EnumerateFiles())
        {
            Debug.Log($"{f.FullName}");
            fileToReturn = f.FullName;
        }

        return fileToReturn;
    }

    public static void LoadSceneFromBundle(string path)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(path);
        string[] scenes = bundle.GetAllScenePaths();
        Debug.Log($"scenepath: {scenes[0]}");
        string scene = Path.GetFileNameWithoutExtension(scenes[0]);
        SceneManager.LoadScene(scene);
    }
}
