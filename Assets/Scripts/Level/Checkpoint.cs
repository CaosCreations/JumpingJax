using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool drawGizmo;
    public bool isFirstCheckpoint;
    public bool isFinalCheckpoint;

    public Material startMaterial;
    public Material startBottomMaterial;
    public Material completedMaterial;
    public Material completedBottomMaterial;

    public bool isCompleted;

    public Renderer checkpointRenderer;
    public Renderer checkpointBottomRenderer;


    public void SetCompleted()
    {
        checkpointRenderer.sharedMaterial = completedMaterial;
        checkpointBottomRenderer.sharedMaterial = completedBottomMaterial;
        isCompleted = true;
    }

    public void SetUncompleted()
    {
        checkpointRenderer.sharedMaterial = startMaterial;
        checkpointBottomRenderer.sharedMaterial = startBottomMaterial;
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