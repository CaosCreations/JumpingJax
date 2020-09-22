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

    private PlayerMovement playerMovement;
    private CameraMove cameraMove;
    private PlayerGhostRun playerGhostRun;
    private Crosshair crosshair;
    private PortalPair portalPair;
    private Checkpoint firstCheckpoint;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        cameraMove = GetComponent<CameraMove>();
        playerGhostRun = GetComponent<PlayerGhostRun>();
        crosshair = GetComponent<Crosshair>();
        portalPair = GameObject.FindObjectOfType<PortalPair>();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (InputManager.GetKeyDown(PlayerConstants.ResetLevel))
        {
            Respawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Checkpoint checkPointHit = other.gameObject.GetComponent<Checkpoint>();
        if (checkPointHit)
        {
            if(firstCheckpoint == null)
            {
                firstCheckpoint = checkPointHit;
            }
            HitNewCheckPoint(checkPointHit);
        }
    }

    public void HitNewCheckPoint(Checkpoint checkpoint)
    {
        checkpoint.SetCompleted();
        currentCheckpoint = checkpoint;

        if (currentCheckpoint.isFinalCheckpoint)
        {
            playerGhostRun.SaveCurrentRunData();
            GameManager.FinishedLevel();
            playerUI.ShowWinScreen();
            Time.timeScale = 0;
        }
    }

    public void Respawn()
    {
        // Prevent a crash if the player loads into a workshop level and never touches a checkpoint
        if (currentCheckpoint == null)
        {
            return;
        }
        Vector3 respawnPosition = currentCheckpoint.transform.position + PlayerConstants.PlayerSpawnOffset;
        transform.position = respawnPosition;

        Vector3 respawnRotation = new Vector3();
        respawnRotation.y = currentCheckpoint.transform.rotation.eulerAngles.y;
        cameraMove.ResetTargetRotation(Quaternion.Euler(respawnRotation));

        playerMovement.newVelocity = Vector3.zero;

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
            ResetCheckpoints();
        }
    }

    public void ResetPlayer()
    {
        playerUI.ToggleOffWinScreen();
        currentCheckpoint = firstCheckpoint;
        Respawn();
        
        GameManager.RestartLevel();
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
    }

    private void ResetCollectibles()
    {
        CollectibleHandler[] collectibles = GameObject.FindObjectsOfType<CollectibleHandler>();
        foreach (CollectibleHandler handler in collectibles)
        {
            handler.ResetActive();
        }
    }
}