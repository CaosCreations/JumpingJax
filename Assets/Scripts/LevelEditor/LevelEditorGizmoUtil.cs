using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorGizmoUtil
{
    #region Position
    public static Vector3[] redPositionGizmoVerts = new Vector3[]
    {
        // arrow base
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 0.2f, 0),
        // arrow tip
        new Vector3(1, 0.3f, 0),
        new Vector3(1, -0.1f, 0),
        new Vector3(1.3f, 0.1f, 0)
    };

    public static Vector3[] greenPositionGizmoVerts = new Vector3[]
    {
        // arrow base
        new Vector3(0, 0, 0),
        new Vector3(0.2f, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0.2f, 1, 0),
        // arrow tip
        new Vector3(0.3f, 1, 0),
        new Vector3(-0.1f, 1, 0),
        new Vector3(0.1f, 1.3f, 0)
    };

    public static Vector3[] bluePositionGizmoVerts = new Vector3[]
    {
        // arrow base
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0.2f, 1),
        // arrow tip
        new Vector3(0, 0.3f, 1),
        new Vector3(0, -0.1f, 1),
        new Vector3(0, 0.1f, 1.3f)
    };

    public static int[] positionTriangles = new int[]
    {
        // arrow base
        0, 1, 2,
        0, 2, 1,
        1, 3, 2,
        1, 2, 3,
        // arrow tip
        4, 5, 6,
        4, 6, 5
    };
    #endregion

    #region Rotation
    public static Vector3[] redRotationGizmoVerts = new Vector3[]
    {
        // arrow base
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 0.2f, 0),
        // arrow tip
        new Vector3(1, 0.3f, 0),
        new Vector3(1, -0.1f, 0),
        new Vector3(1.3f, 0.1f, 0)
    };

    public static Vector3[] greenRotationGizmoVerts = new Vector3[]
    {
        // arrow base
        new Vector3(0, 0, 0),
        new Vector3(0.2f, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0.2f, 1, 0),
        // arrow tip
        new Vector3(0.3f, 1, 0),
        new Vector3(-0.1f, 1, 0),
        new Vector3(0.1f, 1.3f, 0)
    };

    public static Vector3[] blueRotationGizmoVerts = new Vector3[]
    {
        // arrow base
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0.2f, 1),
        // arrow tip
        new Vector3(0, 0.3f, 1),
        new Vector3(0, -0.1f, 1),
        new Vector3(0, 0.1f, 1.3f)
    };

    public static int[] rotationTriangles = new int[]
    {
        // arrow base
        0, 1, 2,
        0, 2, 1,
        1, 3, 2,
        1, 2, 3,
        // arrow tip
        4, 5, 6,
        4, 6, 5
    };
    #endregion

    #region Scale
    public static Vector3[] redScaleGizmoVerts = new Vector3[]
    {
        // Arrow Base
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 0.2f, 0),
        // Arrow Tip
        new Vector3(1, 0.3f, 0),
        new Vector3(1, -0.1f, 0),
        new Vector3(1.4f, 0.3f, 0),
        new Vector3(1.4f, -0.1f, 0)
    };

    public static Vector3[] greenScaleGizmoVerts = new Vector3[]
    {
        // Arrow Base
        new Vector3(0, 0, 0),
        new Vector3(0.2f, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0.2f, 1, 0),
        // Arrow Tip
        new Vector3(-0.1f, 1, 0),
        new Vector3(0.3f, 1, 0),
        new Vector3(-0.1f, 1.4f, 0),
        new Vector3(0.3f, 1.4f, 0)
    };

    public static Vector3[] blueScaleGizmoVerts = new Vector3[]
    {
        // Arrow Base
        new Vector3(0, 0, 0),
        new Vector3(0, 0.2f, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0.2f, 1),
        // Arrow Tip
        new Vector3(0, 0.3f, 1),
        new Vector3(0, -0.1f, 1),
        new Vector3(0, 0.3f, 1.4f),
        new Vector3(0, -0.1f, 1.4f)
    };

    public static int[] scaleTriangles = new int[]
    {
        // Arrow Base
        0, 1, 2,
        0, 2, 1,
        1, 3, 2,
        1, 2, 3,
        // Arrow Tip
        4, 5, 7,
        4, 7, 5,
        4, 6, 7,
        4, 7, 6
    };
    #endregion
}
