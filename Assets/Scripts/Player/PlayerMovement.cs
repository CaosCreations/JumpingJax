using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))] // Collider is necessary for custom collision detection
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

    private BoxCollider myCollider;
    private CameraMove cameraMove;
    public Camera ghostCamera;
    private Level currentLevel;

    private bool noClip;

    private void Awake()
    {
        newVelocity = Vector3.zero;
        noClip = false;
    }

    private void Start()
    {
        myCollider = GetComponent<BoxCollider>();
        cameraMove = GetComponent<CameraMove>();
        currentLevel = GameManager.GetCurrentLevel();
    }

    private void FixedUpdate()
    {
        if (ghostCamera.enabled || GameManager.Instance.isLoadingScene)
        {
            return;
        }

        if (noClip)
        {
            NoClipMove();
            return;
        }

        CheckCrouch();

        ApplyGravity();

        CheckGrounded();
        CheckCeiling();
        CheckJump();

        var inputVector = GetWorldSpaceInputVector();
        var wishDir = inputVector.normalized;
        var wishSpeed = inputVector.magnitude;

        if (grounded)
        {
            if (IsPlayerWalkingBackwards())
            {
                wishSpeed *= PlayerConstants.BackWardsMoveSpeedScale;
            }
            ApplyFriction();
            ApplyGroundAcceleration(wishDir, wishSpeed, PlayerConstants.NormalSurfaceFriction);
            ClampVelocity(PlayerConstants.MoveSpeed);
            CheckFootstepSound();
            ExecuteGroundMovement();
        }
        else
        {
            ApplyAirAcceleration(wishDir, wishSpeed);
            ExecuteAirMovement();
        }

        FrameEndCleanup();
    }

    private void FrameEndCleanup()
    {
        // Since the StepMove() function could take the player off the ground, we need to check again at the end of the frame
        // but only if we are falling, otherwise it shouldn't be necessary
        if (newVelocity.y <= 0)
        {
            CheckGrounded();
        }

        ClampVelocity(PlayerConstants.MaxVelocity);

        if (grounded)
        {
            newVelocity.y = 0;
        }
    }

    private void CheckFootstepSound()
    {

        if (newVelocity.magnitude > 0)
        {
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Footstep);
        }
    }

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

    private void DampenCollider()
    {
        // Update player collider
        float endHeight = crouching ? PlayerConstants.CrouchingPlayerHeight : PlayerConstants.StandingPlayerHeight;
        float velocity = 0;
        float startingHeight = myCollider.size.y;
        float height = Mathf.SmoothDamp(myCollider.size.y, endHeight, ref velocity, Time.deltaTime);

        if(height > startingHeight && grounded)
        {
            Vector3 newPosition = transform.position;
            newPosition.y += height - startingHeight;
            transform.position = newPosition;
        }
        myCollider.size = new Vector3(myCollider.size.x, height, myCollider.size.z);
    }

    private void DampenCamera()
    {
        if (grounded)
        {
            Vector3 endOffset = crouching ? PlayerConstants.CrouchingCameraOffset : PlayerConstants.StandingCameraOffset;
            Vector3 currentOffset = cameraMove.playerCamera.transform.localPosition;
            float v = 0;
            float yOffset = Mathf.SmoothDamp(currentOffset.y, endOffset.y, ref v, Time.deltaTime);
            Vector3 newOffset = new Vector3(0, yOffset, 0);
            cameraMove.playerCamera.transform.localPosition = newOffset;
        }
    }

    private void ApplyGravity()
    {
        if (!grounded && newVelocity.y > -PlayerConstants.MaxFallSpeed)
        {
            newVelocity.y -= currentLevel.gravityMultiplier * PlayerConstants.Gravity * Time.fixedDeltaTime;
        }

        CheckGravitySound();
    }

    private void CheckGravitySound()
    {
        if (newVelocity.y <= -PlayerConstants.MaxFallSpeed)
        {
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Falling);
        }
    }

    private bool CanUncrouch()
    {
        float crouchingHeightChange = (PlayerConstants.StandingPlayerHeight - PlayerConstants.CrouchingPlayerHeight) / 2;
        float uncrouchCastDistance = crouchingHeightChange + float.Epsilon;

        RaycastHit[] hits = Physics.BoxCastAll(
            center: myCollider.bounds.center,
            halfExtents: myCollider.bounds.extents,
            direction: Vector3.up,
            orientation: Quaternion.identity,
            maxDistance: uncrouchCastDistance,
            layerMask: layersToIgnore,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore);

        List<RaycastHit> validHits = hits
            .ToList()
            .Where(hit => !Physics.GetIgnoreCollision(hit.collider, myCollider))
            .Where(hit => hit.point != Vector3.zero)
            .ToList();

        return validHits.Count == 0;
    }

    private void CheckCeiling()
    {
        if(newVelocity.y <= 0)
        {
            return;
        }

        float castDistance = Mathf.Abs(newVelocity.y * Time.fixedDeltaTime);
        RaycastHit[] hits = Physics.BoxCastAll(
            center: myCollider.bounds.center,
            halfExtents: myCollider.bounds.extents,
            direction: Vector3.up,
            orientation: Quaternion.identity,
            maxDistance: castDistance,
            layerMask: layersToIgnore,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore);

        List<RaycastHit> validHits = hits
            .ToList()
            .Where(hit => !Physics.GetIgnoreCollision(hit.collider, myCollider))
            .Where(hit => hit.point != Vector3.zero)
            .ToList();

        if(validHits.Count > 0)
        {
            newVelocity.y = 0;
        }
    }

    private void CheckGrounded()
    {
        if (newVelocity.y > 5)
        {
            grounded = false;
            wasGrounded = false;
            return;
        }

        bool willBeGrounded = false;

        // We need to use Mathf.Min because on the first frame of landing, we will zero out our velocity
        // So, the next frame, the cast distance will be zero, so every other frame we would be NOT grounded
        float castDistance = Mathf.Max(Mathf.Abs(newVelocity.y * Time.fixedDeltaTime), PlayerConstants.MinimumGroundCastDistance);

        RaycastHit[] hits = Physics.BoxCastAll(
            center: myCollider.bounds.center,
            halfExtents: myCollider.bounds.extents,
            direction: Vector3.down,
            orientation: Quaternion.identity,
            maxDistance: castDistance,
            layerMask: layersToIgnore,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore);

        List<RaycastHit> validHits = hits
            .ToList()
            .OrderBy(hit => hit.distance)
            .Where(hit => !Physics.GetIgnoreCollision(hit.collider, myCollider))
            .Where(hit => hit.point != Vector3.zero)
            .Where(hit => hit.normal.y > 0.7f)
            .ToList();

        if(validHits.Count > 0)
        {
            willBeGrounded = true;
        }

        grounded = willBeGrounded;

        CheckLandingSound();

        if (grounded && newVelocity.y < 0)
        {
            newVelocity.y = 0;
        }

        wasGrounded = grounded;
    }

    private void CheckLandingSound()
    {
        if (!wasGrounded && grounded)
        {
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Land);
        }
    }

    private void CheckJump()
    {
        if (grounded && InputManager.GetKey(PlayerConstants.Jump))
        {
            newVelocity.y = 0;
            newVelocity.y += crouching ? PlayerConstants.CrouchingJumpPower : PlayerConstants.JumpPower;
            grounded = false;
        }
    }

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
            horizontalSpeed = -moveSpeed;
        }

        if (InputManager.GetKey(PlayerConstants.Right))
        {
            horizontalSpeed = moveSpeed;
        }

        if (InputManager.GetKey(PlayerConstants.Back))
        {
            verticalSpeed = -moveSpeed;
        }

        if (InputManager.GetKey(PlayerConstants.Forward))
        {
            verticalSpeed = moveSpeed;
        }

        return new Vector3(horizontalSpeed, 0, verticalSpeed);
    }

    private bool IsPlayerWalkingBackwards()
    {
        Vector3 inputDirection = GetInputVelocity(PlayerConstants.MoveSpeed);

        return inputDirection.z < 0;
    }

    //wishDir: the direction the player wishes to go in the newest frame
    //wishSpeed: the speed the player wishes to go this frame
    private void ApplyGroundAcceleration(Vector3 wishDir, float wishSpeed, float surfaceFriction)
    {
        var currentSpeed = Vector3.Dot(newVelocity, wishDir); //Vector projection of the current velocity onto the new direction
        var speedToAdd = wishSpeed - currentSpeed;

        var acceleration = PlayerConstants.GroundAcceleration * Time.fixedDeltaTime; //acceleration to apply in the newest direction

        if (speedToAdd <= 0)
        {
            return;
        }

        var accelspeed = Mathf.Min(acceleration * wishSpeed * surfaceFriction, speedToAdd);
        newVelocity += accelspeed * wishDir; //add acceleration in the new direction
    }

    //wishDir: the direction the player  wishes to goin the newest frame
    //wishSpeed: the speed the player wishes to go this frame
    private void ApplyAirAcceleration(Vector3 wishDir, float wishSpeed)
    {
        var wishSpd = Mathf.Min(wishSpeed, PlayerConstants.AirAccelerationCap);
        Vector3 xzVelocity = newVelocity;
        xzVelocity.y = 0;
        var currentSpeed = Vector3.Dot(xzVelocity, wishDir);
        var speedToAdd = wishSpd - currentSpeed;

        if (speedToAdd <= 0)
        {
            return;
        }

        var accelspeed = Mathf.Min(speedToAdd, PlayerConstants.AirAcceleration * wishSpeed * Time.fixedDeltaTime);
        var velocityTransformation = accelspeed * wishDir;

        newVelocity += velocityTransformation;
    }

    private void ApplyFriction()
    {
        var speed = newVelocity.magnitude;

        // Don't apply friction if the player isn't moving
        // Clear speed if it's too low to prevent accidental movement
        // Also makes the player's friction feel more snappy
        if (speed < PlayerConstants.MinimumSpeedCutoff)
        {
            newVelocity = Vector3.zero;
            return;
        }

        // Bleed off some speed, but if we have less than the bleed
        // threshold, bleed the threshold amount.
        var control = (speed < PlayerConstants.StopSpeed) ? PlayerConstants.StopSpeed : speed;

        // Add the amount to the loss amount.
        var lossInSpeed = control * PlayerConstants.Friction * Time.fixedDeltaTime;
        var newSpeed = Mathf.Max(speed - lossInSpeed, 0);

        if (newSpeed != speed)
        {
            newVelocity *= newSpeed / speed; //Scale velocity based on friction
        }
    }

    // This function keeps the player from exceeding a maximum velocity
    private void ClampVelocity(float range)
    {
        newVelocity = Vector3.ClampMagnitude(newVelocity, PlayerConstants.MaxVelocity);
    }

    // This function is what keeps the player from walking through walls
    // We calculate how far we are inside of an object from moving this frame
    // and move the player just barely outside of the colliding object

    private void ExecuteGroundMovement()
    {
        // - boxcast forwards based on speed, find the point in time where i hit it, and stop me there
        Vector3 horizontalVelocity = newVelocity;
        horizontalVelocity.y = 0;

        float castDistance = horizontalVelocity.magnitude * Time.fixedDeltaTime;

        RaycastHit[] hits = Physics.BoxCastAll(
            center:                     myCollider.bounds.center,
            halfExtents:                myCollider.bounds.extents,
            direction:                  horizontalVelocity.normalized,
            orientation:                Quaternion.identity,
            maxDistance:                castDistance,
            layerMask:                  layersToIgnore,
            queryTriggerInteraction:    QueryTriggerInteraction.Ignore);

        List<RaycastHit> validHits = hits
            .ToList()
            .OrderBy(hit => hit.distance)
            .Where(hit => !Physics.GetIgnoreCollision(hit.collider, myCollider))
            .Where(hit => hit.point != Vector3.zero)
            .Where(hit => hit.normal.y < 1) // TO DO: check this
            .ToList();

        // If we are going to hit a wall, set ourselves just outside of the wall and translate momentum along the wall
        if (validHits.Count() > 0) //&& newVelocity.magnitude > 10)
        {
            float fractionOfDistanceTraveled = validHits.First().distance / newVelocity.magnitude;
            // slide along the wall and prevent a complete loss of momentum
            ClipVelocity(validHits.First().normal);
            // set our position to just outside of the wall
            transform.position += newVelocity * fractionOfDistanceTraveled;
            StepMove(1 - fractionOfDistanceTraveled);
        }
        else
        {
            transform.position += newVelocity * Time.fixedDeltaTime;
        }

        StayOnGround();
    }

    private void ExecuteAirMovement()
    {
        float castDistance = newVelocity.magnitude * Time.fixedDeltaTime;

        RaycastHit[] hits = Physics.BoxCastAll(
            center: myCollider.bounds.center,
            halfExtents: myCollider.bounds.extents,
            direction: newVelocity.normalized,
            orientation: Quaternion.identity,
            maxDistance: castDistance,
            layerMask: layersToIgnore,
            queryTriggerInteraction: QueryTriggerInteraction.Ignore);

        List<RaycastHit> validHits = hits
            .ToList()
            .OrderBy(hit => hit.distance)
            .Where(hit => !Physics.GetIgnoreCollision(hit.collider, myCollider))
            .Where(hit => hit.point != Vector3.zero)
            .ToList();

        // If we are going to hit a wall, set ourselves just outside of the wall and translate momentum along the wall
        if (validHits.Count() > 0)
        {
            float fractionOfDistanceTraveled = validHits.First().distance / newVelocity.magnitude;
            // slide along the wall and prevent a complete loss of momentum
            ClipAirVelocity(validHits.First().normal);
            // set our position to just outside of the wall
            transform.position += newVelocity * fractionOfDistanceTraveled;
        }
        else
        {
            transform.position += newVelocity * Time.fixedDeltaTime;
        }
    }

    private void StepMove(float fractionLeftToMove)
    {
        Vector3 positionStepUp = transform.position + new Vector3(0, PlayerConstants.StepOffset, 0);
        Trace upTrace = RayCastUtils.TraceBBoxFrom(myCollider, transform.position, positionStepUp, layersToIgnore);

        if(upTrace.fraction > 0)
        {

        }

        Vector3 endPosition = transform.position;
        Vector3 position = transform.position;
        Vector3 velocity = newVelocity;
        Trace currentTrace;

        //TryPlayerMove();

        Vector3 downPosition = transform.position;
        Vector3 downVelocity = newVelocity;

        // Reset original values
        transform.position = position;
        newVelocity = velocity;

        // Move up a stair height
        endPosition = transform.position;
        endPosition.y += PlayerConstants.StepOffset + float.Epsilon;

        //currentTrace = RayCastUtils.StayOnGroundTrace(myCollider, endPosition, layersToIgnore);
        //if (!currentTrace.allSolid)
        //{
        //    transform.position = currentTrace.hitPoint;
        //}

        // Slide move up
        //TryPlayerMove();

        // Attempt to move down a stair
        endPosition = transform.position;
        endPosition.y -= PlayerConstants.StepOffset + float.Epsilon;

        currentTrace = RayCastUtils.TracePlayerBBox(myCollider, endPosition, layersToIgnore);

        // If we are not on the ground any more then use the original movement attempt.
        if (currentTrace.hit.normal.y < 0.7f) {
            transform.position = downPosition;
            newVelocity = downVelocity;
        }

        // If the trace ended up in empty space, move to the origin.
        //if (!currentTrace.allSolid)
        //{
        //    transform.position = currentTrace.hitPoint;
        //}

        Vector3 upPosition = transform.position;

        // decide which one went farther
        float downDistance = (downPosition.x - position.x) * (downPosition.x - position.x) + (downPosition.z - position.z) * (downPosition.z - position.z);
        float upDistance = (upPosition.x - position.x) * (upPosition.x - position.x) + (upPosition.z - position.z) * (upPosition.z - position.z);

        if(downDistance > upDistance)
        {
            transform.position = downPosition;
            newVelocity = downVelocity;
        }
        else
        {
            // copy y value from slide move
            newVelocity.y = downVelocity.y;
        }
    }

    private void StayOnGround()
    {
        Vector3 positionSlightlyAbove = transform.position;
        positionSlightlyAbove.y += 0.05f;

        Vector3 destinationPosition = transform.position;
        destinationPosition.y -= PlayerConstants.StepOffset;

        // Test upwards to make sure we can start from a safe location
        Trace traceUp = RayCastUtils.TracePlayerBBox(myCollider, positionSlightlyAbove, layersToIgnore);
        positionSlightlyAbove = traceUp.hitPoint;

        // Now trace down from a known safe position
        Trace traceDown = RayCastUtils.StayOnGroundTrace(myCollider, positionSlightlyAbove, destinationPosition, layersToIgnore);
        if(traceDown.fraction > 0                    // must go somewhere
            && traceDown.fraction < 1                // must hit something
            && traceDown.hit.normal.y >= 0.7f)       // can't hit a steep slope that we can't stand on anyway
        {
            transform.position = traceDown.hitPoint + new Vector3(0, 0.01f, 0);
        }
    }

    //Slide off of the impacting surface
    private void ClipVelocity(Vector3 normal)
    {
        // Determine how far along plane to slide based on incoming direction.
        float backoff = Vector3.Dot(newVelocity, normal);

        Vector3 change = normal * backoff;
        change.y = 0; // only affect horizontal velocity
        newVelocity -= change;
    }

    private void ClipAirVelocitya(Vector3 normal)
    {
        float backoff = Vector3.Dot(newVelocity, normal);
        Vector3 change = normal * backoff;
        newVelocity -= change;
    }

    //Slide off of the impacting surface
    private void ClipAirVelocity(Vector3 normal)
    {
        Vector3 toReturn = newVelocity;

        // Determine how far along plane to slide based on incoming direction.
        float backoff = Vector3.Dot(newVelocity, normal);

        var change = normal * backoff;
        toReturn -= change;

        // iterate once to make sure we aren't still moving through the plane
        float adjust = Vector3.Dot(toReturn, normal);
        if (adjust < 0)
        {
            toReturn -= (normal * adjust);
        }

        newVelocity = toReturn;
    }

    private void ResolveCollisions()
    {
        var overlaps = Physics.OverlapBox(myCollider.bounds.center, myCollider.bounds.extents, Quaternion.identity);

        foreach (var other in overlaps)
        {
            // If the collider is my own, check the next one
            if (other == myCollider || other.isTrigger)
            {
                continue;
            }

            if (Physics.ComputePenetration(myCollider, transform.position, transform.rotation,
                other, other.transform.position, other.transform.rotation,
                out Vector3 dir, out float dist))
            {
                if (Vector3.Dot(dir, newVelocity.normalized) > 0 ||
                    Physics.GetIgnoreCollision(myCollider, other))
                {
                    continue;
                }

                Vector3 depenetrationVector = dir * dist; // The vector needed to get outside of the collision

                if (showDebugGizmos)
                {
                    Debug.Log($"Object Depenetration Vector: {depenetrationVector.ToString("F8")} \n Collided with: {other.gameObject.name}");
                }

                transform.position += depenetrationVector;
            }
        }
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