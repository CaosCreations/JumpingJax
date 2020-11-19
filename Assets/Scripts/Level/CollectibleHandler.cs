using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleHandler : MonoBehaviour
{
    public int collectibleNumber;

    private GameObject child;
    private Level currentLevel;

    private void Start()
    {
        child = transform.GetChild(0).gameObject;
        currentLevel = GameManager.GetCurrentLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == PlayerConstants.PlayerLayer)
        {
            currentLevel.levelSaveData.collectiblesCollected++;
            child.SetActive(false);
        }
    }

    public void ResetActive()
    {
        child.SetActive(true);
    }
}
