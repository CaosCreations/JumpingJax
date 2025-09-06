using JetBrains.Annotations;
using Newtonsoft.Json;
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
    // Newtonsoft.Json does not support serializing Vector3 directly
    [SerializeField]
    [JsonConverter(typeof(Vec3Converter))]
    public Vec3 position;
    [SerializeField]
    [JsonConverter(typeof(Vec3Converter))]
    public Vec3 cameraRotation;
    [SerializeField]
    [JsonConverter(typeof(KeyPressedConverter))]
    public KeysPressed keysPressed;
    [SerializeField]
    public float velocity;
    [SerializeField]
    public float timeSinceLastFrame;

    public Vector3 GetPosition()
    {
        return new Vector3(position.x, position.y, position.z);
    }

    public Vector3 GetCameraRotation()
    {
        return new Vector3(cameraRotation.x, cameraRotation.y, cameraRotation.z);
    }
}

public class SelectedGhostRunData
{
    public string playerName;
    public RecordedFrame[] recordedFrames;
}

public class Vec3
{
    public float x;
    public float y;
    public float z;
    public Vec3() { }

    public Vec3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }


    public override string ToString()
    {
        return $"{x},{y},{z}";
    }

    public static Vec3 FromString(string jsonString)
    {
        var parts = jsonString.Split(',');
        return new Vec3()
        {
            x = float.Parse(parts[0]),
            y = float.Parse(parts[1]),
            z = float.Parse(parts[2])
        };
    }
}
