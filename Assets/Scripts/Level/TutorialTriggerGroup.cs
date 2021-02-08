using UnityEngine.SceneManagement;
using UnityEngine;

public class TutorialTriggerGroup : MonoBehaviour
{

    public TutorialTrigger[] triggers;
    public InGameUI gameUI;
    public PlayerProgress playerProgress;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnNewLevelLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnNewLevelLoaded;
    }

    private void OnNewLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateTriggers();
    }

    void UpdateTriggers()
    {
        triggers = FindObjectsOfType<TutorialTrigger>();
        if (triggers.Length == 0)
        {
            // No need to search for PlayerProgress or InGameUI if there are no trigger elements.
            return;
        }
        playerProgress = GameObject.FindWithTag(PlayerConstants.PlayerTag).GetComponent<PlayerProgress>();
        gameUI = playerProgress.GetComponentInChildren<InGameUI>(true);
        if (playerProgress == null || gameUI == null)
        {
            Debug.LogError("Unable to find playerProgress or gameUI");
            return;
        }

        foreach (TutorialTrigger trigger in triggers)
        {
            trigger.AddGameUI(gameUI);
            trigger.AddPlayerProgress(playerProgress);
        }
    }

    // Reset triggers to inactive on level reset to display messages again
    public void ResetTriggers()
    {
        foreach (TutorialTrigger trigger in triggers)
        {
            trigger.ResetTrigger();
        }
    }
}