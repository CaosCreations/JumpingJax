using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //script stolen from nash by lb :p
    private void OnDrawGizmos()
    {
        BoxCollider myCollider = GetComponent<BoxCollider>();
        Gizmos.color = new Color(181.0f / 255.0f , 45.0f / 255.0f, 50.0f / 255.0f, 0.5f); //this is supposed to be red, semi-transparent
        Gizmos.DrawCube(transform.position, transform.localScale);
        Gizmos.color = new Color(181.0f / 255.0f, 45.0f / 255.0f, 50.0f / 255.0f, 1.0f); //this is supposed to be red
        Gizmos.DrawWireCube(transform.position, transform.localScale);

    }
}
