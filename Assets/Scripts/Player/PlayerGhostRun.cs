using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Steamworks;

public class PlayerGhostRun : MonoBehaviour
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

    private List<Vector3> currentRunPositionData;
    private List<Vector3> currentRunCameraRotationData;
    public List<KeysPressed> currentRunKeyData;
    private List<float> currentRunVelocityData;

    private Vector3[] pastRunPositionData;
    private Vector3[] pastRunCameraRotationData;
    private KeysPressed[] pastRunKeyData;
    private float[] pastRunVelocityData;
    public string pastRunPlayerSteamName;

    private float ghostRunnerTimer = 0;
    public Level currentLevel;
    public int currentDataIndex = 0;

    private const int maxDataCount = 25000; //Makes it so max file save is 5MB, stores 20.8 min of Ghost data saved

    private const float ghostRunSaveInterval = 0.01667f;


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
        if(pastRunPositionData == null)
        {
            if (string.IsNullOrEmpty(GameManager.Instance.ReplayFileLocation) && currentLevel.levelSaveData != null && currentLevel.levelSaveData.isCompleted)
            {
                Debug.Log($"Loading replay data from local files for {currentLevel.levelName}. From: {FilePathUtil.GetLevelDataFilePath(currentLevel.levelName)}");
                pastRunPositionData = currentLevel.levelSaveData.ghostRunPositions;
                pastRunCameraRotationData = currentLevel.levelSaveData.ghostRunCameraRotations;
                pastRunKeyData = currentLevel.levelSaveData.ghostRunKeys;
                pastRunVelocityData = currentLevel.levelSaveData.ghostRunVelocities;

                if (SteamClient.IsValid)
                {
                    pastRunPlayerSteamName = SteamClient.Name;
                }
                else
                {
                    pastRunPlayerSteamName = currentLevel.levelSaveData.ghostRunPlayerName;
                }
            }
            else if (!string.IsNullOrEmpty(GameManager.Instance.ReplayFileLocation))
            {
                Debug.Log($"Trying to load leaderboard replay from: {GameManager.Instance.ReplayFileLocation}");
                if (File.Exists(GameManager.Instance.ReplayFileLocation))
                {
                    try
                    {
                        string replayLevelData = File.ReadAllText(GameManager.Instance.ReplayFileLocation);
                        PersistentLevelDataModel levelSaveData = new PersistentLevelDataModel();
                        JsonUtility.FromJsonOverwrite(replayLevelData, levelSaveData);

                        pastRunPositionData = levelSaveData.ghostRunPositions;
                        pastRunCameraRotationData = levelSaveData.ghostRunCameraRotations;
                        pastRunKeyData = levelSaveData.ghostRunKeys;
                        pastRunVelocityData = levelSaveData.ghostRunVelocities;
                        pastRunPlayerSteamName = levelSaveData.ghostRunPlayerName;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"{e.Message}\n{e.StackTrace}");
                    }
                }
            }
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

        if (InputManager.GetKeyDown(PlayerConstants.FirstPersonGhost) && ShouldGhostBeActive())
        {
            ToggleGhostCamera();
        }
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (ghostCamera.enabled)
        {
            if (pastRunKeyData[currentDataIndex].isMouseLeftPressed && ghostPortalPlacement.portalPair != null)
            {
                ghostPortalPlacement.FirePortal(PortalType.Blue, ghostCamera.transform.position, ghostCamera.transform.forward,
                    PlayerConstants.PortalRaycastDistance, ghostCamera.transform);
            }
            else if (pastRunKeyData[currentDataIndex].isMouseRightPressed && ghostPortalPlacement.portalPair != null)
            {
                ghostPortalPlacement.FirePortal(PortalType.Pink, ghostCamera.transform.position, ghostCamera.transform.forward,
                    PlayerConstants.PortalRaycastDistance, ghostCamera.transform);
            }
        }
        
        UpdateGhost();

        ghostRunnerTimer += Time.deltaTime;

        if (ghostRunnerTimer >= ghostRunSaveInterval)
        {
            if (!ghostCamera.enabled)
            {
                // No need to record current run data while spectating, you get reset while watching anyways
                RecordCurrentRunData();
            }

            currentDataIndex++;
            ghostRunnerTimer = 0;
        }
    }

    private void UpdateGhost()
    {
        if (pastRunPositionData == null || pastRunPositionData.Length == 0)
        {
            return; 
        }

        if (currentDataIndex >= pastRunPositionData.Length - 1)
        {
            currentDataIndex = 0;

            if (ghostCamera.enabled && ghostPortalPlacement.portalPair != null)
            {
                ghostPortalPlacement.portalPair.ResetPortals();
            }

            GameManager.Instance.currentCompletionTime = 0;
        }

        ghostRunner.transform.position = pastRunPositionData[currentDataIndex];
        if (ghostCamera.enabled)
        {
            ghostCamera.transform.eulerAngles = pastRunCameraRotationData[currentDataIndex];
            if (pastRunVelocityData != null && pastRunVelocityData.Length >= currentDataIndex)
            {
                inGameUI.currentSpeed = pastRunVelocityData[currentDataIndex];
            }
            else
            {
                inGameUI.currentSpeed = 0;
            }
            
            keyPressed.SetPressed(pastRunKeyData[currentDataIndex]);
        }
        else
        {
            ghostRunner.transform.eulerAngles = new Vector3(0f, pastRunCameraRotationData[currentDataIndex].y, 0f);
        }
    }

    private void RecordCurrentRunData()
    {
        if (currentRunPositionData.Count < maxDataCount)
        {
            currentRunPositionData.Add(transform.position);
            currentRunCameraRotationData.Add(playerCamera.transform.eulerAngles);
            currentRunKeyData.Add(GetCurrentKeysPressed());
            currentRunVelocityData.Add(new Vector2(playerMovement.currentVelocity.x, playerMovement.currentVelocity.z).magnitude);
        }
    }

    public void RestartRun()
    {
        SetPastRunData();
        ghostRunner.SetActive(ShouldGhostBeActive());
        ghostRunnerTimer = 0;
        currentDataIndex = 0;
        currentRunPositionData = new List<Vector3>();
        currentRunCameraRotationData = new List<Vector3>();
        currentRunKeyData = new List<KeysPressed>();
        currentRunVelocityData = new List<float>();
    }

    private KeysPressed GetCurrentKeysPressed()
    {
        KeysPressed toReturn = new KeysPressed()
        {
            isForwardPressed = InputManager.GetKey(PlayerConstants.Forward),
            isLeftPressed = InputManager.GetKey(PlayerConstants.Left),
            isRightPressed = InputManager.GetKey(PlayerConstants.Right),
            isBackPressed = InputManager.GetKey(PlayerConstants.Back),
            isJumpPressed = InputManager.GetKey(PlayerConstants.Jump),
            isCrouchPressed = InputManager.GetKey(PlayerConstants.Crouch),
            isMouseLeftPressed = InputManager.GetKey(PlayerConstants.Portal1),
            isMouseRightPressed = InputManager.GetKey(PlayerConstants.Portal2)
        };	

        return toReturn;
    }

    public void SaveCurrentRunData()
    {
        if(currentLevel.levelSaveData.completionTime > GameManager.Instance.currentCompletionTime || currentLevel.levelSaveData.completionTime == 0)
        {
            currentLevel.levelSaveData.ghostRunPositions = currentRunPositionData.ToArray();
            currentLevel.levelSaveData.ghostRunCameraRotations = currentRunCameraRotationData.ToArray(); 
            currentLevel.levelSaveData.ghostRunKeys = currentRunKeyData.ToArray();
            currentLevel.levelSaveData.ghostRunVelocities = currentRunVelocityData.ToArray();
            if (SteamClient.IsValid)
            {
                currentLevel.levelSaveData.ghostRunPlayerName = SteamClient.Name;
            } else
            {
                currentLevel.levelSaveData.ghostRunPlayerName = "Yourself"; //so that local replay will say "spectatiing: yourself"
            }
        }
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
        return pastRunPositionData != null && pastRunPositionData.Length > 0 && OptionsPreferencesManager.GetGhostToggle();
    }
}
