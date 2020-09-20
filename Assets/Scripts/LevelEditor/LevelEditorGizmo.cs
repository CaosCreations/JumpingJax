using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelEditorGizmo : MonoBehaviour
{
    public Material redGizmoMaterial;
    public Material blueGizmoMaterial;
    public Material greenGizmoMaterial;

    public Transform selectedObject;

    public Mesh redGizmoMesh;
    public Mesh greenGizmoMesh;
    public Mesh blueGizmoMesh;
    void Start()
    {
        GenerateRedMesh();
        GenerateGreenMesh();
        GenerateBlueMesh();
    }

    void Update()
    {
        if(selectedObject == null)
        {
            return;
        }

        DrawGizmo();
    }

    public void SetGizmo(Transform selectedObject)
    {
        this.selectedObject = selectedObject;
    }

    public void ClearGizmo()
    {
        selectedObject = null;
    }

    private void DrawGizmo()
    {
        Graphics.DrawMesh(redGizmoMesh, selectedObject.position, selectedObject.rotation, redGizmoMaterial, 0);
        Graphics.DrawMesh(greenGizmoMesh, selectedObject.position, selectedObject.rotation, greenGizmoMaterial, 0);
        Graphics.DrawMesh(blueGizmoMesh, selectedObject.position, selectedObject.rotation, blueGizmoMaterial, 0);
    }


    private void GenerateRedMesh()
    {
        redGizmoMesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        verts.Add(new Vector3(0, 0, 0));
        verts.Add(new Vector3(0, 0.2f, 0));
        verts.Add(new Vector3(1, 0, 0));
        verts.Add(new Vector3(1, 0.2f, 0));
        redGizmoMesh.vertices = verts.ToArray();
        redGizmoMesh.triangles = new int[]
        {
            0, 1, 2,
            0, 2, 1,
            1, 3, 2,
            1, 2, 3
        };

        redGizmoMesh.RecalculateNormals();
    }

    private void GenerateGreenMesh()
    {
        greenGizmoMesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        verts.Add(new Vector3(0, 0, 0));
        verts.Add(new Vector3(0.2f, 0, 0));
        verts.Add(new Vector3(0, 1, 0));
        verts.Add(new Vector3(0.2f, 1, 0));
        greenGizmoMesh.vertices = verts.ToArray();
        greenGizmoMesh.triangles = new int[]
        {
            0, 1, 2,
            0, 2, 1,
            1, 3, 2,
            1, 2, 3
        };
        greenGizmoMesh.RecalculateNormals();
    }

    private void GenerateBlueMesh()
    {
        blueGizmoMesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        verts.Add(new Vector3(0, 0, 0));
        verts.Add(new Vector3(0, 0.2f, 0));
        verts.Add(new Vector3(0, 0, 1));
        verts.Add(new Vector3(0, 0.2f, 1));
        blueGizmoMesh.vertices = verts.ToArray();
        blueGizmoMesh.triangles = new int[]
        {
            0, 1, 2,
            0, 2, 1,
            1, 3, 2,
            1, 2, 3
        };
        blueGizmoMesh.colors = new Color[]
        {
            new Color(1, 1, 1),
            new Color(1, 1, 1),
            new Color(1, 1, 1),
            new Color(1, 1, 1)
        };
        blueGizmoMesh.RecalculateNormals();
    }
}
