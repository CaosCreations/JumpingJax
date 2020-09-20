using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelEditorHUD : MonoBehaviour
{
    public Button prefabViewToggleButton;
    public GameObject prefabScrollView;
    public Transform prefabScrollViewContent;
    public LevelPrefabContainer levelPrefabContainer;
    public GameObject levelButtonPrefab;


    public Button playButton;
    public Button saveButton;
    public Button publishButton;

    public Inspector inspector;

    public Camera levelEditorCamera;
    public GameObject currentSelectedObject;
    public LayerMask gizmoLayerMask;
    public LayerMask selectionLayerMask;
    public Material outlineMaterial;
    public bool isUsingGizmo = false;
    public GizmoColor currentGizmoColor;

    public GameObject playerPrefab;
    public GameObject playerInstance;

    void Start()
    {
        levelEditorCamera = GetComponentInParent<Camera>();
        prefabViewToggleButton.onClick.RemoveAllListeners();
        prefabViewToggleButton.onClick.AddListener(() => TogglePrefabMenu());

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => PlayTest());

        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(() => Save());

        publishButton.onClick.RemoveAllListeners();
        publishButton.onClick.AddListener(() => Publish());

        prefabScrollView.SetActive(false);
        inspector.gameObject.SetActive(false);
        PopulatePrefabMenu();

        playerInstance = Instantiate(playerPrefab);
        playerInstance.SetActive(false);
    }

    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (Input.GetMouseButtonDown(0))
        {
            // Break out if we clicked on the UI, prevents clearing the object when clicking on UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = levelEditorCamera.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit gizmoHit, 1000, gizmoLayerMask))
            {
                isUsingGizmo = true;
                GizmoType tempType = gizmoHit.collider.gameObject.GetComponent<GizmoType>();
                if(tempType == null)
                {
                    return;
                }
                currentGizmoColor = tempType.gizmoColor;
            }
            if (Physics.Raycast(ray, out RaycastHit hit, 1000, selectionLayerMask))
            {
                SelectObject(hit.collider.gameObject);
            }
            else
            {
                UnselectCurrentObject();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isUsingGizmo = false;
        }

        ShowInspector();
        
    }

    private void SelectObject(GameObject objectToSelect)
    {
        if (!objectToSelect.Equals(currentSelectedObject))
        {
            UnselectCurrentObject();
        }
        currentSelectedObject = objectToSelect;
        inspector.InspectObject(currentSelectedObject.transform);
        Renderer renderer = currentSelectedObject.GetComponentInChildren<Renderer>();
        List<Material> currentMaterials = renderer.sharedMaterials.ToList();
        if (!currentMaterials.Contains(outlineMaterial))
        {
            currentMaterials.Insert(0, outlineMaterial);
        }
        renderer.sharedMaterials = currentMaterials.ToArray();
    }

    private void UnselectCurrentObject()
    {
        if (currentSelectedObject == null) {
            return;
        }
        Renderer renderer = currentSelectedObject.GetComponentInChildren<Renderer>();
        List<Material> currentMaterials = renderer.sharedMaterials.ToList();
        for (int i = currentMaterials.Count - 1; i >= 0; i--)
        {
            if (currentMaterials[i].name == "outline")
            {
                currentMaterials.RemoveAt(i);
            }
        }
        renderer.sharedMaterials = currentMaterials.ToArray();
        currentSelectedObject = null;
    }

    private void PlayTest()
    {
        playerInstance.transform.position = transform.parent.position;
        playerInstance.SetActive(!playerInstance.activeInHierarchy);
    }

    private void Save()
    {
        LevelEditorLevel newLevel = new LevelEditorLevel();
        LevelEditorObject[] sceneObjects = FindObjectsOfType<LevelEditorObject>();
        foreach(LevelEditorObject sceneObject in sceneObjects)
        {
            newLevel.levelObjects.Add(sceneObject.GetObjectData());
        }

        string jsonData = JsonUtility.ToJson(newLevel);
        string folderPath = Application.persistentDataPath;
        string filePath = GameManager.GetCurrentLevel().levelEditorScenePath;

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        Debug.Log($"Saving level {GameManager.GetCurrentLevel().levelName} to {filePath}");
        File.WriteAllText(filePath, jsonData);
    }

    private void Publish()
    {
        // Open publish UI, disable publish button until there's at least 2 checkpoint, and a valid name/description
    }

    private void ShowInspector()
    {
        if (currentSelectedObject == null)
        {
            inspector.gameObject.SetActive(false);
        }
        else
        {
            inspector.gameObject.SetActive(true);

        }
    }

    private void TogglePrefabMenu()
    {
        prefabScrollView.SetActive(!prefabScrollView.activeInHierarchy);
    }

    private void PopulatePrefabMenu()
    {
        foreach(LevelPrefab levelPrefab in levelPrefabContainer.levelPrefabs)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab, prefabScrollViewContent);
            LevelEditorPrefabButton newPrefabButton = levelButton.GetComponent<LevelEditorPrefabButton>();
            newPrefabButton.button.onClick.RemoveAllListeners();
            newPrefabButton.button.onClick.AddListener(() => PrefabButtonClicked(levelPrefab));
            newPrefabButton.image.sprite = levelPrefab.previewImage;
        }
    }

    private void PrefabButtonClicked(LevelPrefab levelPrefab)
    {
        GameObject newObject = Instantiate(levelPrefab.prefab);
        newObject.transform.position = levelEditorCamera.transform.position + (levelEditorCamera.transform.forward * 10);
        currentSelectedObject = newObject;
        inspector.InspectObject(currentSelectedObject.transform);
    }

    public void LoadSceneData()
    {
        Level currentLevel = GameManager.GetCurrentLevel();


        if (currentLevel.levelEditorScenePath == string.Empty)
        {
            Debug.Log($"Trying to load level: {currentLevel.levelName} but it has not been saved");
            return;
        }


        if (!File.Exists(currentLevel.levelEditorScenePath))
        {
            Debug.Log($"Trying to load level: {currentLevel.levelName} from {currentLevel.levelEditorScenePath} but the save file has been deleted");
            return;
        }

        string jsonData = File.ReadAllText(currentLevel.levelEditorScenePath);
        LevelEditorLevel levelToLoad = JsonUtility.FromJson<LevelEditorLevel>(jsonData);

        foreach (ObjectData objectData in levelToLoad.levelObjects)
        {
            CreateObject(objectData);
        }
    }

    private void CreateObject(ObjectData objectData)
    {
        LevelPrefab prefabOfType = levelPrefabContainer.levelPrefabs.ToList().Where(x => x.objectType == objectData.objectType).FirstOrDefault();
        if(prefabOfType == null)
        {
            return;
        }

        GameObject newObject = Instantiate(prefabOfType.prefab);

        newObject.transform.position = objectData.position;
        newObject.transform.rotation = Quaternion.Euler(objectData.rotation);
        newObject.transform.localScale = objectData.scale;
    }
}
