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

    public float footstepTimer;
    public float footstepInverval;

    public AudioClip footstep1;
    public AudioClip footstep2;
    public bool lastFootstep1;

    void Awake()
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

        Instance.footstepInverval = 0.5f;
    }

    private void Update()
    {
        Instance.footstepTimer += Time.deltaTime;
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
                if(Instance.footstepTimer > Instance.footstepInverval)
                {
                    Instance.footstepTimer = 0;
                    if (Instance.lastFootstep1)
                    {
                        Instance.footstepAudioSource.clip = Instance.footstep1;
                        Instance.lastFootstep1 = false;
                    }
                    else
                    {
                        Instance.footstepAudioSource.clip = Instance.footstep2;
                        Instance.lastFootstep1 = true;
                    }
                    Instance.footstepAudioSource.Play();
                }
                break;
        }
    }
}
