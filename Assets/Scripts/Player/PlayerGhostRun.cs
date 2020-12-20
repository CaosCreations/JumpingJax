﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerGhostRun : MonoBehaviour
{
    public KeyPressed keyPressed;
    public static event Action<Transform, PortalType> onGhostPortalPress;

    public GameObject ghostRunnerPrefab;
	
    private Camera playerCamera;
    private PortalPlacement portalPlacement;
    public Camera ghostCamera;
    public GameObject ghostRunner;
    public PlayerMovement playerMovement;

    private List<Vector3> currentRunPositionData;
    private List<Vector3> currentRunCameraRotationData;
    public List<KeysPressed> currentRunKeyData;

    private Vector3[] pastRunPositionData;
    private Vector3[] pastRunCameraRotationData;
    private KeysPressed[] pastRunKeyData;

    private float ghostRunSaveTimer = 0;
    private float ghostRunnerTimer = 0;
    public Level currentLevel;
    public int currentDataIndex = 0;

    private const int maxDataCount = 25000; //Makes it so max file save is 5MB, stores 20.8 min of Ghost data saved

    private const float ghostRunSaveInterval = 0.0167f;

    private bool usingLeaderboardGhost;

    void Start()
    {
        currentLevel = GameManager.GetCurrentLevel();
        playerMovement = GetComponent<PlayerMovement>();
        SetPastRunData();
        SetupGhostObject();
        
        playerCamera = GetComponent<CameraMove>().playerCamera;
        portalPlacement = GetComponent<PortalPlacement>();
        RestartRun();

        MiscOptions.onGhostToggle += ToggleGhost;
    }

    private void SetPastRunData()
    {
        if (string.IsNullOrEmpty(GameManager.Instance.replayFileLocation))
        {
            Debug.Log("No replay file location set");
            if (currentLevel.levelSaveData.isCompleted)
            {
                Debug.Log("Loading replay data from local files");
                pastRunPositionData = currentLevel.levelSaveData.ghostRunPositions;
                pastRunCameraRotationData = currentLevel.levelSaveData.ghostRunCameraRotations;
                pastRunKeyData = currentLevel.levelSaveData.ghostRunKeys;
            }
        }
        else{
            if (pastRunPositionData == null)
            {
                Debug.Log($"Trying to load leaderboard replay from: {GameManager.Instance.replayFileLocation}");
                if (File.Exists(GameManager.Instance.replayFileLocation))
                {
                    Debug.Log("Found leaderboard replay file");

                    try
                    {
                        string replayLevelData = File.ReadAllText(GameManager.Instance.replayFileLocation);
                        Level replayLevel = ScriptableObject.CreateInstance<Level>();
                        replayLevel.levelSaveData = new PersistentLevelDataModel();
                        JsonUtility.FromJsonOverwrite(replayLevelData, replayLevel.levelSaveData);

                        pastRunPositionData = replayLevel.levelSaveData.ghostRunPositions;
                        pastRunCameraRotationData = replayLevel.levelSaveData.ghostRunCameraRotations;
                        pastRunKeyData = replayLevel.levelSaveData.ghostRunKeys;
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

            ghostCamera = ghostRunner.GetComponentInChildren<Camera>();
            playerMovement.ghostCamera = ghostCamera;
            GetComponent<PortalPlacement>().ghostCamera = ghostCamera;
            ghostCamera.enabled = false;

            ghostRunner.layer = PlayerConstants.GhostLayer;
            Transform[] allChildren = ghostRunner.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                child.gameObject.layer = PlayerConstants.GhostLayer;
            }
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0 || GameManager.GetCurrentLevel().workshopFilePath != string.Empty)
        {
            return;
        }
        
        if (InputManager.GetKeyDown(PlayerConstants.FirstPersonGhost))
        {
            ToggleGhostCamera();
        }
        
        if (ghostCamera.enabled)
        {
            if(pastRunKeyData[currentDataIndex].isMouseLeftPressed)
            {
                Debug.Log("GHOST MOUSE LEFT PRESSED");

                // Firing an event here is unnecessary:

                //onGhostPortalPress?.Invoke(ghostRunner.transform, PortalType.Blue);

                portalPlacement.FirePortal(PortalType.Blue, ghostRunner.transform.position, ghostRunner.transform.forward,
                    PortalPlacement.portalRaycastDistance);

            }
            else if (pastRunKeyData[currentDataIndex].isMouseRightPressed)
            {
                Debug.Log("GHOST MOUSE RIGHT PRESSED");

                //onGhostPortalPress?.Invoke(ghostRunner.transform, PortalType.Pink);

                portalPlacement.FirePortal(PortalType.Pink, ghostRunner.transform.position, ghostRunner.transform.forward,
                    PortalPlacement.portalRaycastDistance);

            }
        }

        RecordCurrentRunData();
        UpdateGhost();
    }

    public void RestartRun()
    {
        SetPastRunData();
        ghostRunner.SetActive(ShouldGhostBeActive());
        ghostRunSaveTimer = 0;
        ghostRunnerTimer = 0;
        currentDataIndex = 0;
        currentRunPositionData = new List<Vector3>();
        currentRunCameraRotationData = new List<Vector3>(); 
        currentRunKeyData = new List<KeysPressed>();
    }

    private void UpdateGhost()
    {
        if (pastRunPositionData == null)
        {
            return; 
        }

        if (currentDataIndex >= pastRunPositionData.Length - 1)
        {
            currentDataIndex = 0;
        }

        float lerpValue = ghostRunnerTimer / ghostRunSaveInterval;
        Vector3 position = Vector3.Lerp(ghostRunner.transform.position, pastRunPositionData[currentDataIndex], lerpValue);
        ghostRunner.transform.position = position;
        if (ghostCamera.enabled)
        {
            ghostRunner.transform.eulerAngles = Vector3.zero;
            ghostCamera.transform.eulerAngles = pastRunCameraRotationData[currentDataIndex];
        }
        else
        {
            Vector3 rotation = Vector3.Lerp(ghostRunner.transform.eulerAngles, pastRunCameraRotationData[currentDataIndex], lerpValue);
            ghostRunner.transform.eulerAngles = new Vector3(0f, rotation.y, 0f);
        }

        keyPressed.SetPressed(pastRunKeyData[currentDataIndex]);

        ghostRunnerTimer += Time.deltaTime;
        if (ghostRunnerTimer >= ghostRunSaveInterval)
        {
            currentDataIndex++;
            ghostRunnerTimer = 0;
        }
    }

    private void RecordCurrentRunData()
    {
        ghostRunSaveTimer += Time.deltaTime;
        if (ghostRunSaveTimer > ghostRunSaveInterval && currentRunPositionData.Count < maxDataCount)
        {
            ghostRunSaveTimer = 0;
            currentRunPositionData.Add(transform.position);
            currentRunCameraRotationData.Add(playerCamera.transform.eulerAngles);
            currentRunKeyData.Add(GetCurrentKeysPressed());
        }
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
        }
    }

    private void ToggleGhost(bool isOn)
    {
        OptionsPreferencesManager.SetGhostToggle(isOn);
        ghostRunner.SetActive(isOn && ShouldGhostBeActive());
    }

    private void ToggleGhostCamera()
    {
        ghostCamera.enabled = !ghostCamera.enabled;
        playerCamera.enabled = !playerCamera.enabled;
    }

    private bool ShouldGhostBeActive()
    {
        return pastRunPositionData != null && pastRunPositionData.Length > 0;
    }
}
