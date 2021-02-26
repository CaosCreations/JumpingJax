using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
    public bool isDebugging;
    public Vector3 debugStartPosition;
    
    void Start()
    {
    #if UNITY_EDITOR
        if(isDebugging == true)
        {
            gameObject.transform.position = debugStartPosition;
            Debug.Log("[PlayerDebug]: Starting player mode at debug position.");
        }
        else
        {
            Debug.Log("[PlayerDebug]: Starting player at entity editor position.");
        }
    #endif
    }
}
