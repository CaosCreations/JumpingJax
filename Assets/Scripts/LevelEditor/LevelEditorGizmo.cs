using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum GizmoColor
{
    Red, Green, Blue
}

public class LevelEditorGizmo : MonoBehaviour
{
    public Material redGizmoMaterial;
    public Material blueGizmoMaterial;
    public Material greenGizmoMaterial;

    public Camera mainCamera;

    public int gizmoLayer = 15;
    private Transform selectedObject;
    private ManipulationType manipulationType;
    
    private GameObject redPositionGizmo;
    private GameObject redRotationGizmo;
    private GameObject redScaleGizmo;

    private GameObject greenPositionGizmo;
    private GameObject greenRotationGizmo;
    private GameObject greenScaleGizmo;

    private GameObject bluePositionGizmo;
    private GameObject blueRotationGizmo;
    private GameObject blueScaleGizmo;

    private Vector3 lastMousePosition;
    void Awake()
    {
        GenerateGizmos();
        lastMousePosition = Vector3.zero;
        mainCamera = GetComponentInParent<Camera>();
    }
    
    public void GizmoFollowMouse(GizmoColor gizmoColor)
    {
        if(lastMousePosition == Vector3.zero)
        {
            lastMousePosition = Input.mousePosition;
            return;
        }

        Vector3 newPosition = Vector3.zero;
        float distanceFromCameraToObject = (mainCamera.transform.position - selectedObject.position).magnitude;
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCameraToObject));
        Vector3 mouseDelta = lastMousePosition - worldPoint;

        switch (gizmoColor)
        {
            case GizmoColor.Red:
                
                newPosition = new Vector3(-mouseDelta.x, 0, 0);
                break;
            case GizmoColor.Green:
                newPosition = new Vector3(0, -mouseDelta.y, 0);
                break;
            case GizmoColor.Blue:
                newPosition = new Vector3(0, 0, -mouseDelta.z);
                break;
        }
        selectedObject.position += newPosition;
        lastMousePosition = worldPoint;
    }

    public void UpdateGizmos()
    {
        switch (manipulationType)
        {
            case ManipulationType.Position:
                redPositionGizmo.transform.position = selectedObject.transform.position;
                greenPositionGizmo.transform.position = selectedObject.transform.position;
                bluePositionGizmo.transform.position = selectedObject.transform.position;
                break;
            case ManipulationType.Rotation:
                break;
            case ManipulationType.Scale:
                break;
        }
    }

    public void SetGizmo(Transform selectedObject, ManipulationType manipulationType)
    {
        this.selectedObject = selectedObject;
        this.manipulationType = manipulationType;

        switch (manipulationType)
        {
            case ManipulationType.Position:
                redPositionGizmo.SetActive(true);
                greenPositionGizmo.SetActive(true);
                bluePositionGizmo.SetActive(true);
                break;
            case ManipulationType.Rotation:
                //redRotationGizmo.SetActive(true);
                //greenRotationGizmo.SetActive(true);
                //blueRotationGizmo.SetActive(true);
                break;
            case ManipulationType.Scale:
                //redScaleGizmo.SetActive(true);
                //greenScaleGizmo.SetActive(true);
                //blueScaleGizmo.SetActive(true);
                break;
        }
    }

    public void ClearGizmo()
    {
        selectedObject = null;

        redPositionGizmo.SetActive(false);
        //redRotationGizmo.SetActive(false);
        //redScaleGizmo.SetActive(false);

        greenPositionGizmo.SetActive(false);
        //greenRotationGizmo.SetActive(false);
        //greenScaleGizmo.SetActive(false);

        bluePositionGizmo.SetActive(false);
        //blueRotationGizmo.SetActive(false);
        //blueScaleGizmo.SetActive(false);
    }

    private void GenerateGizmos()
    {
        GeneratePositionGizmos();
        GenerateRotationGizmos();
        GenerateScaleGizmos();
    }

    private void GeneratePositionGizmos()
    {
        GeneratePositions(GizmoColor.Red);
        GeneratePositions(GizmoColor.Green);
        GeneratePositions(GizmoColor.Blue);
    }

    private void GeneratePositions(GizmoColor gizmoColor)
    {
        GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GizmoType gizmoType = newObject.AddComponent<GizmoType>();
        gizmoType.gizmoColor = gizmoColor;
        newObject.layer = gizmoLayer;
        MeshFilter meshFilter = newObject.GetComponent<MeshFilter>();
        Renderer renderer = newObject.GetComponent<Renderer>();
        BoxCollider collider = newObject.GetComponent<BoxCollider>();
        meshFilter.mesh = GeneratePosition(gizmoColor);

        switch (gizmoColor)
        {
            case GizmoColor.Red:
                renderer.sharedMaterial = redGizmoMaterial;
                redPositionGizmo = newObject;
                break;
            case GizmoColor.Green:
                renderer.sharedMaterial = greenGizmoMaterial;
                greenPositionGizmo = newObject;
                break;
            case GizmoColor.Blue:
                renderer.sharedMaterial = blueGizmoMaterial;
                bluePositionGizmo = newObject;
                break;
        }

        collider.center = renderer.bounds.center;

        Vector3 size = renderer.bounds.size;

        if(size.x == 0)
        {
            size.x = 0.05f;
        }
        if (size.y == 0)
        {
            size.y = 0.05f;
        }
        if (size.z == 0)
        {
            size.z = 0.05f;
        }

        collider.size = size;
    }

    private Mesh GeneratePosition(GizmoColor gizmoColor)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        switch (gizmoColor)
        {
            case GizmoColor.Red:
                verts = LevelEditorGizmoUtil.redPositionGizmoVerts.ToList();
                break;
            case GizmoColor.Green:
                verts = LevelEditorGizmoUtil.greenPositionGizmoVerts.ToList();
                break;
            case GizmoColor.Blue:
                verts = LevelEditorGizmoUtil.bluePositionGizmoVerts.ToList();
                break;
        }
        mesh.vertices = verts.ToArray();
        mesh.triangles = new int[]
        {
            0, 1, 2,
            0, 2, 1,
            1, 3, 2,
            1, 2, 3
        };

        mesh.RecalculateNormals();
        return mesh;
    }

    private void GenerateRotationGizmos()
    {

    }

    private void GenerateScaleGizmos()
    {

    }
}
