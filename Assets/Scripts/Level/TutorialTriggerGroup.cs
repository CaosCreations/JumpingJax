using System.Collections;
using UnityEngine;

public class TutorialTriggerGroup : MonoBehaviour
{

    public TutorialTrigger[] triggers;
    public InGameUI gameUI;
    public PlayerProgress playerProgress;

    // Use this for initialization
    void Start()
    {
        // Referenced here because OnLevelLoaded isn't called if we directly run a level
        UpdateTriggers();
    }

    private void OnLevelWasLoaded(int level)
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