using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClipContainer audioClips;
    public AudioSource audiosource;
    private List<AudioClip> availableClips;

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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
