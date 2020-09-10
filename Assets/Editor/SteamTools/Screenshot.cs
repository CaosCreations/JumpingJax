using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Screenshot : EditorWindow
{
    [MenuItem("Tools/CaosCreations/Screenshot")]
    static void TakeScreenshot()
    {
        EditorWindow.GetWindow(typeof(Screenshot));
    }

    string path = Path.Combine(Directory.GetCurrentDirectory(), "testScreenshot.png");
    private void OnGUI()
    {
        if (GUILayout.Button("Screenshot current camera"))
        {
            Debug.Log($"Screenshot created at: {path}");
            //ScreenCapture.CaptureScreenshot(path);
            CreateScreenshot();
        }
    }

    void CreateScreenshot()
    {
        
    }
}
