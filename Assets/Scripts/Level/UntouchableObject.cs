﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UntouchableObject : MonoBehaviour
{

    public bool useGizmos;
    private GameObject currentGameObject;

    private void Start()
    {
        Collider myCollider = GetComponent<Collider>();
        myCollider.isTrigger = true;
        currentGameObject = this.gameObject;
    }

    //lb: I added some useful debugging features for level design
    private void OnTriggerEnter(Collider other)
    {
        PlayerProgress playerProgress = other.GetComponent<PlayerProgress>();
        if(playerProgress == null) {
            Debug.Log("The player HIT GameObject " + currentGameObject.name + "that has UntouchableScript.cs on it at " + currentGameObject.transform.position);
            return;
        }

        playerProgress.Respawn();
        PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Death);
    }

    private void OnDrawGizmos()
    {
        if (useGizmos)
        {
            BoxCollider myCollider = GetComponent<BoxCollider>();
            Gizmos.color = new Color(181.0f / 255.0f, 45.0f / 255.0f, 50.0f / 255.0f, 1.0f); //this is supposed to be red
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}
