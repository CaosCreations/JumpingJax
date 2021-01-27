using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebug : MonoBehaviour
{
    

    public bool isDebugging;
    public Vector3 normalStartPosition;
    public Vector3 debugStartPosition;
    private GameObject currentGameObject;
    
    //public bool isDebugging;
    //public bool isDebugging;

    void Start()
    {
        currentGameObject = this.gameObject;
        
        if(isDebugging == true)
        {
            currentGameObject.transform.position = debugStartPosition;
            Debug.Log("Starting player mode: DEBUG");
        }
        else
        {
            currentGameObject.transform.position = normalStartPosition;
            Debug.Log("Starting player mode: NORMAL");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
