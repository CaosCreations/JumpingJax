using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorGizmoUtil
{
    #region Position
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

    public static int[] positionTriangles = new int[]
    {
        0, 1, 2,
        0, 2, 1,
        1, 3, 2,
        1, 2, 3
    };
    #endregion

    #region Rotation
    public static Vector3[] redRotationGizmoVerts = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 0.2f, 0)
    };

    public static Vector3[] greenRotationGizmoVerts = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0.2f, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0.2f, 1, 0)
    };

    public static Vector3[] blueRotationGizmoVerts = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0.2f, 1)
    };

    public static int[] rotationTriangles = new int[]
    {
        0, 1, 2,
        0, 2, 1,
        1, 3, 2,
        1, 2, 3
    };
    #endregion

    #region Scale
    public static Vector3[] redScaleGizmoVerts = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 0.2f, 0)
    };

    public static Vector3[] greenScaleGizmoVerts = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0.2f, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0.2f, 1, 0)
    };

    public static Vector3[] blueScaleGizmoVerts = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0.2f, 1)
    };

    public static int[] scaleTriangles = new int[]
    {
        0, 1, 2,
        0, 2, 1,
        1, 3, 2,
        1, 2, 3
    };
    #endregion
}
