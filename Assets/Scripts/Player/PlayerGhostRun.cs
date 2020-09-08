using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerGhostRun : MonoBehaviour
{
    public KeyPressed keyPressed;

    public GameObject ghostRunnerPrefab;
    private GameObject ghostRunner;

    private List<Vector3> currentRunPositionData;
    private List<KeysPressed> currentRunKeyData;
    private float ghostRunSaveTimer = 0;
    private float ghostRunnerTimer = 0;
    private Level currentLevel;
    private int currentDataIndex = 0;


    private const float ghostRunSaveInterval = 0.05f;


    void Start()
    {
        keyPressed = GetComponentInChildren<KeyPressed>();
        RestartRun();
        currentLevel = GameManager.GetCurrentLevel();
        if(ghostRunner == null)
        {
            ghostRunner = Instantiate(ghostRunnerPrefab);
        }
        ghostRunner.SetActive(false);
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        RecordCurrentRunData();
        UpdateGhost();
    }

    public void RestartRun()
    {
        ghostRunSaveTimer = 0;
        ghostRunnerTimer = 0;
        currentDataIndex = 0;
        currentRunPositionData = new List<Vector3>();
        currentRunKeyData = new List<KeysPressed>();
    }

    private void UpdateGhost()
    {
        if (currentLevel == null 
            || currentLevel.ghostRunPositions == null 
            || currentDataIndex >= currentLevel.ghostRunPositions.Length - 1)
        {
            return; // ghost run is finished
        }

        // Only show the ghost run for a level we've completed
        if (currentLevel.isCompleted)
        {
            ghostRunner.SetActive(true);

            float lerpValue = ghostRunnerTimer / ghostRunSaveInterval;
            Vector3 position = Vector3.Lerp(ghostRunner.transform.position, currentLevel.ghostRunPositions[currentDataIndex], lerpValue);
            ghostRunner.transform.position = position;
            keyPressed.SetPressed(currentLevel.ghostRunKeys[currentDataIndex]);
        }

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
        if (ghostRunSaveTimer > ghostRunSaveInterval)
        {
            ghostRunSaveTimer = 0;
            currentRunPositionData.Add(transform.position);
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
        GameManager.GetCurrentLevel().ghostRunPositions = currentRunPositionData.ToArray();
        GameManager.GetCurrentLevel().ghostRunKeys = currentRunKeyData.ToArray();
    }
}
