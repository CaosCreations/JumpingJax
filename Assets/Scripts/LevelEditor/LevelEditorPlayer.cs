using UnityEngine;

public class LevelEditorPlayer : MonoBehaviour
{
    public float moveSpeed = 10f;

    private const float maxMoveSpeed = 500f;
    private const float minMoveSpeed = 5f;
    private const float moveSpeedDelta = 5f;

    private float sensitivityMultiplier;
    private const float maxCameraXRotation = 90;
    private const float halfRotation = 180;
    private const float fullRotation = 360;

    private void Start()
    {
        sensitivityMultiplier = OptionsPreferencesManager.GetSensitivity();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        UpdatePosition();

        if (Input.GetKey(KeyCode.Mouse1))
        {
            UpdateRotation();
        }

        if (InputManager.GetKeyDown(PlayerConstants.LevelEditorSpeedIncrease))
        {
            UpdateSpeed(increasing: true);
        }
        else if (InputManager.GetKeyDown(PlayerConstants.LevelEditorSpeedDecrease))
        {
            UpdateSpeed(increasing: false);
        }
    }

    private void UpdatePosition()
    {
        if (!InputManager.GetKey(PlayerConstants.ModifierKey))
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position += -transform.forward * Time.deltaTime * moveSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += transform.right * Time.deltaTime * moveSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += -transform.right * Time.deltaTime * moveSpeed;
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.position += transform.up * Time.deltaTime * moveSpeed;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                transform.position += -transform.up * Time.deltaTime * moveSpeed;
            }
        }
    }

    private void UpdateRotation()
    {
        // Rotate the camera.
        var rotation = new Vector2(-Input.GetAxis(PlayerConstants.MouseY), Input.GetAxis(PlayerConstants.MouseX));
        var targetEuler = transform.rotation.eulerAngles + (Vector3)rotation * sensitivityMultiplier * 10;
        if (targetEuler.x > halfRotation)
        {
            targetEuler.x -= fullRotation;
        }
        targetEuler.x = Mathf.Clamp(targetEuler.x, -maxCameraXRotation, maxCameraXRotation);
        transform.rotation = Quaternion.Euler(targetEuler);

    }

    private void UpdateSpeed(bool increasing)
    {
        float delta = increasing ? moveSpeedDelta : -moveSpeedDelta;
        moveSpeed = Mathf.Clamp(moveSpeed + delta, minMoveSpeed, maxMoveSpeed);
    }
}
