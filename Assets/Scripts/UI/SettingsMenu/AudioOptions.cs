using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioOptions : MonoBehaviour
{
    public AudioMixer audioMixer;
    public GameObject sliderPrefab;
    public Transform scrollViewContent;
    private SliderItem musicVolumeSlider;
    private SliderItem soundEffectVolumeSlider;

    private const string musicVolumeParam = "MusicVolume";
    private const string soundEffectVolumeParam = "SoundEffectVolume";

    void Start()
    {
        InitializeVolume();
    }

    public void SetDefaults()
    {
        OptionsPreferencesManager.SetMusicVolume(OptionsPreferencesManager.defaultMusicVolume);
        musicVolumeSlider.slider.value = ConvertFromDecibel(OptionsPreferencesManager.defaultMusicVolume);

        OptionsPreferencesManager.SetSoundEffectVolume(OptionsPreferencesManager.defaultSoundEffectVolume);
        musicVolumeSlider.slider.value = ConvertFromDecibel(OptionsPreferencesManager.defaultSoundEffectVolume);
    }

    public void SetMusicVolume(float value)
    {
        musicVolumeSlider.input.text = (int)(value * 100) + "%";

        float volumeInDecibels = ConvertToDecibel(value);
        audioMixer.SetFloat(musicVolumeParam, volumeInDecibels);
        OptionsPreferencesManager.SetMusicVolume(volumeInDecibels);
    }

    public void SetSoundEffectVolume(float value)
    {
        soundEffectVolumeSlider.input.text = (int)(value * 100) + "%";

        float volumeInDecibels = ConvertToDecibel(value);
        audioMixer.SetFloat(soundEffectVolumeParam, volumeInDecibels);
        OptionsPreferencesManager.SetMusicVolume(volumeInDecibels);
    }

    public void InitializeVolume()
    {
        if(musicVolumeSlider == null)
        {
            GameObject newSlider = Instantiate(sliderPrefab, scrollViewContent);
            musicVolumeSlider = newSlider.GetComponent<SliderItem>();
        }
        musicVolumeSlider.Init("Music Volume", ConvertFromDecibel(OptionsPreferencesManager.GetMusicVolume()), SetMusicVolume, 0.0001f, 1, false, PlayerConstants.MusicVolumeTooltip);
        musicVolumeSlider.input.text = (int) (ConvertFromDecibel(OptionsPreferencesManager.GetMusicVolume()) * 100) + "%";

        if (soundEffectVolumeSlider == null)
        {
            GameObject newSlider = Instantiate(sliderPrefab, scrollViewContent);
            soundEffectVolumeSlider = newSlider.GetComponent<SliderItem>();
        }
        soundEffectVolumeSlider.Init("Sound Effect Volume", ConvertFromDecibel(OptionsPreferencesManager.GetSoundEffectVolume()), SetSoundEffectVolume, 0.0001f, 1, false, PlayerConstants.SoundEffectVolumeTooltip);
        soundEffectVolumeSlider.input.text = (int)(ConvertFromDecibel(OptionsPreferencesManager.GetSoundEffectVolume()) * 100) + "%";
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
