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
    [MenuItem("Tools/CaosCreations/Compile Map")]
    static void CompileMap()
    {
        string path = "Assets/Resources/Workshop";
        AssetBundleBuild build = new AssetBundleBuild();

        if (!AssetDatabase.IsValidFolder(path))
        {
            Debug.Log("No Uploads folder found, make sure this folder exists: Assets/Resources/Workshop/");
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
    }
}
