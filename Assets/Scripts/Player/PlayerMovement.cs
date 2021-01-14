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
        FixCeiling();

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

            if (newVelocity.magnitude > 0)
            {
                PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Footstep);
            }
        }
        else
        {
            ApplyAirAcceleration(wishDir, wishSpeed);
        }

        CheckGrounded();
        ClampVelocity(PlayerConstants.MaxVelocity);

        if (grounded)
        {
            newVelocity.y = 0;
        }
        //ContinuousCollisionDetection();
    }

    private void CheckCrouch()
    {
        if (InputManager.GetKey(PlayerConstants.Crouch))
        {
            crouching = true;
        }
        else
        {
            if (crouching)
            {
                crouching = CheckAbove(0.8f);
            }
            else
            {
                crouching = false;
            }
        }

        // Update player collider
        float endHeight = crouching ? PlayerConstants.CrouchingPlayerHeight : PlayerConstants.StandingPlayerHeight;
        float velocity = 0;
        float height = Mathf.SmoothDamp(myCollider.size.y, endHeight, ref velocity, Time.deltaTime);

        myCollider.size = new Vector3(myCollider.size.x, height, myCollider.size.z);

        DampenCamera();
        
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

    private void ApplyGravity()
    {
        if (!grounded && newVelocity.y > -PlayerConstants.MaxFallSpeed)
        {
            float gravityScale = currentLevel.gravityMultiplier;
            newVelocity.y -= gravityScale * PlayerConstants.Gravity * Time.fixedDeltaTime;
        }

        if(newVelocity.y <= -PlayerConstants.MaxFallSpeed)
        {
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Falling);
        }
    }

    private void FixCeiling()
    {
        if (CheckAbove() && newVelocity.y > 0)
        {
            newVelocity.y = 0;
        }
    }

    private bool CheckAbove(float distanceToCheck = 0.1f)
    {
        Ray[] boxTests = GetRays(Vector3.up);


        foreach (Ray ray in boxTests)
        {
            if (showDebugGizmos)
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.yellow, 3);
            }
            if (Physics.Raycast(
                ray: ray,
                hitInfo: out RaycastHit hit,
                maxDistance: myCollider.bounds.extents.y + distanceToCheck, // add a small offset to allow the player to find the ground is ResolveCollision() sets us too far away
                layerMask: layersToIgnore,
                QueryTriggerInteraction.Ignore))
            {
                if (hit.point.y > transform.position.y) 
                {
                    return true;
                }
            }
        }

        return false;

        
    }

    // Performs 5 raycasts to check if there is a spot on the BoxCollider which is below the player and sets the grounded status
    private void CheckGrounded()
    {
        Ray[] boxTests = GetRays(Vector3.down);

        bool willBeGrounded = false;

        foreach(Ray ray in boxTests)
        {
            if (showDebugGizmos)
            {
                Debug.DrawRay(ray.origin, ray.direction, Color.blue, 3);
            }
            if(Physics.Raycast(
                ray: ray,
                hitInfo: out RaycastHit hit,
                maxDistance: myCollider.bounds.extents.y + 0.2f, // add a small offset to allow the player to find the ground is ResolveCollision() sets us too far away
                layerMask: layersToIgnore,
                QueryTriggerInteraction.Ignore))
            {
                if(hit.point.y < transform.position.y && 
                    !Physics.GetIgnoreCollision(myCollider, hit.collider) && // don't check for the ground on an object that is ignored (for example a wall with a portal on it)
                    hit.normal.y > 0.7f) // don't check for the ground on a sloped surface above 45 degrees
                {
                    willBeGrounded = true;
                }
            }
        }

        if (newVelocity.y > 5)
        {
            willBeGrounded = false;
        }

        grounded = willBeGrounded;

        if (!wasGrounded && grounded)
        {
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Land);
        }

        if (grounded && newVelocity.y < 0)
        {
            newVelocity.y = 0;
        }

        wasGrounded = grounded;
    }

    private Ray[] GetRays(Vector3 direction)
    {
        Vector3 center = myCollider.bounds.center;
        Vector3 frontLeft = myCollider.bounds.center;
        frontLeft.x -= myCollider.bounds.extents.x - PlayerConstants.groundCheckOffset;
        frontLeft.z += myCollider.bounds.extents.z - PlayerConstants.groundCheckOffset;
        Vector3 backLeft = myCollider.bounds.center;
        backLeft.x -= myCollider.bounds.extents.x - PlayerConstants.groundCheckOffset;
        backLeft.z -= myCollider.bounds.extents.z - PlayerConstants.groundCheckOffset;
        Vector3 frontRight = myCollider.bounds.center;
        frontRight.x += myCollider.bounds.extents.x - PlayerConstants.groundCheckOffset;
        frontRight.z -= myCollider.bounds.extents.z - PlayerConstants.groundCheckOffset;
        Vector3 backRight = myCollider.bounds.center;
        backRight.x += myCollider.bounds.extents.x - PlayerConstants.groundCheckOffset;
        backRight.z += myCollider.bounds.extents.z - PlayerConstants.groundCheckOffset;

        Ray ray0 = new Ray(center, direction);
        Ray ray1 = new Ray(frontLeft, direction);
        Ray ray2 = new Ray(backLeft, direction);
        Ray ray3 = new Ray(frontRight, direction);
        Ray ray4 = new Ray(backRight, direction);

        return new Ray[] { ray0, ray1, ray2, ray3, ray4 };
    }

    private void CheckJump()
    {
        if (grounded && InputManager.GetKey(PlayerConstants.Jump))
        {
            newVelocity.y = 0;
            newVelocity.y += crouching ? PlayerConstants.CrouchingJumpPower : PlayerConstants.JumpPower;
            grounded = false;
            PlayerSoundEffects.PlaySoundEffect(SoundEffectType.Jump);
        }
    }

    private Vector3 GetWorldSpaceInputVector()
    {
        float moveSpeed = crouching ? PlayerConstants.CrouchingMoveSpeed : PlayerConstants.MoveSpeed;

        var inputVelocity = GetInputVelocity(moveSpeed);
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
        TryPlayerMoveGrounded();
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
        //  threshold, bleed the threshold amount.

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

    private void TryPlayerMoveGrounded()
    {
        // - boxcast forwards based on speed, find the point in time where i hit it, and stop me there
        Vector3 horizontalVelocity = newVelocity;
        horizontalVelocity.y = 0;

        float castDistance = horizontalVelocity.magnitude * Time.fixedDeltaTime;
        RaycastHit[] hits = Physics.BoxCastAll(
            center: myCollider.bounds.center,
            halfExtents: myCollider.bounds.extents,
            direction: horizontalVelocity.normalized,
            Quaternion.identity,
            maxDistance: castDistance,
            layerMask: layersToIgnore);

        List<RaycastHit> validHits = hits
            .ToList()
            .OrderBy(hit => hit.distance)
            .Where(hit => !hit.collider.isTrigger)
            .Where(hit => !Physics.GetIgnoreCollision(hit.collider, myCollider))
            .Where(hit => hit.point != Vector3.zero)
            .Where(hit => hit.normal.y < 1)
            .ToList();

        // If we are going to hit a wall, set ourselves just outside of the wall and translate momentum along the wall
        if (validHits.Count() > 0) //&& newVelocity.magnitude > 10)
        {
            /*
            // find the time at which we would have hit the wall between this and the next frame
            float timeToImpact = validHits.First().distance / newVelocity.magnitude;
            // slide along the wall and prevent a complete loss of momentum
            ClipVelocity(validHits.First().normal);
            // set our position to just outside of the wall
            transform.position += newVelocity * timeToImpact;
            */
            StepMove(horizontalVelocity * Time.fixedDeltaTime);
            StayOnGround();
        }
        else
        {
            transform.position += newVelocity * Time.fixedDeltaTime;
            StayOnGround();
        }
    }

    private void StepMove(Vector3 destination)
    {
        Vector3 endPosition = destination;
        Vector3 position = transform.position;
        Vector3 velocity = newVelocity;
        Trace currentTrace;

        TryPlayerMove();

        Vector3 downPosition = transform.position;
        Vector3 downVelocity = newVelocity;

        // Reset original values
        transform.position = position;
        newVelocity = velocity;

        // Move up a stair height
        endPosition = transform.position;
        endPosition.y += PlayerConstants.StepOffset + float.Epsilon;

        currentTrace = RayCastUtils.TracePlayerBBox(myCollider, endPosition, layersToIgnore);
        if (currentTrace.didLeaveBoundingBox)
        {
            transform.position = currentTrace.hitPoint;
        }

        // Slide move up
        TryPlayerMove();

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
        if (currentTrace.didLeaveBoundingBox)
        {
            transform.position = currentTrace.hitPoint;
        }

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

    private void TryPlayerMove()
    {
        Vector3 originalVelocity = newVelocity;
        Vector3 primalVelocity = newVelocity;
        Vector3 testVelocity = Vector3.zero;
        float allFraction = 0;
        float timeLeftInFrame = Time.fixedDeltaTime;
        Vector3 end;
        int numPlanes = 0;
        Vector3[] planes = new Vector3[PlayerConstants.MaxClippingPlanes];
        int i, j;

        for(int bumpCount = 0; bumpCount < 4; bumpCount++)
        {
            if(newVelocity.magnitude == 0)
                break;
            
            end = transform.position + (newVelocity * timeLeftInFrame);

            Trace trace = RayCastUtils.TracePlayerBBox(myCollider, end, layersToIgnore);
            
            allFraction += trace.fraction;

            //if the cast doesn't make it outside of the player's bbox (because we start from the center)
            // TODO: make this more accurate. if the ray is at an angle it could have a distance longer than extents.x, but less that extends.magnitude
            if (!trace.didLeaveBoundingBox)
            {
                newVelocity = Vector3.zero;
                return;
            }

            // actually covered some distance, set the player to the end position of the trace and zero the plane counter
            if (trace.fraction > 0)
            {
                // Source had the following addition
                // if we are overlapping something
                // set velocity to 0 and break
                transform.position += trace.hitPoint;
                newVelocity = originalVelocity;
                numPlanes = 0;
            }

            // If we covered the entire distance, we are done
            //  and can return.
            if (trace.fraction == 1)
            {
                break;
            }

            timeLeftInFrame -= timeLeftInFrame * trace.fraction;

            // Did we run out of planes to clip against?
            // This shouldn't happen. stop our movement
            if (numPlanes >= PlayerConstants.MaxClippingPlanes)
            {
                newVelocity = Vector3.zero;
                break;
            }

            // Set up next clipping plane
            planes[numPlanes] = trace.hit.normal;
            numPlanes++;

            // Only give this a try for first impact plane because you can get yourself stuck in an acute corner by jumping in place
            if (numPlanes == 1 && !grounded)
            {
                for (i = 0; i < numPlanes; i++)
                {
                    // floor or slope, less than 45 degree angle
                    if (planes[i].y > 0.7f)
                    {
                        testVelocity = ClipVelocity(originalVelocity, planes[i], testVelocity);
                        originalVelocity = testVelocity;
                    }
                    else
                    {
                        testVelocity = ClipVelocity(originalVelocity, planes[i], testVelocity);
                    }
                }

                newVelocity = testVelocity;
                originalVelocity = testVelocity;
            }
            else
            {
                for (i = 0; i < numPlanes; i++)
                {
                    newVelocity = ClipVelocity(originalVelocity, planes[i], newVelocity);

                    for (j = 0; j < numPlanes; j++)
                    {
                        if(j != i)
                        {
                            // Don't continue if we're moving against this plane
                            if (Vector3.Dot(newVelocity, planes[j]) < 0)
                            {
                                break;
                            }
                        }

                        if(j == numPlanes)
                        {
                            break;
                        }
                    }
                }

                if(i == numPlanes)
                {
                    if(numPlanes != 2)
                    {
                        newVelocity = Vector3.zero;
                        break;
                    }

                    Vector3 direction = Vector3.Cross(planes[0], planes[1]);
                    direction.Normalize();
                    float dot = Vector3.Dot(direction, newVelocity);
                    newVelocity = direction * dot;
                }

                // if original velocity is against the original velocity, stop dead
                // to avoid tiny occilations in sloping corners
                float dot2 = Vector3.Dot(newVelocity, primalVelocity);
                if(dot2 <= 0)
                {
                    newVelocity = Vector3.zero;
                    break;
                }
            }
        }

        if (allFraction == 0)
        {
            newVelocity = Vector3.zero;
        }
    }

    private void StayOnGround()
    {

    }

    //Slide off of the impacting surface
    //private void ClipVelocity(Vector3 normal)
    //{
    //    // Determine how far along plane to slide based on incoming direction.
    //    var backoff = Vector3.Dot(newVelocity, normal);

    //    var change = normal * backoff;
    //    change.y = 0; // only affect horizontal velocity
    //    newVelocity -= change;
    //}

    //Slide off of the impacting surface
    private Vector3 ClipVelocity(Vector3 inVec, Vector3 normal, Vector3 outVec)
    {
        Vector3 toReturn = outVec;

        // Determine how far along plane to slide based on incoming direction.
        float backoff = Vector3.Dot(inVec, normal);

        var change = normal * backoff;
        //change.y = 0; // only affect horizontal velocity
        toReturn -= change;

        // iterate once to make sure we aren't still moving through the plane
        float adjust = Vector3.Dot(toReturn, normal);
        if(adjust < 0)
        {
            toReturn -= (normal * adjust);
        }

        return toReturn;
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