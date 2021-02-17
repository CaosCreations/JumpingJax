using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsInitializer : MonoBehaviour
{
    public AudioMixer audioMixer;
    private const string masterVolumeParam = "MasterVolume";
    private const string musicVolumeParam = "MusicVolume";
    private const string soundEffectVolumeParam = "SoundEffectVolume";

    void Start()
    {
        InitializeVolume();
        InitializeVideoOptions();
    }

    void InitializeVideoOptions()
    {
        int savedHeight = OptionsPreferencesManager.GetResolutionHeight();
        int savedWidth = OptionsPreferencesManager.GetResolutionWidth();
        bool fullScreen = OptionsPreferencesManager.GetFullScreen();
        Screen.SetResolution(savedWidth, savedHeight, fullScreen);

        QualitySettings.SetQualityLevel(OptionsPreferencesManager.GetQuality());
        QualitySettings.vSyncCount = OptionsPreferencesManager.GetVsync();
    }

    #region Audio
    public void SetMasterVolume(float volume)
    {
        float volumeInDecibels = ConvertToDecibel(volume);
        audioMixer.SetFloat(masterVolumeParam, volumeInDecibels);
        OptionsPreferencesManager.SetMasterVolume(volumeInDecibels);
    }

    public void SetMusicVolume(float volume)
    {
        float volumeInDecibels = ConvertToDecibel(volume);
        audioMixer.SetFloat(musicVolumeParam, volumeInDecibels);
        OptionsPreferencesManager.SetMusicVolume(volumeInDecibels);
    }

    public void SetSoundEffectVolume(float volume)
    {
        float volumeInDecibels = ConvertToDecibel(volume);
        audioMixer.SetFloat(soundEffectVolumeParam, volumeInDecibels);
        OptionsPreferencesManager.SetSoundEffectVolume(volumeInDecibels);
    }

    public void InitializeVolume()
    {
        float masterVolume = OptionsPreferencesManager.GetMasterVolume();
        SetMasterVolume(ConvertFromDecibel(masterVolume));

        float musicVolume = OptionsPreferencesManager.GetMusicVolume();
        SetMusicVolume(ConvertFromDecibel(musicVolume));

        float soundEffectVolume = OptionsPreferencesManager.GetSoundEffectVolume();
        SetSoundEffectVolume(ConvertFromDecibel(soundEffectVolume));
    }

    public float ConvertToDecibel(float value)
    {
        return Mathf.Log10(value) * 20;
    }

    public float ConvertFromDecibel(float value)
    {
        return Mathf.Pow(10, value / 20);
    }
    #endregion
}
