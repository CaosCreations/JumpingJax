using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsPreferencesManager
{
    public const string resolutionWidthKey = "ResolutionWidth";
    public const int defaultResolutionWidth = 1920;

    public const string resolutionHeightKey = "ResolutionHeight";
    public const int defaultResolutionHeight = 1080;

    public const string volumeKey = "Volume";
    public const int defaultVolume = -10;

    public const string qualityKey = "Quality";
    public const int defaultQuality = 0;

    public const string fullScreenKey = "IsFullScreen";
    public const int defaultIsFullScreen = 0;

    public const string sensitivityKey = "Sensitivity";
    public const float defaultSensitivity = 0.2f;

    public const string portalRecursionKey = "PortalRecursion";
    public const int defaultPortalRecursion = 2;

    public const string cameraFOVKey = "CameraFOV";
    public const int defaultCameraFOV = 90;

    public const string crosshairToggleKey = "CrosshairToggle";
    public const int defaultCrosshairToggle = 1;

    public const string speedToggleKey = "SpeedToggle";
    public const int defaultSpeedToggle = 1;

    public const string timeToggleKey = "TimeToggle";
    public const int defaultTimeToggle = 1;

    public const string keyPressedToggleKey = "KeyPressedToggle";
    public const int defaultKeyPressedToggle = 1;

    public const string tutorialToggleKey = "TutorialToggle";
    public const int defaultTutorialToggle = 1;

    public const string ghostToggleKey = "GhostToggle";
    public const int defaultGhostToggle = 1;

    public const string consoleToggleKey = "ConsoleToggle";
    public const int defaultConsoleToggle = 0;

    public static int GetResolutionWidth()
    {
        return PlayerPrefs.GetInt(resolutionWidthKey, defaultResolutionWidth);
    }

    public static int GetResolutionHeight()
    {
        return PlayerPrefs.GetInt(resolutionWidthKey, defaultResolutionHeight);
    }

    public static void SetResolution(int width, int height)
    {
        PlayerPrefs.SetInt(resolutionWidthKey, width);
        PlayerPrefs.SetInt(resolutionHeightKey, height);
    }

    public static float GetVolume()
    {
        return PlayerPrefs.GetFloat(volumeKey, defaultVolume);
    }

    public static void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat(volumeKey, volume);
    }

    public static int GetQuality()
    {
        return PlayerPrefs.GetInt(qualityKey, defaultQuality);
    }

    public static void SetQuality(int quality)
    {
        PlayerPrefs.SetInt(qualityKey, quality);
    }

    public static bool GetFullScreen()
    {
        int isFullScreen = PlayerPrefs.GetInt(fullScreenKey, defaultIsFullScreen);
        return isFullScreen == 0 ? false : true;
    }

    public static void SetFullScreen(bool isFullScreen)
    {
        PlayerPrefs.SetInt(fullScreenKey, isFullScreen ? 1 : 0);
    }

    public static float GetSensitivity()
    {
        return PlayerPrefs.GetFloat(sensitivityKey, defaultSensitivity);
    }

    public static void SetSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat(sensitivityKey, sensitivity);
    }

    public static int GetPortalRecursion()
    {
        return PlayerPrefs.GetInt(portalRecursionKey, defaultPortalRecursion);
    }

    public static void SetPortalRecursion(int recursionLevel)
    {
        PlayerPrefs.SetInt(portalRecursionKey, recursionLevel);
    }

    public static int GetCameraFOV()
    {
        return PlayerPrefs.GetInt(cameraFOVKey, defaultCameraFOV);
    }

    public static void SetCameraFOV(int fieldOfViewLevel)
    {
        PlayerPrefs.SetInt(cameraFOVKey, fieldOfViewLevel);
    }

    public static bool GetCrosshairToggle()
    {
        int isOn = PlayerPrefs.GetInt(crosshairToggleKey, defaultCrosshairToggle);
        return isOn == 0 ? false : true;
    }

    public static void SetCrosshairToggle(bool isOn)
    {
        PlayerPrefs.SetInt(crosshairToggleKey, isOn ? 1 : 0);
    }

    public static bool GetSpeedToggle()
    {
        int isOn = PlayerPrefs.GetInt(speedToggleKey, defaultSpeedToggle);
        return isOn == 0 ? false : true;
    }

    public static void SetSpeedToggle(bool isOn)
    {
        PlayerPrefs.SetInt(speedToggleKey, isOn ? 1 : 0);
    }

    public static bool GetTimeToggle()
    {
        int isOn = PlayerPrefs.GetInt(timeToggleKey, defaultTimeToggle);
        return isOn == 0 ? false : true;
    }

    public static void SetTimeToggle(bool isOn)
    {
        PlayerPrefs.SetInt(timeToggleKey, isOn ? 1 : 0);
    }

    public static bool GetKeyPressedToggle()
    {
        int isOn = PlayerPrefs.GetInt(keyPressedToggleKey, defaultKeyPressedToggle);
        return isOn == 0 ? false : true;
    }

    public static void SetKeyPressedToggle(bool isOn)
    {
        PlayerPrefs.SetInt(keyPressedToggleKey, isOn ? 1 : 0);
    }
    public static bool GetTutorialToggle()
    {
        int isOn = PlayerPrefs.GetInt(tutorialToggleKey, defaultTutorialToggle);
        return isOn == 0 ? false : true;
    }

    public static void SetTutorialToggle(bool isOn)
    {
        PlayerPrefs.SetInt(tutorialToggleKey, isOn ? 1 : 0);
    }

    public static bool GetGhostToggle()
    {
        int isOn = PlayerPrefs.GetInt(ghostToggleKey, defaultGhostToggle);
        return isOn == 0 ? false : true;
    }

    public static void SetGhostToggle(bool isOn)
    {
        PlayerPrefs.SetInt(ghostToggleKey, isOn ? 1 : 0);
    }

    public static bool GetConsoleToggle()
    {
        int isOn = PlayerPrefs.GetInt(consoleToggleKey, defaultConsoleToggle);
        return isOn == 0 ? false : true;
    }

    public static void SetConsoleToggle(bool isOn)
    {
        PlayerPrefs.SetInt(consoleToggleKey, isOn ? 1 : 0);
    }
}