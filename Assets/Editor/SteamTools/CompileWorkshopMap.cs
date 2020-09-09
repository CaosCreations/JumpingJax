using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Experimental.AssetBundlePatching;
using UnityEditor.VersionControl;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class CompileWorkshopMap : MonoBehaviour
{
    [MenuItem("Tools/OneLeif/Compile Map")]
    static void CompileMap()
    {
        string path = "Assets/Resources/Workshop/Uploads";
        AssetBundleBuild build = new AssetBundleBuild();

        if (!AssetDatabase.IsValidFolder(path))
        {
            Debug.Log("No Export folder found, creating one.");
            AssetDatabase.CreateFolder("Assets", "Export");
        }

        try
        {
            Scene currentScene = EditorSceneManager.GetActiveScene();
            List<string> assetNames = new List<string>();
            assetNames.Add(currentScene.path);

            EditorSceneManager.SaveOpenScenes();
            build.assetBundleName = currentScene.name + ".caos";
            build.assetNames = assetNames.ToArray();

            AssetBundleBuild[] buildMap = new AssetBundleBuild[] { build };
            BuildPipeline.BuildAssetBundles(path, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }

        //AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        //buildMap[0].assetBundleName = "mapBundle";
        //buildMap[0].assetNames = new string[] { "Assets/Scenes/BunnyHop1.unity" };


        //AssetBundleManifest x = BuildPipeline.BuildAssetBundles("Assets/StreamingAssets",
        //                                buildMap,
        //                                BuildAssetBundleOptions.None,
        //                                BuildTarget.StandaloneWindows);
    }


}
