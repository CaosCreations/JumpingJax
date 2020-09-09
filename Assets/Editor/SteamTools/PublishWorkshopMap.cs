using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Steamworks;
using System.IO;

public class PublishWorkshopMap : EditorWindow
{
    [MenuItem("Tools/OneLeif/Publish Map")]
    private static void PackageMap()
    {
        EditorWindow.GetWindow(typeof(PublishWorkshopMap));
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Name: ");
        string mapName = GUILayout.TextField("");
        
        GUILayout.Label("Author: ");
        string author = GUILayout.TextField("");

        GUILayout.Label("Version: ");
        string version = GUILayout.TextField("");

        GUILayout.Label("Description: ");
        string description = GUILayout.TextArea("");

        GUILayout.BeginHorizontal();
        GUILayout.Label("No File Selected! ");
        GUILayout.Button("Select a file");
        GUILayout.EndHorizontal();

        GUILayout.Button("Compile");
    }

    private async void UploadMap()
    {
        Debug.Log("publishing");
        DirectoryInfo directory = new DirectoryInfo("C:/Users/ambid/Documents/GitHub/JumpingJax/Assets/Resources/Workshop/Uploads");
        var result = await Steamworks.Ugc.Editor.NewCommunityFile
            .WithTitle("map1")
            .WithDescription("map 1 description")
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
}
