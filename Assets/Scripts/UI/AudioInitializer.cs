using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInitializer : MonoBehaviour
{
    public AudioMixer audioMixer;
    private const string masterVolumeParam = "MasterVolume";
    private const string musicVolumeParam = "MusicVolume";
    private const string soundEffectVolumeParam = "SoundEffectVolume";

    void Start()
    {
        InitializeVolume();
    }

    public void SetDefaults()
    {

    }

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
}
