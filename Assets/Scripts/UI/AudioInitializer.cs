using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInitializer : MonoBehaviour
{
    public AudioMixer audioMixer;
    private const string musicVolumeParam = "MusicVolume";
    private const string soundEffectVolumeParam = "SoundEffectVolume";

    void Start()
    {
        InitializeVolume();
    }

    public void SetDefaults()
    {

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
}
