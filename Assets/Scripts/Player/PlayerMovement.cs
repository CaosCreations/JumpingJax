using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Set In Editor")]
    public LayerMask layersToIgnore;

    [Header("Debugging properties")]
    [Tooltip("Red line is current velocity, blue is the new direction")]
    public bool showDebugGizmos = false;
    //The velocity applied at the end of every physics frame
    public Vector3 velocityToApply;
    public Vector3 currentInput;

    [SerializeField]
    private bool grounded;

    [SerializeField]
    private bool crouching;

    public CharacterController controller;
    private PlayerPortalableController playerPortalableController;
    private CameraMove cameraMove;
    public Camera ghostCamera;
    private Level currentLevel;

    private bool noClip;
    public Vector3 currentVelocity; // This result is the finalized value of velocityToApply, used for GhostVelocity value

    private void Awake()
    {
        velocityToApply = Vector3.zero;
        currentVelocity = velocityToApply;
        noClip = false;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerPortalableController = GetComponent<PlayerPortalableController>();
        cameraMove = GetComponent<CameraMove>();
        currentLevel = GameManager.GetCurrentLevel();
    }

    // All input checking going in Update, so no Input queries are missed
    private void Update()
    {
        if ((ghostCamera != null && ghostCamera.enabled) || GameManager.Instance.isLoadingScene)
        {
            return;
        }

        if (noClip)
        {
            NoClipMove();
            return;
        }

        SetGrounded();
        CheckCrouch();

        ApplyGravity();

        CheckJump();

        currentInput = GetWorldSpaceInputVector();
        currentVelocity = velocityToApply;
        controller.Move(velocityToApply * Time.deltaTime);

        CheckFootstepSound();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // If we hit a wall, make us lose speed so we can't continuously jump into a wall and gain speed
        if(controller.collisionFlags == CollisionFlags.Sides && hit.normal.y < 0.7f)
        {
            velocityToApply = ClipVelocity(hit.normal);
        }
    }

    private void FixedUpdate()
    {
        Vector3 wishDir = currentInput.normalized;
        float wishSpeed = currentInput.magnitude;

        if (grounded)
        {
            if (IsPlayerWalkingBackwards())
            {
                wishSpeed *= PlayerConstants.BackWardsMoveSpeedScale;
            }
            ApplyFriction();
            ApplyGroundAcceleration(wishDir, wishSpeed, PlayerConstants.NormalSurfaceFriction);
        }
        else
        {
            ApplyAirAcceleration(wishDir, wishSpeed);
        }

        ClampVelocity(PlayerConstants.MaxVelocity);
    }

    private void SetGrounded()
    {
        CheckLandingSound();

        grounded = controller.isGrounded;

        // If we are falling into a portal, make sure we don't clip with the ground
        if((velocityToApply.y < -1 || velocityToApply.y > 1) && playerPortalableController.IsInPortal())
        {
            grounded = false;
        }

        if (controller.collisionFlags == CollisionFlags.CollidedAbove && velocityToApply.y > 0 && !playerPortalableController.IsInPortal())
        {
            velocityToApply.y = 0;
        }
    }

    #region Crouch
    private void CheckCrouch()
    {
        if (InputManager.GetKey(PlayerConstants.Crouch))
        {
            crouching = true;
        }
        else
        {
            // If we are already crouching, check if we need to stay crouching (something is above the player)
            if (crouching)
            {
                crouching = !CanUncrouch();
            }
            else
            {
                crouching = false;
            }
        }

        // Resize the player bounding box
        DampenCollider();

        // Move the camera to the correct offset
        DampenCamera();
    }

    private bool CanUncrouch()
    {
        // Get the vertical distance covered when uncrouching
        float castDistance = (PlayerConstants.StandingPlayerHeight - PlayerConstants.CrouchingPlayerHeight) + 0.01f;
        RaycastHit hit;
        Ray ray = new Ray(transform.position + new Vector3(0, PlayerConstants.CrouchingPlayerHeight / 2, 0), Vector3.up);
        if (Physics.Raycast(ray, out hit, castDistance, layersToIgnore))
        {
            return false;
        }
        return true;
    }

    private void DampenCollider()
    {
        // Update player collider
        float endHeight = crouching ? PlayerConstants.CrouchingPlayerHeight : PlayerConstants.StandingPlayerHeight;
        float velocity = 0;
        float startingHeight = controller.height;
        float height = Mathf.SmoothDamp(controller.height, endHeight, ref velocity, Time.deltaTime);

        if(height > startingHeight && grounded)
        {
            Vector3 newPosition = transform.position;
            newPosition.y += height - startingHeight;
            transform.position = newPosition;
        }

        controller.height = height;
    }

    private void DampenCamera()
    {
        Vector3 endOffset = crouching ? PlayerConstants.CrouchingCameraOffset : PlayerConstants.StandingCameraOffset;
        Vector3 currentOffset = cameraMove.playerCamera.transform.localPosition;
        float v = 0;
        float yOffset = Mathf.SmoothDamp(currentOffset.y, endOffset.y, ref v, Time.deltaTime);
        Vector3 newOffset = new Vector3(0, yOffset, 0);
        cameraMove.playerCamera.transform.localPosition = newOffset;
    }
    #endregion

    private void ApplyGravity()
    {
        if (!grounded && velocityToApply.y > -PlayerConstants.MaxFallSpeed)
        {
            velocityToApply.y -= currentLevel.gravityMultiplier * PlayerConstants.Gravity * Time.deltaTime;
        }

        CheckGravitySound();
    }

    private void CheckJump()
    {
        if (grounded && InputManager.GetKey(PlayerConstants.Jump))
        {
            RaycastHit hit;
            Vector3 startPos = transform.position - new Vector3(0, controller.height / 2, 0);
            if (Physics.Raycast(startPos, Vector3.down, out hit, 0.301f, layersToIgnore, QueryTriggerInteraction.Ignore) && hit.normal.y > 0.7f)
            {
                Vector3 clippedVelocity = ClipVelocity(hit.normal);
                clippedVelocity.y = 0;

                Vector3 temp = velocityToApply;
                temp.y = 0;
                if (Vector3.Dot(clippedVelocity, temp) > 0 && clippedVelocity.magnitude >= temp.magnitude)
                {
                    velocityToApply = clippedVelocity;
                }
            }

            velocityToApply.y = 0;
            velocityToApply.y += crouching ? PlayerConstants.CrouchingJumpPower : PlayerConstants.JumpPower;
            grounded = false;
        }
    }

    #region Input
    private Vector3 GetWorldSpaceInputVector()
    {
        float moveSpeed = crouching ? PlayerConstants.CrouchingMoveSpeed : PlayerConstants.MoveSpeed;

        Vector3 inputVelocity = GetInputVelocity(moveSpeed);
        if (inputVelocity.magnitude > moveSpeed)
        {
            inputVelocity *= moveSpeed / inputVelocity.magnitude;
        }

        //Get the velocity vector in world space coordinates, by rotating around the camera's y-axis
        return Quaternion.AngleAxis(cameraMove.playerCamera.transform.rotation.eulerAngles.y, Vector3.up) * inputVelocity;
    }

    private Vector3 GetInputVelocity(float moveSpeed)
    {
        float horizontalSpeed = 0;
        float verticalSpeed = 0;

        if (InputManager.GetKey(PlayerConstants.Left))
        {
            horizontalSpeed -= moveSpeed;
        }

        if (InputManager.GetKey(PlayerConstants.Right))
        {
            horizontalSpeed += moveSpeed;
        }

        if (InputManager.GetKey(PlayerConstants.Back))
        {
            verticalSpeed -= moveSpeed;
        }

        if (InputManager.GetKey(PlayerConstants.Forward))
        {
            verticalSpeed += moveSpeed;
        }

        return new Vector3(horizontalSpeed, 0, verticalSpeed);
    }

    private bool IsPlayerWalkingBackwards()
    {
        Vector3 inputDirection = GetInputVelocity(PlayerConstants.MoveSpeed);

        return inputDirection.z < 0;
    }
    #endregion

    //wishDir: the direction the player wishes to go in the newest frame
    //wishSpeed: the speed the player wishes to go this frame
    private void ApplyGroundAcceleration(Vector3 wishDir, float wishSpeed, float surfaceFriction)
    {
        var currentSpeed = Vector3.Dot(velocityToApply, wishDir); //Vector projection of the current velocity onto the new direction
        var speedToAdd = wishSpeed - currentSpeed;

        var acceleration = PlayerConstants.GroundAcceleration * Time.deltaTime; //acceleration to apply in the newest direction

        if (speedToAdd <= 0)
        {
            return;
        }

        var accelspeed = Mathf.Min(acceleration * wishSpeed * surfaceFriction, speedToAdd);
        velocityToApply += accelspeed * wishDir; //add acceleration in the new direction
    }

    //wishDir: the direction the player  wishes to goin the newest frame
    //wishSpeed: the speed the player wishes to go this frame
    private void ApplyAirAcceleration(Vector3 wishDir, float wishSpeed)
    {
        var wishSpd = Mathf.Min(wishSpeed, PlayerConstants.AirAccelerationCap);
        Vector3 xzVelocity = velocityToApply;
        xzVelocity.y = 0;
        var currentSpeed = Vector3.Dot(xzVelocity, wishDir);
        var speedToAdd = wishSpd - currentSpeed;

        if (speedToAdd <= 0)
        {
            return;
        }

        var accelspeed = Mathf.Min(speedToAdd, PlayerConstants.AirAcceleration * wishSpeed * Time.deltaTime);
        var velocityTransformation = accelspeed * wishDir;

        velocityToApply += velocityTransformation;
    }

    private void ApplyFriction()
    {
        var speed = velocityToApply.magnitude;

        // Don't apply friction if the player isn't moving
        // Clear speed if it's too low to prevent accidental movement
        // Also makes the player's friction feel more snappy
        if (speed < PlayerConstants.MinimumSpeedCutoff)
        {
            Vector3 noVelocity = velocityToApply; // don't reset y velocity
            noVelocity.x = 0;
            noVelocity.z = 0;
            velocityToApply = noVelocity;
            return;
        }

        // Bleed off some speed, but if we have less than the bleed
        // threshold, bleed the threshold amount.
        var control = (speed < PlayerConstants.StopSpeed) ? PlayerConstants.StopSpeed : speed;

        // Add the amount to the loss amount.
        var lossInSpeed = control * PlayerConstants.Friction * Time.deltaTime;
        var newSpeed = Mathf.Max(speed - lossInSpeed, 0);

        if (newSpeed != speed)
        {
            velocityToApply.x *= newSpeed / speed; //Scale velocity based on friction
            velocityToApply.z *= newSpeed / speed; //Scale velocity based on friction
        }
    }

    // This function keeps the player from exceeding a maximum velocity
    private void ClampVelocity(float maxLength)
    {
        velocityToApply = Vector3.ClampMagnitude(velocityToApply, maxLength);
    }

    private Vector3 ClipVelocity(Vector3 normal)
    {
        Vector3 toReturn = velocityToApply;

        // Determine how far along plane to slide based on incoming direction.
        float backoff = Vector3.Dot(velocityToApply, normal);

        var change = normal * backoff;
        toReturn -= change;

        // iterate once to make sure we aren't still moving through the plane
        float adjust = Vector3.Dot(toReturn, normal);
        if (adjust < 0)
        {
            toReturn -= (normal * adjust);
        }

        return toReturn;
    }

    public void NoClip()
    {
        noClip = !noClip;
        controller.enabled = !noClip;
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

    #region SoundChecks
    private void CheckGravitySound()
    {
        if (velocityToApply.y <= -PlayerConstants.MaxFallSpeed)
        {
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.TerminalVelocity);
        }
    }

    private void CheckLandingSound()
    {
        if (!grounded && controller.isGrounded)
        {
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Land);
        }
    }

    private void CheckFootstepSound()
    {
        Vector3 horizontalVelocty = velocityToApply;
        horizontalVelocty.y = 0;
        if (grounded && horizontalVelocty.magnitude > 1)
        {
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Footstep);
        }
    }
    #endregion
}