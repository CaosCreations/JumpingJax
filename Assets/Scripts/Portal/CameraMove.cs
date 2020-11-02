﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject playerModel;

    [NonSerialized]
    public float sensitivityMultiplier;

    public Camera playerCamera;
    public Quaternion TargetRotation { private set; get; }

    private const float maxCameraXRotation = 90;
    private const float halfRotation = 180;
    private const float fullRotation = 360;
    private const float baseSensitivityMultiplier = 10;
    private Quaternion DefaultRotation = Quaternion.identity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        TargetRotation = transform.rotation;
    }

    private void Start()
    {
        sensitivityMultiplier = OptionsPreferencesManager.GetSensitivity();
        playerCamera.fieldOfView = OptionsPreferencesManager.GetCameraFOV();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        // Rotate the camera.
        var rotation = new Vector2(-Input.GetAxis(PlayerConstants.MouseY), Input.GetAxis(PlayerConstants.MouseX));
        var targetEuler = TargetRotation.eulerAngles + (Vector3) rotation * sensitivityMultiplier * baseSensitivityMultiplier;
        if (targetEuler.x > halfRotation)
        {
            targetEuler.x -= fullRotation;
        }
        targetEuler.x = Mathf.Clamp(targetEuler.x, -maxCameraXRotation, maxCameraXRotation);
        TargetRotation = Quaternion.Euler(targetEuler);

        playerCamera.transform.rotation = TargetRotation;

        Quaternion newRotation = TargetRotation;
        newRotation.eulerAngles = new Vector3(0, newRotation.eulerAngles.y, 0);
        playerModel.transform.rotation = newRotation;
    }

    public void ResetTargetRotation(Quaternion target)
    {
        TargetRotation = target;
    }

    public void SetTargetRotation(Quaternion newRotation)
    {
        newRotation.eulerAngles = new Vector3(newRotation.eulerAngles.x, newRotation.eulerAngles.y, 0);
        TargetRotation = newRotation;
    }
}