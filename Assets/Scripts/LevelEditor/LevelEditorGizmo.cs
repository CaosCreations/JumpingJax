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
    public Inspector inspector;

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

    public Vector3 lastMousePosition;

    void Awake()
    {
        GenerateGizmos();
        lastMousePosition = Vector3.zero;
        mainCamera = GetComponentInParent<Camera>();
        inspector = GetComponentInChildren<Inspector>();
    }

    public void GizmoFollowMouse(GizmoColor gizmoColor)
    {
        switch (manipulationType)
        {
            case ManipulationType.Position:
                GizmoFollowPosition(gizmoColor);
                break;
            case ManipulationType.Rotation:
                GizmoFollowRotation(gizmoColor);
                break;
            case ManipulationType.Scale:
                GizmoFollowScale(gizmoColor);
                break;
        }
    }

    private void GizmoFollowPosition(GizmoColor gizmoColor)
    {
        float distanceFromCameraToObject = (mainCamera.transform.position - selectedObject.position).magnitude;

        // Clamp the distance so the object doesn't go too far away
        if(distanceFromCameraToObject > 50)
        {
            distanceFromCameraToObject = 50;
        }

        // Convert the mouse position to the object's position
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCameraToObject));

        if (lastMousePosition == Vector3.zero)
        {
            lastMousePosition = worldPoint;
            return;
        }

        Vector3 mouseDelta = worldPoint - lastMousePosition;
        Vector3 newMouseChange = Vector3.zero;

        switch (gizmoColor)
        {
            case GizmoColor.Red:
                if(inspector.currentSnap == 0)
                {
                    newMouseChange = new Vector3(mouseDelta.x, 0, 0);
                }
                else if (Mathf.Abs(mouseDelta.x) > inspector.currentSnap)
                {
                    newMouseChange = new Vector3(Mathf.Clamp(mouseDelta.x, -inspector.currentSnap, inspector.currentSnap), 0, 0);
                }
                else
                {
                    return;
                }
                break;
            case GizmoColor.Green:
                if (inspector.currentSnap == 0)
                {
                    newMouseChange = new Vector3(0, mouseDelta.y, 0);
                }
                else if (Mathf.Abs(mouseDelta.y) > inspector.currentSnap)
                {
                    newMouseChange = new Vector3(0, Mathf.Clamp(mouseDelta.y, -inspector.currentSnap, inspector.currentSnap), 0);
                }
                else
                {
                    return;
                }
                break;
            case GizmoColor.Blue:
                if (inspector.currentSnap == 0)
                {
                    newMouseChange = new Vector3(0, 0, mouseDelta.z);
                }
                else if (Mathf.Abs(mouseDelta.z) > inspector.currentSnap)
                {
                    newMouseChange = new Vector3(0, 0, Mathf.Clamp(mouseDelta.z, -inspector.currentSnap, inspector.currentSnap));
                }
                else
                {
                    return;
                }
                break;
        }
        selectedObject.position += newMouseChange;
        lastMousePosition = worldPoint;   
    }

    private void GizmoFollowRotation(GizmoColor gizmoColor)
    {
        float distanceFromCameraToObject = (mainCamera.transform.position - selectedObject.position).magnitude;
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCameraToObject));

        if (lastMousePosition == Vector3.zero)
        {
            lastMousePosition = worldPoint;
            return;
        }

        Vector3 mouseDelta = worldPoint - lastMousePosition;
        Vector3 newMouseChange = Vector3.zero;

        switch (gizmoColor)
        {
            case GizmoColor.Red:
                if (inspector.currentSnap == 0)
                {
                    newMouseChange = new Vector3(mouseDelta.x, 0, 0);
                }
                else if (Mathf.Abs(mouseDelta.x) > inspector.currentSnap)
                {
                    newMouseChange = new Vector3(Mathf.Clamp(mouseDelta.x, -inspector.currentSnap, inspector.currentSnap), 0, 0);
                }
                else
                {
                    return;
                }
                break;
            case GizmoColor.Green:
                if (inspector.currentSnap == 0)
                {
                    newMouseChange = new Vector3(0, mouseDelta.y, 0);
                }
                else if (Mathf.Abs(mouseDelta.y) > inspector.currentSnap)
                {
                    newMouseChange = new Vector3(0, Mathf.Clamp(mouseDelta.y, -inspector.currentSnap, inspector.currentSnap), 0);
                }
                else
                {
                    return;
                }
                break;
            case GizmoColor.Blue:
                if (inspector.currentSnap == 0)
                {
                    newMouseChange = new Vector3(0, 0, mouseDelta.z);
                }
                else if (Mathf.Abs(mouseDelta.z) > inspector.currentSnap)
                {
                    newMouseChange = new Vector3(0, 0, Mathf.Clamp(mouseDelta.z, -inspector.currentSnap, inspector.currentSnap));
                }
                else
                {
                    return;
                }
                break;
        }
        selectedObject.Rotate(newMouseChange * 2);
        lastMousePosition = worldPoint;
    }


    private void GizmoFollowScale(GizmoColor gizmoColor)
    {
        float distanceFromCameraToObject = (mainCamera.transform.position - selectedObject.position).magnitude;
        Vector3 worldPoint = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceFromCameraToObject));

        if (lastMousePosition == Vector3.zero)
        {
            lastMousePosition = worldPoint;
            return;
        }

        Vector3 mouseDelta = worldPoint - lastMousePosition;
        Vector3 newMouseChange = Vector3.zero;

        switch (gizmoColor)
        {
            case GizmoColor.Red:
                if (inspector.currentSnap == 0)
                {
                    newMouseChange = new Vector3(mouseDelta.x, 0, 0);
                }
                else if (Mathf.Abs(mouseDelta.x) > inspector.currentSnap)
                {
                    newMouseChange = new Vector3(Mathf.Clamp(mouseDelta.x, -inspector.currentSnap, inspector.currentSnap), 0, 0);
                }
                else
                {
                    return;
                }
                break;
            case GizmoColor.Green:
                if (inspector.currentSnap == 0)
                {
                    newMouseChange = new Vector3(0, mouseDelta.y, 0);
                }
                else if (Mathf.Abs(mouseDelta.y) > inspector.currentSnap)
                {
                    newMouseChange = new Vector3(0, Mathf.Clamp(mouseDelta.y, -inspector.currentSnap, inspector.currentSnap), 0);
                }
                else
                {
                    return;
                }
                break;
            case GizmoColor.Blue:
                if (inspector.currentSnap == 0)
                {
                    newMouseChange = new Vector3(0, 0, mouseDelta.z);
                }
                else if(Mathf.Abs(mouseDelta.z) > inspector.currentSnap)
                {
                    newMouseChange = new Vector3(0, 0, Mathf.Clamp(mouseDelta.z, -inspector.currentSnap, inspector.currentSnap));
                }
                else
                {
                    return;
                }
                break;
        }
        selectedObject.localScale += newMouseChange;
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
        ClearGizmo();
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

        // Handle when Inspector.OnDisable() tries to destroy objects as this script is killed first
        if(redPositionGizmo == null 
            || redRotationGizmo == null
            || redScaleGizmo == null
            || greenPositionGizmo == null
            || greenRotationGizmo == null
            || greenScaleGizmo == null
            || bluePositionGizmo == null
            || blueRotationGizmo == null
            || blueScaleGizmo == null)
        {
            return;
        }
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
        newObject.layer = PlayerConstants.GizmoLayer;

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
                switch (gizmoColor)
                {
                    case GizmoColor.Red:
                        mesh.vertices = LevelEditorGizmoUtil.redPositionGizmoVerts;

                        break;
                    case GizmoColor.Green:
                        mesh.vertices = LevelEditorGizmoUtil.greenPositionGizmoVerts;

                        break;
                    case GizmoColor.Blue:
                        mesh.vertices = LevelEditorGizmoUtil.bluePositionGizmoVerts;

                        break;
                }
                mesh.triangles = LevelEditorGizmoUtil.positionTriangles;
                break;
            case ManipulationType.Rotation:
                switch (gizmoColor)
                {
                    case GizmoColor.Red:
                        mesh.vertices = LevelEditorGizmoUtil.redRotationGizmoVerts;

                        break;
                    case GizmoColor.Green:
                        mesh.vertices = LevelEditorGizmoUtil.greenRotationGizmoVerts;

                        break;
                    case GizmoColor.Blue:
                        mesh.vertices = LevelEditorGizmoUtil.blueRotationGizmoVerts;

                        break;
                }
                mesh.triangles = LevelEditorGizmoUtil.rotationTriangles;
                break;
            case ManipulationType.Scale:
                switch (gizmoColor)
                {
                    case GizmoColor.Red:
                        mesh.vertices = LevelEditorGizmoUtil.redScaleGizmoVerts;

                        break;
                    case GizmoColor.Green:
                        mesh.vertices = LevelEditorGizmoUtil.greenScaleGizmoVerts;

                        break;
                    case GizmoColor.Blue:
                        mesh.vertices = LevelEditorGizmoUtil.blueScaleGizmoVerts;
                        break;
                }
                mesh.triangles = LevelEditorGizmoUtil.scaleTriangles;
                break;
        }
        
        mesh.RecalculateNormals();
        return mesh;
    }
}
