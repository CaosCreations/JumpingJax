using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorGizmoUtil
{
    public static Vector3[] redPositionGizmoVerts = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 0.2f, 0)
    };

    public static Vector3[] greenPositionGizmoVerts = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0.2f, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0.2f, 1, 0)
    };

    public static Vector3[] bluePositionGizmoVerts = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0.2f, 1)
    };
}
