using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TutorialTriggerGroup))] //TutorialTriggerGroups necessary for playerProgress and gameUI population
public class TutorialTrigger : MonoBehaviour
{
    public int requiredDeaths;
    public string[] TutorialText;
    private PlayerProgress playerProgress;
    private InGameUI gameUI;

    private int initialDeaths;
    private bool activated;

    public bool drawGizmo;

    // Use this for initialization
    void Start()
    {
        Collider myCollider = GetComponent<BoxCollider>();
        myCollider.isTrigger = true;
        initialDeaths = -1;
        activated = false;

    }

    public void AddGameUI(InGameUI ui)
    {
        gameUI = ui;
    }

    public void AddPlayerProgress(PlayerProgress player)
    {
        playerProgress = player;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == PlayerConstants.PlayerLayer)
        {
            if (activated) return;
            int currentDeaths = playerProgress.Deaths;
            // On first trigger set initialDeaths = to current player deaths to allow us to 
            // track the number of deaths experienced after first passing the trigger
            // Also reset if player has reset level

            //bl: trying to debug this...
            if (initialDeaths == -1 || currentDeaths < initialDeaths)
            {
                initialDeaths = currentDeaths;
                Debug.Log("initialDeaths set to currentDeaths\n");
            }
            if (currentDeaths - initialDeaths >= requiredDeaths)
            {
                gameUI.SetupTutorialTexts(TutorialText);
                activated = true;
                Debug.Log("Tutorial text set called, activated = true");
            }
        }
    }

    public void ResetTrigger()
    {
        activated = false;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmo)
        {
            BoxCollider myCollider = GetComponent<BoxCollider>();
            Gizmos.color = new Color(0.317f, .0317f, 0.4f, 0.5f); //ruddy purple, semi-transparent
            Gizmos.DrawCube(transform.position, myCollider.size);
            Gizmos.color = new Color(0.317f, .0317f, 0.4f, 1); //ruddy purple
            Gizmos.DrawWireCube(transform.position, myCollider.size);
        }
    }
}