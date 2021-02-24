using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffectType
{
    Checkpoint, Collectible, Death, Respawn, Win,
    Footstep, Land, TerminalVelocity, 
    PortalOpen, PortalIdle, PortalRejected, PortalPassThrough
}

public class PlayerSoundEffects : MonoBehaviour
{
    public static PlayerSoundEffects Instance { get; private set; }

    // Gameplay
    public AudioSource checkpointAudioSource;
    public AudioSource collectiblePickupAudioSource;
    public AudioSource deathAudioSource;
    public AudioSource respawnAudioSource;
    public AudioSource winAudioSource;

    // Movement
    public AudioSource footstepAudioSource;
    public AudioSource landAudioSource;
    public AudioSource terminalVelocityAudioSource;

    // Portal
    public AudioSource portalOpenAudioSource;
    public AudioSource portalRejectedAudioSource;
    public AudioSource portalPassThroughAudioSource;

    // Timers
    float footstepTimer;
    float footstepInverval = 0.6f;

    float landingTimer;
    float landingInterval = 0.5f;

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
        Instance.landingTimer += Time.deltaTime;
    }

    public static void PlaySoundEffect(SoundEffectType type)
    {
        switch (type)
        {
            // Gameplay
            case SoundEffectType.Checkpoint:
                Instance.checkpointAudioSource.Play();
                break;
            case SoundEffectType.Collectible:
                Instance.collectiblePickupAudioSource.Play();
                break;
            case SoundEffectType.Death:
                Instance.deathAudioSource.Play();
                break;
            case SoundEffectType.Respawn:
                Instance.respawnAudioSource.Play();
                break;
            case SoundEffectType.Win:
                Instance.winAudioSource.Play();
                break;
            
            // Movement
            case SoundEffectType.Footstep:
                HandleFootstepAudio();
                break;
            case SoundEffectType.Land:
                HandleLandingAudio();
                break;
            case SoundEffectType.TerminalVelocity:
                HandleTerminalVelocityAudio();
                break;

            // Portal
            case SoundEffectType.PortalOpen:
                Instance.portalOpenAudioSource.Play();
                break;
            case SoundEffectType.PortalRejected:
                Instance.portalRejectedAudioSource.Play();
                break;
            case SoundEffectType.PortalPassThrough:
                Instance.portalPassThroughAudioSource.Play();
                break;

        }
    }


    public static void HandleFootstepAudio()
    {
        if (Instance.footstepTimer > Instance.footstepInverval)
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
    }

    public static void HandleLandingAudio()
    {
        if (Instance.landingTimer > Instance.landingInterval)
        {
            Instance.landingTimer = 0;
            Instance.landAudioSource.Play();
        }
    }

    public static void HandleTerminalVelocityAudio()
    {
        Instance.terminalVelocityAudioSource.Play();
    }
}
