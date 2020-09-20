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
        switch (manipulationType)
        {
            case ManipulationType.Position:
                Reposition(gizmoColor);
                break;
            case ManipulationType.Rotation:
                ReRotation(gizmoColor);
                break;
            case ManipulationType.Scale:
                ReScale(gizmoColor);
                break;
        }
    }

    private void Reposition(GizmoColor gizmoColor)
    {
        float distanceFromCameraToObject = (mainCamera.transform.position - selectedObject.position).magnitude;
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCameraToObject));

        if (lastMousePosition == Vector3.zero)
        {
            lastMousePosition = worldPoint;
            return;
        }

        Vector3 newPosition = Vector3.zero;

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

    private void ReRotation(GizmoColor gizmoColor)
    {

    }

    private void ReScale(GizmoColor gizmoColor)
    {
        float distanceFromCameraToObject = (mainCamera.transform.position - selectedObject.position).magnitude;
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCameraToObject));

        if (lastMousePosition == Vector3.zero)
        {
            lastMousePosition = worldPoint;
            return;
        }

        Vector3 newScale = Vector3.zero;

        Vector3 mouseDelta = lastMousePosition - worldPoint;

        switch (gizmoColor)
        {
            case GizmoColor.Red:

                newScale = new Vector3(-mouseDelta.x, 0, 0);
                break;
            case GizmoColor.Green:
                newScale = new Vector3(0, -mouseDelta.y, 0);
                break;
            case GizmoColor.Blue:
                newScale = new Vector3(0, 0, -mouseDelta.z);
                break;
        }
        selectedObject.localScale += newScale;
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
                redRotationGizmo.transform.position = selectedObject.transform.position;
                greenRotationGizmo.transform.position = selectedObject.transform.position;
                blueRotationGizmo.transform.position = selectedObject.transform.position;
                break;
            case ManipulationType.Scale:
                redScaleGizmo.transform.position = selectedObject.transform.position;
                greenScaleGizmo.transform.position = selectedObject.transform.position;
                blueScaleGizmo.transform.position = selectedObject.transform.position;
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
                redRotationGizmo.SetActive(true);
                greenRotationGizmo.SetActive(true);
                blueRotationGizmo.SetActive(true);
                break;
            case ManipulationType.Scale:
                redScaleGizmo.SetActive(true);
                greenScaleGizmo.SetActive(true);
                blueScaleGizmo.SetActive(true);
                break;
        }
    }

    public void ClearGizmo()
    {
        lastMousePosition = Vector3.zero;
        selectedObject = null;

        redPositionGizmo.SetActive(false);
        redRotationGizmo.SetActive(false);
        redScaleGizmo.SetActive(false);

        greenPositionGizmo.SetActive(false);
        greenRotationGizmo.SetActive(false);
        greenScaleGizmo.SetActive(false);

        bluePositionGizmo.SetActive(false);
        blueRotationGizmo.SetActive(false);
        blueScaleGizmo.SetActive(false);
    }

    private void GenerateGizmos()
    {
        GenerateGizmoObject(GizmoColor.Red, ManipulationType.Position);
        GenerateGizmoObject(GizmoColor.Red, ManipulationType.Rotation);
        GenerateGizmoObject(GizmoColor.Red, ManipulationType.Scale);
        GenerateGizmoObject(GizmoColor.Green, ManipulationType.Position);
        GenerateGizmoObject(GizmoColor.Green, ManipulationType.Rotation);
        GenerateGizmoObject(GizmoColor.Green, ManipulationType.Scale);
        GenerateGizmoObject(GizmoColor.Blue, ManipulationType.Position);
        GenerateGizmoObject(GizmoColor.Blue, ManipulationType.Rotation);
        GenerateGizmoObject(GizmoColor.Blue, ManipulationType.Scale);
    }

    private void GenerateGizmoObject(GizmoColor gizmoColor, ManipulationType gizmoManipulationType)
    {
        GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newObject.layer = gizmoLayer;

        MeshFilter meshFilter = newObject.GetComponent<MeshFilter>();
        Renderer renderer = newObject.GetComponent<Renderer>();
        meshFilter.mesh = GenerateGizmoMesh(gizmoColor, gizmoManipulationType);

        switch (gizmoColor)
        {
            case GizmoColor.Red:
                renderer.sharedMaterial = redGizmoMaterial;
                switch (gizmoManipulationType)
                {
                    case ManipulationType.Position:
                        redPositionGizmo = newObject;
                        break;
                    case ManipulationType.Rotation:
                        redRotationGizmo = newObject;
                        break;
                    case ManipulationType.Scale:
                        redScaleGizmo = newObject;
                        break;
                }
                break;
            case GizmoColor.Green:
                renderer.sharedMaterial = greenGizmoMaterial;
                switch (gizmoManipulationType)
                {
                    case ManipulationType.Position:
                        greenPositionGizmo = newObject;
                        break;
                    case ManipulationType.Rotation:
                        greenRotationGizmo = newObject;
                        break;
                    case ManipulationType.Scale:
                        greenScaleGizmo = newObject;
                        break;
                }
                break;
            case GizmoColor.Blue:
                renderer.sharedMaterial = blueGizmoMaterial;
                switch (gizmoManipulationType)
                {
                    case ManipulationType.Position:
                        bluePositionGizmo = newObject;
                        break;
                    case ManipulationType.Rotation:
                        blueRotationGizmo = newObject;
                        break;
                    case ManipulationType.Scale:
                        blueScaleGizmo = newObject;
                        break;
                }
                break;
        }

        GizmoType gizmoType = newObject.AddComponent<GizmoType>();
        gizmoType.gizmoColor = gizmoColor;

        BoxCollider collider = newObject.GetComponent<BoxCollider>();
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

    private Mesh GenerateGizmoMesh(GizmoColor gizmoColor, ManipulationType gizmoManipulationType)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        switch (gizmoManipulationType)
        {
            case ManipulationType.Position:
                mesh.triangles = LevelEditorGizmoUtil.positionTriangles;

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

                break;
            case ManipulationType.Rotation:
                mesh.triangles = LevelEditorGizmoUtil.rotationTriangles;

                switch (gizmoColor)
                {
                    case GizmoColor.Red:
                        verts = LevelEditorGizmoUtil.redRotationGizmoVerts.ToList();
                        break;
                    case GizmoColor.Green:
                        verts = LevelEditorGizmoUtil.greenRotationGizmoVerts.ToList();
                        break;
                    case GizmoColor.Blue:
                        verts = LevelEditorGizmoUtil.blueRotationGizmoVerts.ToList();
                        break;
                }

                break;
            case ManipulationType.Scale:
                mesh.triangles = LevelEditorGizmoUtil.scaleTriangles;

                switch (gizmoColor)
                {
                    case GizmoColor.Red:
                        verts = LevelEditorGizmoUtil.redScaleGizmoVerts.ToList();
                        break;
                    case GizmoColor.Green:
                        verts = LevelEditorGizmoUtil.greenScaleGizmoVerts.ToList();
                        break;
                    case GizmoColor.Blue:
                        verts = LevelEditorGizmoUtil.blueScaleGizmoVerts.ToList();
                        break;
                }

                break;
        }
        
        mesh.vertices = verts.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
