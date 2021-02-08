using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
    public bool isDebugging;
    public Vector3 normalStartPosition;
    public Vector3 debugStartPosition;
    
    void Start()
    {
        if(isDebugging == true)
        {
            gameObject.transform.position = debugStartPosition;
            Debug.Log("Starting player mode: DEBUG");
        }
        else
        {
            gameObject.transform.position = normalStartPosition;
            Debug.Log("Starting player mode: NORMAL");
        }
    }
}
