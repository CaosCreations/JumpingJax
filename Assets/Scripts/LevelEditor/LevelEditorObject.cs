using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    FloatingPlatform, Floor, ResetFloor, GlassWall, Checkpoint
}

[Serializable]
public struct ObjectData
{
    [SerializeField]
    public Vector3 position;
    [SerializeField]
    public Vector3 rotation;
    [SerializeField]
    public Vector3 scale;
    [SerializeField]
    public ObjectType objectType;
}

public class LevelEditorObject : MonoBehaviour
{
    public ObjectType objectType;

    public ObjectData GetObjectData()
    {
        ObjectData toReturn = new ObjectData();
        toReturn.position = transform.position;
        toReturn.rotation = transform.rotation.eulerAngles;
        toReturn.scale = transform.localScale;
        toReturn.objectType = objectType;

        return toReturn;
    }
}
