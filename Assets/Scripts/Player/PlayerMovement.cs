using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Rigidbody is necessary to ignore certain colliders for portals
public class PlayerMovement : MonoBehaviour
{
    [Header("Set In Editor")]
    public LayerMask layersToIgnore;

    [Header("Debugging properties")]
    [Tooltip("Red line is current velocity, blue is the new direction")]
    public bool showDebugGizmos = false;
    //The velocity applied at the end of every physics frame
    public Vector3 newVelocity;

    [SerializeField]
    private bool grounded;

    private bool wasGrounded;

    [SerializeField]
    private bool crouching;

    private CameraMove cameraMove;
    public Camera ghostCamera;
    public CharacterController controller;

    private Level currentLevel;

    private bool noClip;

    private void Awake()
    {
        newVelocity = Vector3.zero;
        noClip = false;
    }

    private void Start()
    {
        cameraMove = GetComponent<CameraMove>();
        currentLevel = GameManager.GetCurrentLevel();
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        grounded = controller.isGrounded;

        ApplyGravity();
        if (grounded)
        {
            newVelocity.y = 0;
        }
        CheckJump();

        Vector3 inputVector = GetWorldSpaceInputVector();
        Vector3 wishDir = inputVector.normalized;
        float wishSpeed = inputVector.magnitude;


        if (grounded)
        {
            ApplyFriction();
            ApplyGroundAcceleration(wishDir, wishSpeed);
            ClampVelocity(6);
        }
        else
        {
            ApplyAirAcceleration(wishDir, wishSpeed);
        }

        controller.Move(newVelocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (!controller.isGrounded && newVelocity.y > -90)
        {
            newVelocity.y -= 15 * Time.deltaTime;
        }
    }

    private void CheckJump()
    {
        if (grounded && Input.GetKey(KeyCode.Space))
        {
            grounded = false;
            newVelocity.y = 0;
            newVelocity.y += 5.6f;
        }
    }

    private Vector3 GetWorldSpaceInputVector()
    {
        float moveSpeed = 6;

        Vector3 inputVelocity = GetInputVelocity(moveSpeed);
        if (inputVelocity.magnitude > moveSpeed)
        {
            inputVelocity *= moveSpeed / inputVelocity.magnitude;
        }
        //inputVelocity = new Vector3(0, 0, 6);
        //Get the velocity vector in world space coordinates, by rotating around the camera's y-axis
        Vector3 worldSpaceInputVector = Quaternion.AngleAxis(cameraMove.playerCamera.transform.rotation.eulerAngles.y, Vector3.up) * inputVelocity;

        if (Mathf.Abs(worldSpaceInputVector.x) < 0.001)
        {
            worldSpaceInputVector.x = 0;
        }

        if (Mathf.Abs(worldSpaceInputVector.z) < 0.001)
        {
            worldSpaceInputVector.z = 0;
        }

        return worldSpaceInputVector;
    }

    private Vector3 GetInputVelocity(float moveSpeed)
    {
        float horizontalSpeed = 0;
        float verticalSpeed = 0;

        if (Input.GetKey(KeyCode.A))
        {
            horizontalSpeed = -moveSpeed;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontalSpeed = moveSpeed;
        }

        if (Input.GetKey(KeyCode.S))
        {
            verticalSpeed = -moveSpeed;
        }

        if (Input.GetKey(KeyCode.W))
        {
            verticalSpeed = moveSpeed;
        }

        return new Vector3(horizontalSpeed, 0, verticalSpeed);
    }

    private void ApplyFriction()
    {
        var speed = newVelocity.magnitude;

        // Don't apply friction if the player isn't moving
        // Clear speed if it's too low to prevent accidental movement
        // Also makes the player's friction feel more snappy
        if (speed < 0.5f)
        {
            newVelocity = Vector3.zero;
            return;
        }

        // Bleed off some speed, but if we have less than the bleed
        // threshold, bleed the threshold amount.
        var control = (speed < 6) ? 6 : speed;

        // Add the amount to the loss amount.
        var lossInSpeed = control * 6 * Time.deltaTime;
        var newSpeed = Mathf.Max(speed - lossInSpeed, 0);

        if (newSpeed != speed)
        {
            newVelocity *= newSpeed / speed; //Scale velocity based on friction
        }
    }

    private void ApplyGroundAcceleration(Vector3 wishDir, float wishSpeed)
    {
        var currentSpeed = Vector3.Dot(newVelocity, wishDir); //Vector projection of the current velocity onto the new direction
        var speedToAdd = wishSpeed - currentSpeed;

        var acceleration = 20 * Time.deltaTime; //acceleration to apply in the newest direction

        if (speedToAdd <= 0)
        {
            return;
        }

        var accelspeed = Mathf.Min(acceleration * wishSpeed, speedToAdd);
        newVelocity += accelspeed * wishDir; //add acceleration in the new direction
    }

    private void ApplyAirAcceleration(Vector3 wishDir, float wishSpeed)
    {
        var wishSpd = Mathf.Min(wishSpeed, 0.7f);
        Vector3 xzVelocity = newVelocity;
        xzVelocity.y = 0;
        var currentSpeed = Vector3.Dot(xzVelocity, wishDir);
        var speedToAdd = wishSpd - currentSpeed;

        if (speedToAdd <= 0)
        {
            return;
        }

        var accelspeed = Mathf.Min(speedToAdd, 100 * wishSpeed * Time.deltaTime);
        var velocityTransformation = accelspeed * wishDir;

        newVelocity += velocityTransformation;
    }

    private void ClampVelocity(float maxLength)
    {
        newVelocity = Vector3.ClampMagnitude(newVelocity, maxLength);
    }

    public void NoClip()
    {
        noClip = !noClip;
    }

    private void NoClipMove()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += cameraMove.playerCamera.transform.forward * Time.deltaTime * PlayerConstants.NoClipMoveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -cameraMove.playerCamera.transform.forward * Time.deltaTime * PlayerConstants.NoClipMoveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += cameraMove.playerCamera.transform.right * Time.deltaTime * PlayerConstants.NoClipMoveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -cameraMove.playerCamera.transform.right * Time.deltaTime * PlayerConstants.NoClipMoveSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.position += cameraMove.playerCamera.transform.up * Time.deltaTime * PlayerConstants.NoClipMoveSpeed;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.position += -cameraMove.playerCamera.transform.up * Time.deltaTime * PlayerConstants.NoClipMoveSpeed;
        }
    }
}