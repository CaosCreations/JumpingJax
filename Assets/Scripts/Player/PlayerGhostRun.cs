using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostRun : MonoBehaviour
{
    public KeyPressed keyPressed;

    public GameObject ghostRunnerPrefab;
    public GameObject ghostRunner;

    private List<Vector3> currentRunPositionData;
    private List<KeysPressed> currentRunKeyData;
    private float ghostRunSaveTimer = 0;
    private float ghostRunnerTimer = 0;
    private Level currentLevel;
    private int currentDataIndex = 0;

    private const int maxDataCount = 25000; //Makes it so max file save is 5MB, stores 20.8 min of Ghost data saved

    private const float ghostRunSaveInterval = 0.05f;

    void Start()
    {
        currentLevel = GameManager.GetCurrentLevel();
        if(ghostRunner == null)
        {
            ghostRunner = Instantiate(ghostRunnerPrefab);
            ghostRunner.name = "ghost runner";
        }
        RestartRun();

        MiscOptions.onGhostToggle += ToggleGhost;
    }

    private void Update()
    {
        if (Time.timeScale == 0 || GameManager.GetCurrentLevel().workshopFilePath != string.Empty)
        {
            return;
        }

        RecordCurrentRunData();
        UpdateGhost();
    }

    public void RestartRun()
    {
        ghostRunner.SetActive(OptionsPreferencesManager.GetGhostToggle() && GameManager.GetCurrentLevel().isCompleted);
        ghostRunSaveTimer = 0;
        ghostRunnerTimer = 0;
        currentDataIndex = 0;
        currentRunPositionData = new List<Vector3>();
        currentRunKeyData = new List<KeysPressed>();
    }

    private void UpdateGhost()
    {
        if (currentLevel == null 
            || currentLevel.ghostRunPositions == null)
        {
            return; // ghost run is finished
        }
        if (currentDataIndex >= currentLevel.ghostRunPositions.Length - 1)
        {
            currentDataIndex = 0;
        }
        // Only show the ghost run for a level we've completed
        if (currentLevel.isCompleted)
        {
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
        if (ghostRunSaveTimer > ghostRunSaveInterval && currentRunPositionData.Count < maxDataCount)
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
        if(currentLevel.completionTime > GameManager.Instance.currentCompletionTime || currentLevel.completionTime == 0)
        {
            currentLevel.ghostRunPositions = currentRunPositionData.ToArray();
            currentLevel.ghostRunKeys = currentRunKeyData.ToArray();
        }
    }

    private void ToggleGhost(bool isOn)
    {
        Debug.Log("ToggleGhost fired.");
        ghostRunner.SetActive(isOn); 
        OptionsPreferencesManager.SetGhostToggle(isOn);
    }
}
