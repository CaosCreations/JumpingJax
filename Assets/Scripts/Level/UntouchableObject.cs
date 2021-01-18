using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UntouchableObject : MonoBehaviour
{
    private void Start()
    {
        Collider myCollider = GetComponent<Collider>();
        myCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerProgress playerProgress = other.GetComponent<PlayerProgress>();
        if(playerProgress == null) {
            return;
        }

        playerProgress.Respawn();
        PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Death);
    }

    private void OnDrawGizmos()
    {
        BoxCollider myCollider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(181.0f / 255.0f, 45.0f / 255.0f, 50.0f / 255.0f, 1.0f); //this is supposed to be red
        Gizmos.DrawWireCube(transform.position, transform.localScale);

    }
}
