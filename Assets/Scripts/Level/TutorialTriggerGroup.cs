using UnityEngine;

public class TutorialTriggerGroup : MonoBehaviour
{
    public static TutorialTriggerGroup Instance { get; private set; }

    public TutorialTrigger[] triggers;
    public InGameUI gameUI;
    public PlayerProgress playerProgress;

    private void Awake()
    {
        Init();
        UpdateTriggers();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Init()
    {
        // Make sure to clean references from an old scene
        playerProgress = null;
        gameUI = null;

        GameObject player = GameObject.FindWithTag(PlayerConstants.PlayerTag);
        if(player != null)
        {
            playerProgress = player.GetComponent<PlayerProgress>();
            gameUI = player.GetComponentInChildren<InGameUI>(true);
        }
    }

    void UpdateTriggers()
    {
        triggers = FindObjectsOfType<TutorialTrigger>();
        if (triggers.Length == 0 || playerProgress == null || gameUI == null)
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
    }
}