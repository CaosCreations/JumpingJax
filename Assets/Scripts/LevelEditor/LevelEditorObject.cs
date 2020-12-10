using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    FloatingPlatform, Floor, ResetFloor, GlassWall, Checkpoint, FirstCheckpoint, FinalCheckpoint, Wall, PortalWall
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

    // Since JSONUtility doesn't allow subclasses we have to shove all possible data in here and handle it at load-time
    [SerializeField]
    public int checkpointNumber;
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

