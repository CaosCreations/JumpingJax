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
            Vector3 gizmoShiftVector = new Vector3(0, 0, 0); //positional shift of the gizmo relative to the collider and transform.localPosition

            Gizmos.matrix = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
            BoxCollider myCollider = GetComponent<BoxCollider>();
            Gizmos.color = new Color(0.317f, .0317f, 0.4f, 0.5f); //ruddy purple, semi-transparent
            Gizmos.DrawCube(gizmoShiftVector, myCollider.size);
            Gizmos.color = new Color(0.317f, .0317f, 0.4f, 1); //ruddy purple
            Gizmos.DrawWireCube(gizmoShiftVector, myCollider.size);
        }
    }
}