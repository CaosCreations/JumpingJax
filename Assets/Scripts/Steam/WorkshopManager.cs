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

public class WorkshopManager : MonoBehaviour
{
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
        await DownloadAllSubscribedItems();
        //LoadSceneFromBundle("C:/Users/ambid/Downloads/hop10.caos");
    }

    public async Task DownloadAllSubscribedItems()
    {
        //var query = Steamworks.Ugc.Query.All.WhereUserFollowed(SteamClient.SteamId);
        var query = Steamworks.Ugc.Query.All.SortByCreationDate();
        var result = await query.GetPageAsync(1);

        foreach (Steamworks.Ugc.Item entry in result.Value.Entries)
        {
            Debug.Log($"found: {entry.Title}");
            //var fileDownload = await DownloadWorkshopFile(entry.Id);
        }
    }

    public async Task DownloadWorkshopFile(Steamworks.Data.PublishedFileId id)
    {
        //var q = Steamworks.Ugc.Query.Items.SortByCreationDateAsc();
        //var page = await q.GetPageAsync(1);
        //var file = page.Value.Entries.Where(x => !x.IsInstalled).FirstOrDefault();
        var result = await SteamUGC.QueryFileAsync(id);
        if (!result.HasValue)
        {
            return;
        }
        Steamworks.Ugc.Item file = result.Value;

        Debug.Log($"Found {file.Title}..");
        if (file.IsInstalled)
        {
            Debug.Log($"{file.Title} is already installed");
            return;
        }

        if (!file.Download(true))
        {
            Debug.Log($"Download file with id: {id.Value} doesn't exist");
            return;
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

        foreach (var f in dir.EnumerateFiles())
        {
            Debug.Log($"{f.FullName}");
        }
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
