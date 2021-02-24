using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    [Header("Set in editor")]
    public PlayerUI playerUI;

    [Header("Debugging properties")]
    [SerializeField]
    public Checkpoint currentCheckpoint;
    public int Deaths { get; private set; }

    private PlayerMovement playerMovement;
    private CameraMove cameraMove;
    private PlayerGhostRun playerGhostRun;
    private Crosshair crosshair;
    private PortalPair portalPair;
    private Checkpoint firstCheckpoint;
    private TutorialTriggerGroup tutorialTriggerGroup;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        cameraMove = GetComponent<CameraMove>();
        playerGhostRun = GetComponent<PlayerGhostRun>();
        crosshair = GetComponent<Crosshair>();
        portalPair = FindObjectOfType<PortalPair>();
        tutorialTriggerGroup = GetComponent<TutorialTriggerGroup>();
        Deaths = 0;
        GetFirstCheckpoint();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (InputManager.GetKeyDown(PlayerConstants.Respawn))
        {
            Respawn();
        }

        if (InputManager.GetKeyDown(PlayerConstants.ResetLevel))
        {
            ResetPlayer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Checkpoint checkPointHit = other.gameObject.GetComponent<Checkpoint>();
        if (checkPointHit)
        {
            HitNewCheckPoint(checkPointHit);
        }
    }

    private void GetFirstCheckpoint()
    {
        Checkpoint[] allCheckpoints = FindObjectsOfType<Checkpoint>();
        foreach(Checkpoint checkpoint in allCheckpoints)
        {
            if (checkpoint.isFirstCheckpoint)
            {
                firstCheckpoint = checkpoint;
            }
        }
    }

    public void HitNewCheckPoint(Checkpoint checkpoint)
    {
        // Only play the sound on the first time touching the checkpoint, and don't play the sound if it's the final checkpoint as other sounds may play then
        if (!checkpoint.isCompleted && !checkpoint.isFinalCheckpoint)
        {
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Checkpoint);
        }
        checkpoint.SetCompleted();
        currentCheckpoint = checkpoint;

        if (currentCheckpoint.isFinalCheckpoint)
        {
            playerGhostRun.SaveCurrentRunData();
            GameManager.FinishedLevel();
            playerUI.ShowWinScreen();
            Time.timeScale = 0;
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Win);
        }
    }

    public void Respawn()
    {
        // Prevent a crash if the player loads into a workshop level and never touches a checkpoint
        if (currentCheckpoint == null)
        {
            return;
        }
        playerMovement.controller.enabled = false;
        Vector3 respawnPosition = currentCheckpoint.transform.position + PlayerConstants.PlayerSpawnOffset;
        transform.position = respawnPosition;

        Vector3 respawnRotation = new Vector3();
        respawnRotation.y = currentCheckpoint.transform.rotation.eulerAngles.y;
        cameraMove.ResetTargetRotation(Quaternion.Euler(respawnRotation));

        playerMovement.velocityToApply = Vector3.zero;
        playerMovement.controller.enabled = true;
        ++Deaths;

        PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Respawn);

        // If the player is restarting at the beginning, reset level
        if (currentCheckpoint.isFirstCheckpoint)
        {
            crosshair.Init();
            if(portalPair != null)
            {
                portalPair.ResetPortals();
            }
            GameManager.RestartLevel();
            playerGhostRun.RestartRun();
            ResetCollectibles();
            ResetTutorials();
        }
    }

    public void ResetPlayer()
    {
        playerUI.ToggleOffWinScreen();
        currentCheckpoint = firstCheckpoint;
        ResetCheckpoints();
        Respawn();
    }

    private void ResetCheckpoints()
    {
        Checkpoint[] checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
        foreach(Checkpoint checkpoint in checkpoints)
        {
            checkpoint.SetUncompleted();
            BoxCollider collider = checkpoint.GetComponent<BoxCollider>();
            collider.enabled = false;
            collider.enabled = true;
        }
        Deaths = 0;
    }

    private void ResetCollectibles()
    {
        CollectibleHandler[] collectibles = GameObject.FindObjectsOfType<CollectibleHandler>();
        foreach (CollectibleHandler handler in collectibles)
        {
            handler.ResetActive();
        }
    }

    private void ResetTutorials()
    {
        tutorialTriggerGroup.ResetTriggers();
    }
}