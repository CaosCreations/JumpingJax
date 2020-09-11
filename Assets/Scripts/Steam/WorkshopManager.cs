using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.SceneManagement;

public class WorkshopManager : MonoBehaviour
{
    void Start()
    {
        //
        // Log unhandled exceptions created in Async Tasks so we know when something has gone wrong
        //
        TaskScheduler.UnobservedTaskException += (_, e) => { Debug.LogError($"{e.Exception}\n{e.Exception.Message}\n{e.Exception.StackTrace}"); };
    }

    public static async Task<List<Steamworks.Ugc.Item>> DownloadAllSubscribedItems()
    {
        var query = Steamworks.Ugc.Query.All.WhereUserSubscribed(SteamClient.SteamId.AccountId);
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

        string fileToReturn = "";
        foreach (var dirFiles in dir.EnumerateFiles())
        {
            Debug.Log($"{dirFiles.FullName}");
            fileToReturn = dirFiles.FullName;
        }

        return fileToReturn;
    }
}
