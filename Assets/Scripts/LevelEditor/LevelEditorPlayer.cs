using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorPlayer : MonoBehaviour
{
    public float moveSpeed = 10f;

    private float sensitivityMultiplier;
    private const float maxCameraXRotation = 90;
    private const float halfRotation = 180;
    private const float fullRotation = 360;

    private void Start()
    {
        sensitivityMultiplier = OptionsPreferencesManager.GetSensitivity();
    }

    private void Update()
    {
        transform.position += transform.forward * Input.mouseScrollDelta.y;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            UpdatePosition();
            UpdateRotation();
        }
        
    }

    private void UpdatePosition()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * moveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -transform.forward * Time.deltaTime * moveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime * moveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -transform.right * Time.deltaTime * moveSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.up * Time.deltaTime * moveSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += -transform.up * Time.deltaTime * moveSpeed;
        }
    }

    private void UpdateRotation()
    {
        // Rotate the camera.
        var rotation = new Vector2(-Input.GetAxis(PlayerConstants.MouseY), Input.GetAxis(PlayerConstants.MouseX));
        var targetEuler = transform.rotation.eulerAngles + (Vector3)rotation * sensitivityMultiplier * 10;
        if (targetEuler.x > halfRotation)
        {
            targetEuler.x -= fullRotation;
        }
        targetEuler.x = Mathf.Clamp(targetEuler.x, -maxCameraXRotation, maxCameraXRotation);
        transform.rotation = Quaternion.Euler(targetEuler);

    }
}
