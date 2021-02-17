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
    public Vector3[] ghostRunPositions;

    [SerializeField]
    public Vector3[] ghostRunCameraRotations;

    [SerializeField]
    public KeysPressed[] ghostRunKeys;

    [SerializeField]
    public float[] ghostRunVelocities;

    [SerializeField]
    public string ghostRunPlayerName;
}
