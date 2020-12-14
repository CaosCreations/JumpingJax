using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioClipContainer audioClips;
    public AudioSource audiosource;
    private List<AudioClip> availableClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        availableClips = audioClips.audioClips.ToList();
        audiosource.loop = false;
    }

    void Update()
    {
        if (!audiosource.isPlaying)
        {
            audiosource.clip = GetRandomClip();
            audiosource.Play();
        }
    }

    private AudioClip GetRandomClip()
    {
        if(availableClips.Count == 0)
        {
            availableClips = audioClips.audioClips.ToList();
        }

        int clipIndex = Random.Range(0, availableClips.Count);
        AudioClip clip = availableClips[clipIndex];
        availableClips.RemoveAt(clipIndex);
        return clip;
    }
}
