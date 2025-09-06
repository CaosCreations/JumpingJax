using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelSaveData
{
    [SerializeField]
    public int collectiblesCollected;

    [SerializeField]
    public bool isCompleted;

    [SerializeField]
    public float completionTime;

    [SerializeField]
    public RecordedFrame[] recordedFrames;

    [NonSerialized]
    public SelectedGhostRunData ghostRun;
}

[Serializable]
public class RecordedFrame
{
    [SerializeField]
    public Vector3 position;
    [SerializeField]
    public Vector3 cameraRotation;
    [SerializeField]
    public KeysPressed keysPressed;
    [SerializeField]
    public float velocity;
    [SerializeField]
    public float timeSinceLastFrame;
}

public class SelectedGhostRunData
{
    public string playerName;
    public RecordedFrame[] recordedFrames;
}
