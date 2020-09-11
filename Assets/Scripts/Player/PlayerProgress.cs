using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    [SerializeField]
    public Checkpoint currentCheckpoint;

    public PlayerUI playerUI;

    public PlayerMovement playerMovement;
    public CameraMove cameraMove;
    public PlayerGhostRun playerGhostRun;
    public Crosshair crosshair;

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
            if(checkPointHit.level == 1)
            {
                firstCheckpoint = checkPointHit;
            }
            HitNewCheckPoint(checkPointHit);
        }
    }

    public void HitNewCheckPoint(Checkpoint checkpoint)
    {
        if (currentCheckpoint == null)
        {
            checkpoint.SetCompleted();
            currentCheckpoint = checkpoint;
        }
        else
        {
            if (currentCheckpoint.level <= checkpoint.level)
            {
                checkpoint.SetCompleted();
                currentCheckpoint = checkpoint;

                if (currentCheckpoint.level == GameManager.GetCurrentLevel().numberOfCheckpoints)
                {
                    playerGhostRun.SaveCurrentRunData();
                    GameManager.FinishedLevel();
                    playerUI.ShowWinScreen();
                    Time.timeScale = 0;
                }
            }
        }
    }

    public void Respawn()
    {
        Vector3 respawnPosition = currentCheckpoint.transform.position + PlayerConstants.PlayerSpawnOffset;
        transform.position = respawnPosition;

        Vector3 respawnRotation = new Vector3();
        respawnRotation.y = currentCheckpoint.transform.rotation.eulerAngles.y;
        cameraMove.ResetTargetRotation(Quaternion.Euler(respawnRotation));

        playerMovement.newVelocity = Vector3.zero;


        // If the player is restarting at the beginning, reset timer
        if (currentCheckpoint.level == 1)
        {
            crosshair.Init();
            if(portalPair != null)
            {
                portalPair.ResetPortals();
            }
            GameManager.Instance.currentCompletionTime = 0;
            playerGhostRun.RestartRun();
        }
    }

    public void ResetPlayer()
    {
        playerUI.ToggleOffWinScreen();
        currentCheckpoint = firstCheckpoint;
        Respawn();
        ResetCheckpoints();
        GameManager.RestartLevel();
    }

    private void ResetCheckpoints()
    {
        Checkpoint[] checkpoints = GameObject.FindObjectsOfType<Checkpoint>();
        foreach(Checkpoint checkpoint in checkpoints)
        {
            checkpoint.SetUncompleted();
        }
    }
}