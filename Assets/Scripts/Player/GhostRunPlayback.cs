using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Steamworks;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

public class GhostRunPlayback : MonoBehaviour
{
    [Header("Set in Editor")]
    public KeyPressed keyPressed;
    public GameObject ghostRunnerPrefab;
    public Camera portalCamera;

    [Header("Set at RUNTIME")]
    public GhostPortalCamera ghostRunRecursivePortalCamera;
    public Camera ghostCamera;

    public GameObject ghostRunner;
    public PlayerMovement playerMovement;
    public PlayerProgress playerProgress;
    public InGameUI inGameUI;

    private Camera playerCamera;
    private GhostPortalPlacement ghostPortalPlacement;
    private PortalPlacement portalPlacement;

    private RecordedFrame[] replayFrames;
    public string pastRunPlayerSteamName;

    public Level currentLevel;
    public int frameIndex = 0;



    private void Start()
    {
        playerProgress = GetComponent<PlayerProgress>();
        currentLevel = GameManager.GetCurrentLevel();
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<CameraMove>().playerCamera;
        portalPlacement = GetComponent<PortalPlacement>();
        inGameUI = GetComponentInChildren<InGameUI>(true);
        keyPressed = GetComponentInChildren<KeyPressed>(true);

        SetPastRunData();
        SetupGhostObject();
        RestartRun();

        MiscOptions.onGhostToggle += ToggleGhost;
    }

    private void SetPastRunData()
    {
        if(replayFrames == null && !string.IsNullOrEmpty(GameManager.Instance.ReplayFileLocation))
        {
            GetNewRunData();
        }
    }

    public void GetNewRunData()
    {
        while (AsyncTaskReporter.Instance.ghostDownloadRunning)
        {
            Thread.Sleep(20);
        }

        Debug.Log($"Trying to load leaderboard replay from: {GameManager.Instance.ReplayFileLocation}");
        if (File.Exists(GameManager.Instance.ReplayFileLocation))
        {
            try
            {
                string replayLevelData = File.ReadAllText(GameManager.Instance.ReplayFileLocation);
                PersistentLevelDataModel levelSaveData = new PersistentLevelDataModel();
                JsonUtility.FromJsonOverwrite(replayLevelData, levelSaveData);

                replayFrames = levelSaveData.recordedFrames;
                pastRunPlayerSteamName = levelSaveData.ghostRunPlayerName;
            }
            catch (Exception e)
            {
                Debug.LogError($"{e.Message}\n{e.StackTrace}");
            }
        }
        else
        {
            Debug.Log("no file found for ghost run");
        }
    }

    void SetupGhostObject()
    {
        if (ghostRunner == null)
        {
            ghostRunner = Instantiate(ghostRunnerPrefab);
            ghostRunner.name = "ghost runner";

            ghostRunRecursivePortalCamera = ghostRunner.GetComponentInChildren<GhostPortalCamera>(true);

            ghostCamera = ghostRunRecursivePortalCamera.myCamera;

            playerMovement.ghostCamera = ghostCamera;
            portalPlacement.ghostCamera = ghostCamera;

            ghostPortalPlacement = ghostRunner.GetComponent<GhostPortalPlacement>();

            ghostCamera.enabled = false;

            ghostRunner.layer = PlayerConstants.GhostLayer;
            Transform[] allChildren = ghostRunner.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                child.gameObject.layer = PlayerConstants.GhostLayer;
            }
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        // if no replay frames, don't do anything
        if (replayFrames == null || replayFrames.Length == 0)
        {
            return;
        }

        // only run the ghost if the player has moved, or they are spectating
        if (!GameManager.Instance.hasMoved && !ghostCamera.enabled)
        {
            return;
        }

        if (InputManager.GetKeyDown(PlayerConstants.FirstPersonGhost) && ShouldGhostBeActive())
        {
            ToggleGhostCamera();
        }

        UpdateGhost();
    }

    private void UpdateGhost()
    {
        if (frameIndex >= replayFrames.Length - 1)
        {
            frameIndex = 0;

            if (ghostCamera.enabled)
            {
                GameManager.Instance.currentCompletionTime = 0;

                if (ghostPortalPlacement.portalPair != null)
                {
                    ghostPortalPlacement.portalPair.ResetPortals();
                }
            }
        }

        var currentFrame = replayFrames[frameIndex];

        ApplyCurrentFrame(currentFrame);
    }

    private void ApplyCurrentFrame(RecordedFrame currentFrame)
    {
        if (ghostCamera.enabled)
        {
            if (currentFrame.keysPressed.isMouseLeftPressed && ghostPortalPlacement.portalPair != null)
            {
                ghostPortalPlacement.FirePortal(PortalType.Blue, ghostCamera.transform.position, ghostCamera.transform.forward,
                    PlayerConstants.PortalRaycastDistance, ghostCamera.transform);
            }
            else if (currentFrame.keysPressed.isMouseRightPressed && ghostPortalPlacement.portalPair != null)
            {
                ghostPortalPlacement.FirePortal(PortalType.Pink, ghostCamera.transform.position, ghostCamera.transform.forward,
                    PlayerConstants.PortalRaycastDistance, ghostCamera.transform);
            }
        }

        ghostRunner.transform.position = currentFrame.position;

        if (ghostCamera.enabled)
        {
            ghostCamera.transform.eulerAngles = currentFrame.cameraRotation;
            inGameUI.currentSpeed = currentFrame.velocity;
            keyPressed.SetPressed(currentFrame.keysPressed);
        }
        else
        {
            ghostRunner.transform.eulerAngles = new Vector3(0f, currentFrame.cameraRotation.y, 0f);
        }
    }

    public void RestartRun()
    {
        ghostRunner.SetActive(ShouldGhostBeActive());
        frameIndex = 0;
    }

    private void ToggleGhost(bool isOn)
    {
        OptionsPreferencesManager.SetGhostToggle(isOn);
        ghostRunner.SetActive(ShouldGhostBeActive());
    }

    private void ToggleGhostCamera()
    {
        ghostCamera.enabled = !ghostCamera.enabled;
        ghostRunRecursivePortalCamera.enabled = ghostCamera.enabled;
        playerCamera.enabled = !playerCamera.enabled;

        if (ghostCamera.enabled)
        {
            ghostCamera.fieldOfView = OptionsPreferencesManager.GetCameraFOV();
            inGameUI.IsGhosting = true;
        } else
        {
            inGameUI.IsGhosting = false;
        }

        inGameUI.ToggleGhostUI();
        playerProgress.ResetPlayer();
    }

    private bool ShouldGhostBeActive()
    {
        return replayFrames != null && replayFrames.Length > 0 && OptionsPreferencesManager.GetGhostToggle();
    }

    public void ClearPastRunData()
    {
        replayFrames = new RecordedFrame[0];
        pastRunPlayerSteamName = string.Empty;
    }
}
