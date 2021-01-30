using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionsInitializer : MonoBehaviour
{
    public AudioMixer audioMixer;
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
        Screen.SetResolution(savedHeight, savedWidth, fullScreen);

        QualitySettings.SetQualityLevel(OptionsPreferencesManager.GetQuality());
    }

    #region Audio
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
        OptionsPreferencesManager.SetMusicVolume(volumeInDecibels);
    }

    public void InitializeVolume()
    {
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
