using UnityEngine;

public class TutorialTriggerGroup : MonoBehaviour
{

    public TutorialTrigger[] triggers;
    public InGameUI gameUI;
    public PlayerProgress playerProgress;

    private void Start()
    {
        playerProgress = GetComponent<PlayerProgress>();
        gameUI = GetComponentInChildren<InGameUI>(true);
        UpdateTriggers();
    }

    void UpdateTriggers()
    {
        triggers = FindObjectsOfType<TutorialTrigger>();
        if (triggers.Length == 0)
        {
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
        gameUI.SetupTutorialTexts(GameManager.GetCurrentLevel().tutorialTexts);
    }
}