using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PersistentLevelDataModel
{
    [SerializeField]
    public int collectiblesCollected;

    [SerializeField]
    public bool isCompleted;

    [SerializeField]
    public float completionTime;

    [SerializeField]
    public RecordedFrame[] recordedFrames;

    [SerializeField]
    public string ghostRunPlayerName;
}

public class RecordedFrame
{
    public Vector3 position;
    public Vector3 cameraRotation;
    public KeysPressed keysPressed;
    public float velocity;
    public float timeSinceLastFrame;
}
