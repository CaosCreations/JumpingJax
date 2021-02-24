using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool drawGizmo;
    public bool isFirstCheckpoint;
    public bool isFinalCheckpoint;

    public Material startMaterial;
    public Material completedMaterial;

    public bool isCompleted;
    public Light myLight;
    private Color startColor = new Color(0, 1, 1);
    private Color completedColor = new Color(0, 1, 0);

    private Renderer myRenderer;

    private void Start()
    {
        myRenderer = GetComponent<Renderer>();
    }

    public void SetCompleted()
    {
        myRenderer.sharedMaterial = completedMaterial;
        isCompleted = true;
        //myLight.color = completedColor;
    }

    public void SetUncompleted()
    {
        myRenderer.sharedMaterial = startMaterial;
        isCompleted = false;
        //myLight.color = startColor;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmo)
        {
            Debug.DrawRay(transform.position, transform.forward * 7, Color.magenta, 0f);
        }
    }
}