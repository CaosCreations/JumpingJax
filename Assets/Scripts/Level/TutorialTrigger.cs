using System.Collections;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public string[] TutorialText;
    private PlayerProgress playerProgress;
    private InGameUI gameUI;

    private bool activated;

    public bool drawGizmo;

    void Start()
    {
        gameUI = ReferenceRegistrar.Instance.inGameUI;
        playerProgress = ReferenceRegistrar.Instance.Player.PlayerProgress;
        Collider myCollider = GetComponent<BoxCollider>();
        myCollider.isTrigger = true;
        activated = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == PlayerConstants.PlayerLayer)
        {
            if (activated) return;
            // Handle out-of-order registration due to game UI being enabled after start
            if (gameUI == null)
            {
                gameUI = ReferenceRegistrar.Instance.inGameUI;
            }
            gameUI.SetupTutorialTexts(TutorialText);
            activated = true;
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

            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            BoxCollider myCollider = GetComponent<BoxCollider>();
            Gizmos.color = new Color(0.317f, .0317f, 0.4f, 0.5f); //ruddy purple, semi-transparent
            Gizmos.DrawCube(gizmoShiftVector, myCollider.size);
            Gizmos.color = new Color(0.317f, .0317f, 0.4f, 1); //ruddy purple
            Gizmos.DrawWireCube(gizmoShiftVector, myCollider.size);
        }
    }
}