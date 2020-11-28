using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffectType
{
    Falling, Checkpoint, Collectible, Win, Death, Portal, Land, Jump, Footstep
}

public class PlayerSoundEffects : MonoBehaviour
{
    public static PlayerSoundEffects Instance { get; private set; }

    public AudioSource checkpointAudioSource;
    public AudioSource collectiblePickupAudioSource;
    public AudioSource deathAudioSource;
    public AudioSource footstepAudioSource;
    public AudioSource jumpAudioSource;
    public AudioSource landAudioSource;
    public AudioSource portalAudioSource;
    public AudioSource terminalVelocityAudioSource;
    public AudioSource winAudioSource;

    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        if (PlayerSoundEffects.Instance == null)
        {
            PlayerSoundEffects.Instance = this;
        }
        else if (PlayerSoundEffects.Instance == this)
        {
            Destroy(PlayerSoundEffects.Instance.gameObject);
            PlayerSoundEffects.Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public static void PlaySoundEffect(SoundEffectType type)
    {
        switch (type)
        {
            case SoundEffectType.Falling:
                Instance.terminalVelocityAudioSource.Play();
                break;
            case SoundEffectType.Checkpoint:
                Instance.checkpointAudioSource.Play();
                break;
            case SoundEffectType.Collectible:
                Instance.collectiblePickupAudioSource.Play();
                break;
            case SoundEffectType.Win:
                Instance.winAudioSource.Play();
                break;
            case SoundEffectType.Death:
                Instance.deathAudioSource.Play();
                break;
            case SoundEffectType.Portal:
                Instance.portalAudioSource.Play();
                break;
            case SoundEffectType.Land:
                Instance.landAudioSource.Play();
                break;
            case SoundEffectType.Jump:
                Instance.jumpAudioSource.Play();
                break;
            case SoundEffectType.Footstep:
                Instance.footstepAudioSource.Play();
                break;
        }
    }
}
