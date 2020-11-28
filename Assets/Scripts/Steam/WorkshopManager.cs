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

    public static async Task<Steamworks.Data.PublishedFileId> PublishItem(Level levelToPublish)
    {
        Steamworks.Data.PublishedFileId toReturn = new Steamworks.Data.PublishedFileId();

        if (SteamClient.IsValid)
        {
            Debug.Log($"publishing: {levelToPublish.levelEditorScriptableObjectPath}. WAIT to see \"published\"");

            var result = await Steamworks.Ugc.Editor.NewCommunityFile
                .WithTitle(levelToPublish.levelName)
                .WithDescription(levelToPublish.description)
                .WithPreviewFile(levelToPublish.previewImagePath)
                .WithContent(levelToPublish.levelEditorScenePath)
                .SubmitAsync();

            if (result.Success)
            {
                Debug.Log($"published : {levelToPublish.levelName}");
                // See this for more info: https://partner.steamgames.com/doc/features/workshop/implementation#Legal
                toReturn = result.FileId;
            }
            else
            {
                Debug.LogError($"could not publish: {levelToPublish.levelName}, error: {result}");
            }

            if (result.NeedsWorkshopAgreement)
            {
                Debug.Log($"opening steam overlay to: steam://url/CommunityFilePage/{result.FileId}");
                SteamFriends.OpenWebOverlay($"steam://url/CommunityFilePage/{result.FileId}");
            }
            else
            {
                Debug.Log("User has accepted workshop agreement already");
            }
        }

        return toReturn;
    }

    public static async Task UpdateItem(Level levelToPublish)
    {
        if (SteamClient.IsValid)
        {
            Debug.Log($"updating: {levelToPublish.levelEditorScriptableObjectPath}. WAIT to see \"updated\"");

            var result = await new Steamworks.Ugc.Editor(levelToPublish.fileId)
                .WithTitle(levelToPublish.levelName)
                .WithDescription(levelToPublish.description)
                .WithPreviewFile(levelToPublish.previewImagePath)
                .WithContent(levelToPublish.levelEditorScenePath)
                .SubmitAsync();

            if (result.Success)
            {
                Debug.Log($"updated : {levelToPublish.levelName}");
            }
            else
            {
                Debug.LogError($"could not update: {levelToPublish.levelName}, error: {result.ToString()}");
            }
        }
    }

    public static async Task<List<Steamworks.Ugc.Item>> DownloadAllSubscribedItems()
    {
        List<Steamworks.Ugc.Item> toReturn = new List<Steamworks.Ugc.Item>();

        if (SteamClient.IsValid)
        {
            var query = Steamworks.Ugc.Query.All.WhereUserSubscribed(SteamClient.SteamId.AccountId);
            var result = await query.GetPageAsync(1);

            Debug.Log($"Found {result.Value.TotalCount} subscribed items");

            foreach (Steamworks.Ugc.Item entry in result.Value.Entries)
            {
                Debug.Log($"Found Item: {entry.Title}");
                if (!entry.IsInstalled)
                {
                    await DownloadWorkshopFile(entry.Id);
                }
                else if(entry.IsInstalled && entry.NeedsUpdate)
                {
                    Debug.Log($"Updating installed item {entry.Title}");
                    await DownloadWorkshopFile(entry.Id);
                }
                toReturn.Add(entry);
            }
        }
        else
        {
            Debug.Log("Not Downloading Subscribed items, Steam is NOT valid");
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

        if (!await file.DownloadAsync())
        {
            Debug.Log($"Download file with id: {id.Value} doesn't exist");
            return string.Empty;
        }

        Debug.Log($"Starting download for {file.Title}..");

        while (file.NeedsUpdate)
        {
            await Task.Delay(1000);

            Debug.Log($"Downloading Update... ({file.DownloadAmount:0.000}) [{file.DownloadBytesDownloaded}/{file.DownloadBytesTotal}]");
        }

        while (!file.IsInstalled)
        {
            await Task.Delay(1000);

            Debug.Log($"Installing... {file.Title}");
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
