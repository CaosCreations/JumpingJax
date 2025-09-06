using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class RunRecorder : MonoBehaviour
{
    private List<RecordedFrame> recordedFrames = new List<RecordedFrame>();

    private GhostRunPlayback ghostRunPlayback;
    private PlayerMovement playerMovement;
    private Camera playerCamera;

    public Level currentLevel;
    private float timer = 0;
    private const int maxDataCount = 25000; //Makes it so max file save is 5MB, stores 20.8 min of Ghost data saved
    private const float ghostRunSaveInterval = 0.01667f;

    void Start()
    {
        ghostRunPlayback = GetComponent<GhostRunPlayback>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<CameraMove>().playerCamera;
        currentLevel = GameManager.GetCurrentLevel();
    }

    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        // only run the ghost if the player has moved, or they are spectating
        if (!GameManager.Instance.hasMoved && !ghostRunPlayback.ghostCamera.enabled)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= ghostRunSaveInterval)
        {
            RecordCurrentRunData();

            timer = 0;
        }
    }

    private void RecordCurrentRunData()
    {
        if (recordedFrames.Count < maxDataCount)
        {
            recordedFrames.Add(new RecordedFrame()
            {
                position = transform.position,
                cameraRotation = playerCamera.transform.eulerAngles,
                keysPressed = GetCurrentKeysPressed(),
                velocity = new Vector2(playerMovement.currentVelocity.x, playerMovement.currentVelocity.z).magnitude,
                timeSinceLastFrame = timer
            });
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

    public void PopulateRunRecording()
    {
        // if the new run is better than the old run, or if there is no old run, save the new run
        if (currentLevel.levelSaveData.completionTime > GameManager.Instance.currentCompletionTime || currentLevel.levelSaveData.completionTime == 0)
        {
            currentLevel.levelSaveData.recordedFrames = recordedFrames.ToArray();
            if (SteamClient.IsValid)
            {
                currentLevel.levelSaveData.ghostRunPlayerName = SteamClient.Name;
            }
            else
            {
                currentLevel.levelSaveData.ghostRunPlayerName = "Yourself"; //so that local replay will say "spectatiing: yourself"
            }
        }
    }

    public void RestartRun()
    {
        timer = 0;
        recordedFrames.Clear();
    }
}
