using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ObjectTypeTab
{
    Prefab, Required, None
}

public class LevelEditorHUD : MonoBehaviour
{
    [Header("Set In Editor")]
    public GameObject UIContainer;

    public Button prefabViewToggleButton;
    public Button requiredViewToggleButton;

    public GameObject prefabScrollView;
    public Transform prefabScrollViewContent;
    public LevelPrefabContainer levelPrefabContainer;
    public GameObject levelButtonPrefab;

    public Button playButton;
    public Button saveButton;
   
    public LayerMask gizmoLayerMask;
    public LayerMask selectionLayerMask;
    public Material outlineMaterial;

    public GameObject playerPrefab;

    [Header("Set at Runtime")]
    public GameObject playerInstance;
    public GizmoColor currentGizmoColor;
    public bool isUsingGizmo = false;
    public GameObject currentSelectedObject;

    public Inspector inspector;
    public LevelEditorGizmo levelEditorGizmo;
    public Camera levelEditorCamera;
    public CameraMove playerCamera;

    private List<LevelEditorPrefabButton> prefabButtons;
    public ObjectTypeTab currentTab;

    private bool isWorkshopLevel;
    private bool isInPlayMode;

    private void Awake()
    {
        levelEditorCamera = GetComponentInParent<Camera>();
        levelEditorGizmo = GetComponent<LevelEditorGizmo>();
        inspector = GetComponentInChildren<Inspector>();

        playerInstance = Instantiate(playerPrefab);
        playerInstance.SetActive(false);
        playerCamera = playerInstance.GetComponent<CameraMove>();
    }

    void Start()
    {
        isInPlayMode = false;
        currentTab = ObjectTypeTab.None;
        prefabButtons = new List<LevelEditorPrefabButton>();

        prefabViewToggleButton.onClick.RemoveAllListeners();
        prefabViewToggleButton.onClick.AddListener(() => TogglePrefabMenu(ObjectTypeTab.Prefab));

        requiredViewToggleButton.onClick.RemoveAllListeners();
        requiredViewToggleButton.onClick.AddListener(() => TogglePrefabMenu(ObjectTypeTab.Required));

        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => PlayTest());

        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(() => Save());

        prefabScrollView.SetActive(false);
        PopulatePrefabMenu();
    }

    void Update()
    {
        // If we are loading into a workshop level, we don't want the HUD to do anything
        if (isWorkshopLevel)
        {
            return;
        }

        CheckPlayMode();
        if (isInPlayMode)
        {
            return;
        }

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

            // If we click on a gizmo
            if(Physics.Raycast(ray, out RaycastHit gizmoHit, 1000, gizmoLayerMask))
            {
                isUsingGizmo = true;
                GizmoType tempType = gizmoHit.collider.gameObject.GetComponent<GizmoType>();
                if(tempType == null)
                {
                    return;
                }
                currentGizmoColor = tempType.gizmoColor;

                LevelEditorUndo.prevPos = currentSelectedObject.transform.position;
                LevelEditorUndo.prevRotation = currentSelectedObject.transform.rotation;
                LevelEditorUndo.prevScale = currentSelectedObject.transform.localScale;

                return; // break out so that we dont also select an object
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
            if (isUsingGizmo == true)
            {
                AddMovementCommand();
            }
            isUsingGizmo = false;
            levelEditorGizmo.lastMousePosition = Vector3.zero;
        }

        FocusCamera();
    }

    private void FocusCamera()
    {
        if (!InputManager.GetKey(PlayerConstants.ModifierKey) && Input.GetKeyDown(KeyCode.F))
        {
            levelEditorCamera.transform.position = currentSelectedObject.transform.position - (levelEditorCamera.transform.forward * 10);
        }
    }

    private void CheckPlayMode()
    {
        if (InputManager.GetModifiedKeyDown(PlayerConstants.LevelEditorPlayTest))
        {
            PlayTest();
        }
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

        // Add outline material
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
        inspector.Clear();
    }

    private void PlayTest()
    {
        isInPlayMode = !isInPlayMode;

        UnselectCurrentObject();
        requiredViewToggleButton.gameObject.SetActive(!isInPlayMode);
        prefabViewToggleButton.gameObject.SetActive(!isInPlayMode);


        if (!isInPlayMode)
        {
            transform.parent.position = playerCamera.playerCamera.transform.position;
            transform.parent.rotation = playerCamera.playerCamera.transform.rotation;
        }
        else
        {
            SetPlayTestStartingPosition();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
        playerInstance.SetActive(isInPlayMode);

    }

    private void SetPlayTestStartingPosition()
    {
        Checkpoint firstCheckpoint = LevelEditorUtils.GetFirstCheckpoint();
        if(firstCheckpoint != null)
        {
            Transform startingTransform = LevelEditorUtils.GetFirstCheckpoint().transform;
            playerInstance.transform.position =
                startingTransform.position + PlayerConstants.PlayerSpawnOffset;

            playerCamera.SetTargetRotation(startingTransform.rotation);
        }
        else
        {
            playerInstance.transform.position = playerCamera.playerCamera.transform.position;
            playerCamera.SetTargetRotation(transform.parent.rotation);
        }
    }

    private void AddMovementCommand()
    {
        switch (Inspector.manipType)
        {
            case ManipulationType.Position:
                Vector3 position = currentSelectedObject.transform.position;
                LevelEditorUndo.AddCommand(new PositionCommand(currentSelectedObject, position, LevelEditorUndo.prevPos));
                break;
            case ManipulationType.Rotation:
                Quaternion rotation = currentSelectedObject.transform.rotation;
                LevelEditorUndo.AddCommand(new RotateCommand(currentSelectedObject, rotation, LevelEditorUndo.prevRotation));
                break;
            case ManipulationType.Scale:
                Vector3 scale = currentSelectedObject.transform.localScale;
                LevelEditorUndo.AddCommand(new ScaleCommand(currentSelectedObject, scale, LevelEditorUndo.prevScale));
                break;
        }
    }

    #region Prefab Menu
    private void TogglePrefabMenu(ObjectTypeTab tab)
    {
        prefabScrollView.SetActive(true);
        if(tab == currentTab)
        {
            prefabScrollView.SetActive(false);
            currentTab = ObjectTypeTab.None;
            return;
        }

        foreach (LevelEditorPrefabButton button in prefabButtons)
        {
            switch (tab)
            {
                case ObjectTypeTab.Prefab:
                    button.gameObject.SetActive(button.tab == ObjectTypeTab.Prefab);
                    break;
                case ObjectTypeTab.Required:
                    button.gameObject.SetActive(button.tab == ObjectTypeTab.Required);
                    break;
            }
        }

        currentTab = tab;
    }

    private void PopulatePrefabMenu()
    {
        foreach(LevelPrefab levelPrefab in levelPrefabContainer.levelPrefabs)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab, prefabScrollViewContent);
            levelButton.name = levelPrefab.objectName;
            LevelEditorPrefabButton newPrefabButton = levelButton.GetComponent<LevelEditorPrefabButton>();
            newPrefabButton.button.onClick.RemoveAllListeners();
            newPrefabButton.button.onClick.AddListener(() => PrefabButtonClicked(levelPrefab));
            newPrefabButton.image.sprite = levelPrefab.previewImage;
            newPrefabButton.tab = levelPrefab.isRequired ? ObjectTypeTab.Required : ObjectTypeTab.Prefab;
            newPrefabButton.text.text = levelPrefab.objectName;
            prefabButtons.Add(newPrefabButton);
        }
    }

    private void PrefabButtonClicked(LevelPrefab levelPrefab)
    {
        GameObject newObject = Instantiate(levelPrefab.prefab);
        // Set the object 10 units in front of the camera
        newObject.transform.position = levelEditorCamera.transform.position + (levelEditorCamera.transform.forward * 10);
        LevelEditorUndo.AddCommand(new CreateCommand(newObject));
        SelectObject(newObject);
        Save();
    }

    public static void Create(GameObject gameObject, Vector3 position)
    {
        GameObject newObject = Instantiate(gameObject);
        newObject.transform.position = position;
    }
    #endregion

    #region Workshop Level
    private void Save()
    {
        // Don't save if we don't have a level data object to save to
        // This happens when opening the scene manually
        if (GameManager.Instance != null)
        {
            LevelEditorLevel newLevel = new LevelEditorLevel();
            LevelEditorObject[] sceneObjects = FindObjectsOfType<LevelEditorObject>();
            foreach (LevelEditorObject sceneObject in sceneObjects)
            {
                if(sceneObject.gameObject.activeSelf == true)
                {
                    newLevel.levelObjects.Add(sceneObject.GetObjectData());
                }
                else
                {
                    Destroy(sceneObject.gameObject);
                }
            }

            string jsonData = JsonUtility.ToJson(newLevel, true);

            string filePath = GameManager.GetCurrentLevel().levelEditorLevelDataPath;
            Debug.Log($"Saving level {GameManager.GetCurrentLevel().levelName} to {filePath}");
            try
            {
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception e)
            {
                Debug.LogError($"LevelEditorHud.Save(): Could not save level {e.Message}\n{e.StackTrace}");
            }
        }
    }

    public void LoadSceneData()
    {
        Level currentLevel = GameManager.GetCurrentLevel();

        string filePath = "";
        if (currentLevel.workshopFilePath != string.Empty && currentLevel.workshopFilePath != null)
        {
            DirectoryInfo fileInfo = new DirectoryInfo(currentLevel.workshopFilePath);
            try
            {
                List<DirectoryInfo> levelDataFolders = fileInfo.EnumerateDirectories().ToList();
                DirectoryInfo levelDataFolder = levelDataFolders.First();
                List<FileInfo> levelDataFiles = levelDataFolder.EnumerateFiles("*.json").ToList();
                FileInfo levelDataFile = levelDataFiles.First();
                filePath = levelDataFile.FullName;
            }
            catch(Exception e)
            {
                Debug.LogError($"Could not find any file for workshop folder {fileInfo}. Files may not have been downloaded.\nException\n{e.Message}");
            }
        }
        else
        {
            if (currentLevel.levelEditorLevelDataPath == string.Empty)
            {
                Debug.LogError($"Trying to load level: {currentLevel.levelName} but it has not been saved");
                return;
            }


            if (!File.Exists(currentLevel.levelEditorLevelDataPath))
            {
                Debug.LogError($"Trying to load level: {currentLevel.levelName} from {currentLevel.levelEditorLevelDataPath} but the save file has been deleted");
                return;
            }
            filePath = currentLevel.levelEditorLevelDataPath;
        }

        try
        {
            string jsonData = File.ReadAllText(filePath);
            LevelEditorLevel levelToLoad = JsonUtility.FromJson<LevelEditorLevel>(jsonData);

            CreateObjects(levelToLoad.levelObjects);

            if (currentLevel.workshopFilePath != string.Empty && currentLevel.workshopFilePath != null)
            {
                SetupForWorkshopLevel();
            }
        }catch(Exception e)
        {
            Debug.LogError($"LevelEditorHud.LoadSceneData(): Could not load level {e.Message}\n{e.StackTrace}");
        }
        
    }

    private void CreateObjects(List<ObjectData> allObjects)
    {
        foreach (ObjectData objectData in allObjects)
        {
            LevelPrefab prefabOfType = levelPrefabContainer.levelPrefabs.ToList().Where(x => x.objectType == objectData.objectType).FirstOrDefault();
            if (prefabOfType == null)
            {
                return;
            }

            GameObject newObject = Instantiate(prefabOfType.prefab);

            newObject.transform.position = objectData.position;
            newObject.transform.rotation = Quaternion.Euler(objectData.rotation);
            newObject.transform.localScale = objectData.scale;
        }
    }

    private void SetupForWorkshopLevel()
    {
        isWorkshopLevel = true;

        Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
        foreach(Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint.isFirstCheckpoint)
            {
                playerInstance.transform.position = checkpoint.gameObject.transform.position + PlayerConstants.PlayerSpawnOffset;
            }
        }
        playerInstance.SetActive(true);
        gameObject.SetActive(false);
        levelEditorGizmo.ClearGizmo();
    }
    #endregion
}
