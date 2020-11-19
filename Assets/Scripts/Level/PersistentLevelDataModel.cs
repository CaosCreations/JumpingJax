using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PersistentLevelDataModel
{
    [SerializeField]
    public Collectible[] collectibles;

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
}
