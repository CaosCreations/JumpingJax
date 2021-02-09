using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum ManipulationType
{
    Position, Rotation, Scale
}

public class Inspector : MonoBehaviour
{
    [Header("Set in Editor")]
    public GameObject container;

    public Text currentObjectName;

    public PrimaryButton positionButton;
    public PrimaryButton rotationButton;
    public PrimaryButton scaleButton;

    public InputField xInput;
    public InputField yInput;
    public InputField zInput;
    public InputField snapInput;
    private InputField[] inputFields;

    [Header("Set at Runtime")]
    public Transform objectToInspect;

    public ManipulationType manipulationType;
    public static ManipulationType manipType;
    public float currentSnap = 1;

    public LevelEditorHUD levelEditorHUD;
    public LevelEditorGizmo levelEditorGizmo;


    public enum InputType
    {
        X, Y, Z
    }
    private void Awake()
    {
        levelEditorHUD = GetComponentInParent<LevelEditorHUD>();
        levelEditorGizmo = GetComponentInParent<LevelEditorGizmo>();
    }

    void Start()
    {
        positionButton.Init(() => SetManipulationType(ManipulationType.Position));
        rotationButton.Init(() => SetManipulationType(ManipulationType.Rotation));
        scaleButton.Init(() => SetManipulationType(ManipulationType.Scale));

        xInput.onValueChanged.RemoveAllListeners();
        xInput.onValueChanged.AddListener((value) => InputChanged(value, InputType.X));

        yInput.onValueChanged.RemoveAllListeners();
        yInput.onValueChanged.AddListener((value) => InputChanged(value, InputType.Y));

        zInput.onValueChanged.RemoveAllListeners();
        zInput.onValueChanged.AddListener((value) => InputChanged(value, InputType.Z));

        snapInput.onValueChanged.RemoveAllListeners();
        snapInput.onValueChanged.AddListener((value) => SnapChanged(value));
        snapInput.text = "0";

        inputFields = new InputField[] { xInput, yInput, zInput, snapInput };

        Clear();
    }

    private void Update()
    {
        if(objectToInspect == null)
        {
            return;
        }
        
        levelEditorGizmo.UpdateGizmos();

        if (levelEditorHUD.isUsingGizmo)
        {
            levelEditorGizmo.GizmoFollowMouse(levelEditorHUD.currentGizmoColor);
            UpdateInputs();
        }

        HandleKeyboardInput();
        CheckInspectorCommands();
    }

    public void InspectObject(Transform toInspect)
    {
        objectToInspect = toInspect;
        currentObjectName.text = objectToInspect.name;
        container.SetActive(true);
        UpdateInputs();
        levelEditorGizmo.SetGizmo(objectToInspect.transform, manipulationType);
    }

    public void Clear()
    {
        levelEditorGizmo.ClearGizmo();
        objectToInspect = null;
        container.SetActive(false);
    }

    private void CheckInspectorCommands()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D))
        {
            GameObject duplicate = Instantiate(objectToInspect.gameObject);
            duplicate.name = objectToInspect.name;
            InspectObject(duplicate.transform);
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            LevelEditorUndo.AddCommand(new DeleteCommand(objectToInspect.gameObject));
            FlipActive(objectToInspect.gameObject);
            Clear();
            // Delete 
        }
    }
    private void SetManipulationType(ManipulationType manipulationType)
    {
        this.manipulationType = manipulationType;
        manipType = manipulationType;
        UpdateInputs();
        levelEditorGizmo.SetGizmo(objectToInspect.transform, manipulationType);

        switch (manipulationType)
        {
            case ManipulationType.Position:
                positionButton.SetActive();
                rotationButton.ClearActive();
                scaleButton.ClearActive();
                break;
            case ManipulationType.Rotation:
                positionButton.ClearActive();
                rotationButton.SetActive();
                scaleButton.ClearActive();
                break;
            case ManipulationType.Scale:
                positionButton.ClearActive();
                rotationButton.ClearActive();
                scaleButton.SetActive();
                break;
        }
    }
    
    private void UpdateInputs()
    {
        switch (manipulationType)
        {
            case ManipulationType.Position:
                xInput.text = objectToInspect.position.x.ToString("F2");
                yInput.text = objectToInspect.position.y.ToString("F2");
                zInput.text = objectToInspect.position.z.ToString("F2");
                break;
            case ManipulationType.Rotation:
                xInput.text = objectToInspect.rotation.eulerAngles.x.ToString("F2");
                yInput.text = objectToInspect.rotation.eulerAngles.y.ToString("F2");
                zInput.text = objectToInspect.rotation.eulerAngles.z.ToString("F2");
                break;
            case ManipulationType.Scale:
                xInput.text = objectToInspect.localScale.x.ToString("F2");
                yInput.text = objectToInspect.localScale.y.ToString("F2");
                zInput.text = objectToInspect.localScale.z.ToString("F2");
                break;
        }
    }

    private void InputChanged(string input, InputType inputType)
    {
        float value = 0;
        float.TryParse(input, out value);

        Vector3 newPosition = objectToInspect.position;
        Vector3 newRotation = objectToInspect.rotation.eulerAngles;
        Vector3 newScale = objectToInspect.localScale;

        switch (inputType)
        {
            case InputType.X:
                if (manipulationType == ManipulationType.Position)
                {
                    newPosition.x = value;
                }
                else if (manipulationType == ManipulationType.Rotation)
                {
                    newRotation.x = value;
                }
                else
                {
                    newScale.x = value;
                }
                break;
            case InputType.Y:
                if (manipulationType == ManipulationType.Position)
                {
                    newPosition.y = value;
                }
                else if (manipulationType == ManipulationType.Rotation)
                {
                    newRotation.y = value;
                }
                else
                {
                    newScale.y = value;
                }
                break;
            case InputType.Z:
                if (manipulationType == ManipulationType.Position)
                {
                    newPosition.z = value;
                }
                else if (manipulationType == ManipulationType.Rotation)
                {
                    newRotation.z = value;
                }
                else
                {
                    newScale.z = value;
                }
                break;
            default:
                return;
        }

        objectToInspect.position = newPosition;
        objectToInspect.rotation = Quaternion.Euler(newRotation);
        objectToInspect.localScale = newScale;
    }

    private void HandleKeyboardInput()
    {
        HandleManipulationKeyboardInput();
        HandleManipulationTypeSelectKeyboardInput();
        HandleSelectAxisKeyboardInput();
    }

    private void HandleManipulationKeyboardInput()
    {
        Vector3 vectorBeingManipulated = Vector3.zero;  
        switch (manipulationType)
        {
            case ManipulationType.Position:
                vectorBeingManipulated = objectToInspect.transform.position;
                break;
            case ManipulationType.Rotation:
                vectorBeingManipulated = objectToInspect.transform.eulerAngles;
                break;
            case ManipulationType.Scale:
                vectorBeingManipulated = objectToInspect.transform.localScale;
                break;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            InputChanged((vectorBeingManipulated.x + 1).ToString(), InputType.X);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            InputChanged((vectorBeingManipulated.x - 1).ToString(), InputType.X);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                InputChanged((vectorBeingManipulated.z + 1).ToString(), InputType.Z);
            }
            else
            {
                InputChanged((vectorBeingManipulated.y + 1).ToString(), InputType.Y);
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                InputChanged((vectorBeingManipulated.z - 1).ToString(), InputType.Z);
            }
            else
            {
                InputChanged((vectorBeingManipulated.y - 1).ToString(), InputType.Y);
            }
        }
        UpdateInputs();
    }

    private void HandleManipulationTypeSelectKeyboardInput()
    {
        if (InputManager.GetModifiedKeyDown(PlayerConstants.LevelEditorSelectPosition))
        {
            SetManipulationType(ManipulationType.Position);
        }
        else if (InputManager.GetModifiedKeyDown(PlayerConstants.LevelEditorSelectRotation))
        {
            SetManipulationType(ManipulationType.Rotation);
        }
        else if (InputManager.GetModifiedKeyDown(PlayerConstants.LevelEditorSelectScale))
        {
            SetManipulationType(ManipulationType.Scale);
        }
    }

    private void HandleSelectAxisKeyboardInput()
    {
        if (InputManager.GetKey(PlayerConstants.ModifierKey))
        {
            // Prevent a navigation keypress from altering the value of the input field itself
            SetInputFieldsReadOnly(true);

            if (InputManager.GetKeyDown(PlayerConstants.LevelEditorSelectXAxis))
            {
                xInput.Select();
            }
            else if (InputManager.GetKeyDown(PlayerConstants.LevelEditorSelectYAxis))
            {
                yInput.Select();
            }
            else if (InputManager.GetKeyDown(PlayerConstants.LevelEditorSelectZAxis))
            {
                zInput.Select();
            }
            else if (InputManager.GetKeyDown(PlayerConstants.LevelEditorSelectSnap))
            {
                snapInput.Select();
            }
        }
        else
        {
            SetInputFieldsReadOnly(false);
        }
    }

    private void SetInputFieldsReadOnly(bool isReadOnly)
    {
        inputFields.All(x => { x.readOnly = isReadOnly; return true; });
    }

    private void SnapChanged(string newSnapValue)
    {
        float value = 0;
        float.TryParse(newSnapValue, out value);
        currentSnap = value;
    }

    public static void FlipActive(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
