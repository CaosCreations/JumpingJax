using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UI;

public enum ManipulationType
{
    Position, Rotation, Scale
}

public class Inspector : MonoBehaviour
{
    public Button positionButton;
    public Button rotationButton;
    public Button scaleButton;

    public InputField xInput;
    public InputField yInput;
    public InputField zInput;
    public InputField snapInput;

    public GameObject customDataContainer;
    public Text customLabel;
    public InputField customInput;
    public CustomDataType currentCustomType;

    public enum CustomDataType
    {
        Checkpoint
    }

    public Transform objectToInspect;

    public ManipulationType manipulationType;
    private float currentSnap = 1;

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
        positionButton.onClick.RemoveAllListeners();
        positionButton.onClick.AddListener(() => SetManipulationType(ManipulationType.Position));

        rotationButton.onClick.RemoveAllListeners();
        rotationButton.onClick.AddListener(() => SetManipulationType(ManipulationType.Rotation));

        scaleButton.onClick.RemoveAllListeners();
        scaleButton.onClick.AddListener(() => SetManipulationType(ManipulationType.Scale));
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
    }

    private void OnDisable()
    {
        levelEditorGizmo.ClearGizmo();
    }

    public void InspectObject(Transform toInspect)
    {
        objectToInspect = toInspect;
        UpdateInputs();
        ShowTransformGizmo();

        xInput.onValueChanged.RemoveAllListeners();
        xInput.onValueChanged.AddListener((value) => InputChanged(value, InputType.X));

        yInput.onValueChanged.RemoveAllListeners();
        yInput.onValueChanged.AddListener((value) => InputChanged(value, InputType.Y));

        zInput.onValueChanged.RemoveAllListeners();
        zInput.onValueChanged.AddListener((value) => InputChanged(value, InputType.Z));

        snapInput.onValueChanged.RemoveAllListeners();
        snapInput.onValueChanged.AddListener((value) => SnapChanged(value));

        AddCustomDataInspector();
    }

    private void AddCustomDataInspector()
    {
        customDataContainer.SetActive(false);

        Checkpoint checkpoint = objectToInspect.GetComponent<Checkpoint>();
        if (checkpoint != null)
        {
            customDataContainer.SetActive(true);
            customLabel.text = "Checkpoint order: ";
            customInput.text = "0";
            customInput.onValueChanged.RemoveAllListeners();
            customInput.onValueChanged.AddListener((value) => CustomInputChanged(value));
            currentCustomType = CustomDataType.Checkpoint;
        }

    }

    private void CustomInputChanged(string newValue)
    {
        switch (currentCustomType)
        {
            case CustomDataType.Checkpoint:
                //int.TryParse(newValue)
                break;
        }
    }

    private void SetManipulationType(ManipulationType manipulationType)
    {
        this.manipulationType = manipulationType;
        UpdateInputs();
        ShowTransformGizmo();
    }
    
    private void UpdateInputs()
    {
        switch (manipulationType)
        {
            case ManipulationType.Position:
                xInput.text = objectToInspect.position.x.ToString();
                yInput.text = objectToInspect.position.y.ToString();
                zInput.text = objectToInspect.position.z.ToString();
                break;
            case ManipulationType.Rotation:
                xInput.text = objectToInspect.rotation.eulerAngles.x.ToString();
                yInput.text = objectToInspect.rotation.eulerAngles.y.ToString();
                zInput.text = objectToInspect.rotation.eulerAngles.z.ToString();
                break;
            case ManipulationType.Scale:
                xInput.text = objectToInspect.localScale.x.ToString();
                yInput.text = objectToInspect.localScale.y.ToString();
                zInput.text = objectToInspect.localScale.z.ToString();
                break;
        }
    }

    private void ShowTransformGizmo()
    {
        levelEditorGizmo.SetGizmo(objectToInspect.transform, manipulationType);
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

    private void SnapChanged(string newSnapValue)
    {
        float value = 0;
        float.TryParse(newSnapValue, out value);
        currentSnap = value;
    }
}
