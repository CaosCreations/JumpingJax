using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCollider : MonoBehaviour
{
    private CharacterController ghostController;

    // Start is called before the first frame update
    void Start()
    {
        ghostController = GetComponentInChildren<CharacterController>();
        ghostController.detectCollisions = false;
    }

    public void ActivateCollider(bool activate)
    {
        Debug.Log("Collider is now active status " + activate);
        ghostController.detectCollisions = activate;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("Collider triggered by ghost");
        Checkpoint checkPointHit = other.gameObject.GetComponent<Checkpoint>();
        if (checkPointHit)
        {
            // Only play the sound on the first time touching the checkpoint, and don't play the sound if it's the final checkpoint as other sounds may play then
            if (!checkPointHit.isCompleted && !checkPointHit.isFinalCheckpoint)
            {
                PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Checkpoint);
            }
            checkPointHit.SetCompleted();
        }
    }
}
