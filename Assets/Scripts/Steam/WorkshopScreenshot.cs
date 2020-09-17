using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkshopScreenshot : MonoBehaviour
{
    public Camera playerCamera;
    public int resWidth = 1920;
    public int resHeight = 1080;

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            CreateScreenshot();
        }
    }

    private void CreateScreenshot()
    {
        RenderTexture renderTexture = new RenderTexture(resWidth, resHeight, 24);
        playerCamera.targetTexture = renderTexture;

        Texture2D screenshot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        playerCamera.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        playerCamera.targetTexture = null;
        RenderTexture.active = null;

        Destroy(renderTexture);

        byte[] bytes = screenshot.EncodeToPNG();
        string filename = NameScreenshot();
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format($"Took screenshot to: {filename}"));
    }

    private string NameScreenshot()
    {
        return string.Format($"{Application.dataPath}/{SceneManager.GetActiveScene().name}_{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png");
    }

}
