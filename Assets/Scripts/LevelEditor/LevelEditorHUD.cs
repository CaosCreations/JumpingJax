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
    public Button playButton;
    public Button saveButton;
    public Button publishButton;
    public GameObject prefabScrollView;
    public Transform prefabScrollViewContent;
    public LevelPrefabContainer levelPrefabContainer;
    public GameObject levelButtonPrefab;
    public GameObject selectedObjectGizmo;

    public Inspector inspector;

    public Camera camera;
    public GameObject currentSelectedObject;
    public LayerMask selectionLayerMask;
    public Material outlineMaterial;

    void Start()
    {
        camera = GetComponentInParent<Camera>();
        prefabViewToggleButton.onClick.RemoveAllListeners();
        prefabViewToggleButton.onClick.AddListener(() => TogglePrefabMenu());

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => PlayTest());

        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(() => Save());

        publishButton.onClick.RemoveAllListeners();
        publishButton.onClick.AddListener(() => Save());

        prefabScrollView.SetActive(false);
        selectedObjectGizmo.SetActive(false);
        inspector.gameObject.SetActive(false);
        PopulatePrefabMenu();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Break out if we clicked on the UI, prevents clearing the object when clicking on UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            UnselectCurrentObject();

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000, selectionLayerMask))
            {
                SelectObject(hit.collider.gameObject);
            }
        }

        ShowTransformGizmo();
        ShowInspector();
        
    }

    private void SelectObject(GameObject objectToSelect)
    {
        currentSelectedObject = objectToSelect;
        inspector.InspectObject(currentSelectedObject.transform);
        Renderer renderer = currentSelectedObject.GetComponent<Renderer>();
        List<Material> currentMaterials = renderer.sharedMaterials.ToList();
        if (!currentMaterials.Contains(outlineMaterial))
        {
            currentMaterials.Add(outlineMaterial);
        }
        renderer.sharedMaterials = currentMaterials.ToArray();
    }

    private void UnselectCurrentObject()
    {
        if (currentSelectedObject == null) {
            return;
        }
        Renderer renderer = currentSelectedObject.GetComponent<Renderer>();
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
        // Create player from prefab, set the main camera
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

    private void ShowTransformGizmo()
    {
        if(currentSelectedObject == null)
        {
            selectedObjectGizmo.SetActive(false);
            return;
        }
        selectedObjectGizmo.SetActive(true);
        selectedObjectGizmo.transform.position = currentSelectedObject.transform.position;
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
        newObject.transform.position = camera.transform.position + (camera.transform.forward * 10);
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
