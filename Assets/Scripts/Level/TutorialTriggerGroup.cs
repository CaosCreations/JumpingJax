using UnityEngine;

/// <summary>
/// Should only have 1 per scene, its attached to the Player object
/// </summary>
public class TutorialTriggerGroup : MonoBehaviour
{
    public TutorialTrigger[] triggers;

    private void Start()
    {
        UpdateTriggers();
    }

    void UpdateTriggers()
    {
        triggers = FindObjectsByType<TutorialTrigger>(FindObjectsSortMode.InstanceID);
        if (triggers.Length == 0)
        {
            return;
        }
    }

    // Reset triggers to inactive on level reset to display messages again
    public void ResetTriggers()
    {
        foreach (TutorialTrigger trigger in triggers)
        {
            trigger.ResetTrigger();
        }
        if (!ReferenceRegistrar.Instance.inGameUI.IsGhosting)
        {
            ReferenceRegistrar.Instance.inGameUI.SetupTutorialTexts(GameManager.GetCurrentLevel().tutorialTexts);
        }
    }
}