using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool drawGizmo;
    public bool isFirstCheckpoint;
    public bool isFinalCheckpoint;

    public Material startMaterial;
    public Material completedMaterial;

    public bool isCompleted;

    private Renderer myRenderer;

    private void Start()
    {
        myRenderer = GetComponent<Renderer>();
    }

    public void SetCompleted()
    {
        myRenderer.sharedMaterial = completedMaterial;
        isCompleted = true;
    }

    public void SetUncompleted()
    {
        myRenderer.sharedMaterial = startMaterial;
        isCompleted = false;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmo)
        {
            Debug.DrawRay(transform.position, transform.forward * 7, Color.magenta, 0f);
        }
    }
}