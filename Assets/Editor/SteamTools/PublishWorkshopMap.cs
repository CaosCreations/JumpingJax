﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Steamworks;
using System.IO;

public class PublishWorkshopMap : EditorWindow
{
    [MenuItem("Tools/CaosCreations/Publish Map")]
    private static void PackageMap()
    {
        EditorWindow.GetWindow(typeof(PublishWorkshopMap));
    }

    DirectoryInfo directory = new DirectoryInfo("C:/Users/ambid/Documents/GitHub/JumpingJax/Assets/Resources/Workshop");

    string mapTitle = "";
    string description = "";
    string path = "";
    string imagePath = "";
    private void OnGUI()
    {
        GUILayout.Label("Map Name: ");
        mapTitle = GUILayout.TextField(mapTitle);

        GUILayout.Space(20);

        GUILayout.Label("Description: ");
        description = GUILayout.TextField(description);

        GUILayout.Space(20);

        GUILayout.Label("File Selected: " + path);
        if(GUILayout.Button("Select a file"))
        {
            path = EditorUtility.OpenFilePanel("Select map file", "Assets/Resources/Workshop", "caos");
        }

        GUILayout.Label("Image Selected: " + imagePath);
        if (GUILayout.Button("Select a screenshot"))
        {
            imagePath = EditorUtility.OpenFilePanel("Select screenshot", "Assets/Resources/Workshop", "png");
        }

        if (GUILayout.Button("Publish to steam workshop"))
        {
            UploadMap();
        }
    }

    private async void UploadMap()
    {
        SteamUtil.StartSteam();
        Debug.Log($"publishing: {path}. WAIT to see \"published\"");
        
        var result = await Steamworks.Ugc.Editor.NewCommunityFile
            .WithTitle(mapTitle)
            .WithDescription(description)
            .WithPreviewFile(imagePath)
            .WithContent(path)
            .SubmitAsync();

        if (result.Success)
        {
            Debug.Log($"published : {mapTitle}");
            // See this for more info: https://partner.steamgames.com/doc/features/workshop/implementation#Legal
            if (result.NeedsWorkshopAgreement)
            {
                SteamFriends.OpenWebOverlay($"steam://url/CommunityFilePage/{result.FileId}");
            }
        }
        else
        {
            Debug.LogError($"could not publish: {mapTitle}, error: {result.ToString()}");
        }

        SteamUtil.StopSteam();
    }
}
