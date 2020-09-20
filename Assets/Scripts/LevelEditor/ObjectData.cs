using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    FloatingPlatform, Floor, ResetFloor, GlassWall, Checkpoint
}

public class ObjectData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public ObjectType objectType;
}
