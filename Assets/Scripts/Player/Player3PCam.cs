using System.Collections;
using System.Linq;
using Cinemachine;

using UnityEngine;
using UnityEngine.InputSystem;

//Contains edited code adapted from https://www.youtube.com/watch?v=f473C43s8nE

//Notes for new input system:
/**
Basic udlr movement updated.
sprint updated
basic look updated
dash updated
jump updated
**/

public class Player3PCam : MonoBehaviour
{
    [Header("ThirdPersonCameraVariables")]
    public Transform orientation;
    public Transform orientationFlat;
    public Transform player;
    public Transform playerObj;
    public float rotationSpeed;

    public enum CameraMode
    {
        Free,
        Locked
    }
    public CameraMode currCamMode;
    public Transform currentTargetLock;
    public Transform actualLookPosition;
    public bool autoFindNewTarget = true;
    public Camera actualCamera;
    public CinemachineFreeLook unlockLookCamera;
    public CinemachineFreeLook combatLockCamera;
    public CinemachineFreeLook aerialCombatCamera;
    public CinemachineFreeLook aerialCloseCamera;
    public CinemachineBrain cinemachineBrain;
    public float minimumRadius;
    public float maximumRadius;
    public float minSlopeValue;
    public float maxSlopeValue;
    public float myMaxLockonRange;
    public float bumpDuration;
    public float sprintDuration;
    private float timeInLockedCam;
    public bool invertX;
    public bool invertY;


    [Header("PlayerMovementVariables")]
    public Rigidbody rb;
    public float moveSpeed;
    public float sprintSpeed;
    public float playerHeight;
    public LayerMask terrainMask;
    public float groundDrag;
    public float airDrag;
    public float jumpForce;
    public float airJumpForce;
    public float airMultiplier;
    public float hoverMultiplier;
    public float jumpCooldown;
    public float doubleTapDelay;
    public float dashForce;
    public int maxMidairBoosts;
    private int currMidairBoosts;
    private bool hovering;
    private bool isGrounded;
    private bool canJump = true;
    public bool inputLocked;
    private bool dashing = false;
    private bool sprinting = false;
    private bool allowedToDash = true;




    // new stuff for new input system

    [Header("Control Variables")]
    private PlayerInput playerInput;
    private Vector2 rawMoveInput;
    private Coroutine lastBoostCo;


    //Unity Functions
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = player.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        ActiveCameraMode = CameraMode.Free;
        playerInput = GetComponent<PlayerInput>();

    }
    private void Update()
    {
        CheckTargetLoSRange();
        DetermineActiveCamera();
        AdjustActualLookPos();

        Do3PCameraMovement();


        DetectTargetBumps();
        AdjustDamping();

    }
    private void FixedUpdate()
    {
        GroundedCheckAndDrag();
        MovePlayer();
        BumpPlayerAwayFromTarget();
        DoSpeedControl();
    }

    private void BumpPlayerAwayFromTarget()
    {
        if (currentTargetLock != null && flatDist < 0.3f)
        {
            rb.AddForce(orientationFlat.forward * -2, ForceMode.VelocityChange);
        }
    }

    //Private Functions

    private void CheckTargetLoSRange()
    {
        //Need to implement LoS. Only does range at the moment.
        if (currentTargetLock && Vector3.Distance(player.position, currentTargetLock.position) > myMaxLockonRange)
        {
            ActiveCameraMode = CameraMode.Free;
        }
    }
    private void DetermineActiveCamera()
    {
        if (currCamMode == CameraMode.Free)
        {
            unlockLookCamera.Priority = 11;
            combatLockCamera.Priority = -1;
            aerialCloseCamera.Priority = -1;
            aerialCombatCamera.Priority = -1;
            timeInLockedCam = 0;
        }
        else if (currCamMode == CameraMode.Locked)
        {
            timeInLockedCam += Time.deltaTime;
            unlockLookCamera.Priority = -1;
            if (!isGrounded && flatDist < 5)
            {
                aerialCombatCamera.Priority = -1;
                aerialCloseCamera.Priority = 11;

            }
            else if (!isGrounded && flatDist >= 5)
            {
                aerialCombatCamera.Priority = 11;
                aerialCloseCamera.Priority = -1;

            }
            else if (isGrounded)
            {
                aerialCombatCamera.Priority = -1;
                aerialCloseCamera.Priority = 10;
            }
            combatLockCamera.Priority = isGrounded ? 11 : -1;
        }

    }
    private void Do3PCameraMovement()
    {
        //Code adapted from https://www.youtube.com/watch?v=UCwwn2q4Vys


        //Set viewDir based on which camera is active.
        Vector3 viewDir = new Vector3();
        if (currCamMode == CameraMode.Free)
        {
            viewDir = player.position - new Vector3(unlockLookCamera.transform.position.x, player.position.y, unlockLookCamera.transform.position.z);
        }
        else if (currCamMode == CameraMode.Locked)
        {
            viewDir = actualLookPosition.position - new Vector3(player.position.x, player.position.y, player.position.z);
        }

        //Set orientation and flat orientation.
        //If not grounded, and targeting something, and very close, lerp orientation to avoid camera jerkiness.

        orientation.forward = viewDir.normalized;
        orientationFlat.forward = new Vector3(orientation.forward.x, 0, orientation.forward.z);



        Vector3 adjustOffsetToSprint(Vector3 original)
        {
            bool a, b, c, d, e, f;

            a = rawMoveInput.y <= 0.1 && rawMoveInput.y >= -0.1;
            b = rawMoveInput.x <= 0.1 && rawMoveInput.x >= -0.1;
            c = rawMoveInput.y > 0;
            d = rawMoveInput.x > 0;
            e = rawMoveInput.y < 0;
            f = rawMoveInput.x < 0;
            float offAngle = (a && b) || (b && c) ? 0 : a && d ? 90 : a && f ? -90 : b && e ? 180 : c && d ? 45 : c && f ? -45 : e && f ? -135 : d && e ? 135 : 0;
            return Quaternion.AngleAxis(offAngle, Vector3.up) * original;
        }

        //Set camera to turn automatically if player is sprinting forwards or sideways
        //Additionally , allow player to rotate freely of camera if sprinting.
        Vector3 offset = currCamMode == CameraMode.Free || isGrounded ? orientationFlat.forward : orientation.forward;
        if (sprinting && rawMoveInput.magnitude > 0 && isGrounded)
        {
            if (rawMoveInput.y >= 0)
            {
                unlockLookCamera.m_Follow = playerObj;
                unlockLookCamera.m_RecenterToTargetHeading.m_enabled = true;
            }
            else
            {
                unlockLookCamera.m_RecenterToTargetHeading.m_enabled = false;
                unlockLookCamera.m_Follow = player;
            }
            offset = adjustOffsetToSprint(orientationFlat.forward);
        }
        else
        {
            unlockLookCamera.m_RecenterToTargetHeading.m_enabled = false;
            unlockLookCamera.m_Follow = player;
        }



        //Now adjust player forward and other housekeeping based on which cameras are active
        if (currCamMode == CameraMode.Free)
        {
            // //only adjust the player model rotation if there is a movement input or the player is off the ground.
            // if (rawMoveInput.magnitude > 0 || !isGrounded)
            // {
            //     playerObj.forward = Vector3.Lerp(playerObj.forward, offset, Time.deltaTime * rotationSpeed);
            // }
            //Updated for gun aim code: always adjust player model rotation.
            playerObj.forward = Vector3.Lerp(playerObj.forward, offset, Time.deltaTime * rotationSpeed);
        }
        else if (currCamMode == CameraMode.Locked)
        {

            combatLockCamera.m_RecenterToTargetHeading.m_enabled = true;
            combatLockCamera.m_Follow = orientationFlat;
            aerialCombatCamera.m_RecenterToTargetHeading.m_enabled = true;
            aerialCombatCamera.m_Follow = orientation;
            aerialCloseCamera.m_Follow = playerObj;
            if (!cinemachineBrain.IsBlending && timeInLockedCam >= 0.5f)
            {
                unlockLookCamera.m_XAxis.Value = orientationFlat.localEulerAngles.y;
                unlockLookCamera.m_YAxis.Value = 0.5f;
            }


            AdjustCameraOrbit();
            //Always adjust the player model rotation.
            playerObj.forward = Vector3.Slerp(playerObj.forward, offset, Time.deltaTime * rotationSpeed);

        }

        Debug.DrawRay(player.position, player.forward * 10, Color.green);
        Debug.DrawRay(orientation.position, orientation.forward * 10, Color.white);
        Debug.DrawRay(orientationFlat.position, orientationFlat.forward * 6, Color.red);
        Debug.DrawRay(playerObj.position, playerObj.forward, Color.cyan);
    }

    private void AdjustCameraOrbit()
    {

        //Adjust camera orbit if the radio between player-height-target-height-distance is high. AND the player is grounded.
        Vector3 tg = actualLookPosition.position;
        Vector3 here = player.transform.position;
        float yDist = Mathf.Abs(tg.y - here.y);
        float slope = flatDist / yDist;

        //Log function from https://math.stackexchange.com/questions/716152/graphing-given-two-points-on-a-graph-find-the-logarithmic-function-that-passes
        //Implementation assistance from Jack Green jtg002@ucsd.edu
        float a, b, c, d, h, k;
        a = minSlopeValue;
        b = minimumRadius;
        c = maxSlopeValue;
        d = maximumRadius;

        h = (b - d) / Mathf.Log10(a / c);

        float kUpper = (d * Mathf.Log10(a) - b * Mathf.Log10(c)) / (b - d);
        k = Mathf.Pow(10, kUpper);

        float idealRadius = h * Mathf.Log10(k * Mathf.Clamp(slope, minSlopeValue, maxSlopeValue));

        Mathf.Clamp(idealRadius, minimumRadius, maximumRadius);

        int i = 0;

        foreach (var orbeez in combatLockCamera.m_Orbits)
        {
            float currRad = orbeez.m_Radius;
            combatLockCamera.m_Orbits[i].m_Radius = Mathf.Lerp(currRad, idealRadius, 0.1f * Time.deltaTime);
            //And adjust close camera

            aerialCloseCamera.m_Orbits[i].m_Radius = Mathf.Lerp(aerialCloseCamera.m_Orbits[i].m_Radius, idealRadius * 2, Time.deltaTime);
            aerialCloseCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = Mathf.Lerp(aerialCloseCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y, (here.y - tg.y) / 1.5f, rotationSpeed * Time.deltaTime);
            combatLockCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = Mathf.Lerp(combatLockCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y, (here.y - tg.y) / 2, rotationSpeed * Time.deltaTime);
            aerialCombatCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = Mathf.Lerp(aerialCombatCamera.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y, (here.y - tg.y) / 4, rotationSpeed * Time.deltaTime);




            i++;
        }


    }
    private void AdjustDamping()
    {

        for (int i = 0; i < 3; i++)
        {

            //Modify horizontal damping if player is dashing & locked.

            var clc = combatLockCamera.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>();
            clc.m_XDamping = dashing ? 1 : Mathf.Lerp(clc.m_XDamping, 0, 2 * Time.deltaTime);
            clc.m_ZDamping = dashing ? 1 : Mathf.Lerp(clc.m_ZDamping, 0, 2 * Time.deltaTime);

            var ac = aerialCombatCamera.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>();
            ac.m_XDamping = dashing ? 1 : Mathf.Lerp(ac.m_XDamping, 0, 2 * Time.deltaTime);
            ac.m_ZDamping = dashing ? 1 : Mathf.Lerp(ac.m_ZDamping, 0, 2 * Time.deltaTime);
        }
    }
    private void GroundedCheckAndDrag()
    {
        //Code adapted from https://www.youtube.com/watch?v=f473C43s8nE
        isGrounded = Physics.Raycast(player.transform.position, Vector3.down, playerHeight * 0.5f + 0.05f, terrainMask);
        playerObj.GetComponentInChildren<Animator>().SetBool("Grounded", isGrounded);
        if (isGrounded) { currMidairBoosts = maxMidairBoosts; }
        if (isGrounded) { hovering = false; }
        rb.drag = dashing ? groundDrag * 1.5f : isGrounded ? groundDrag : airDrag;
        if (!isGrounded && hovering && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * hoverMultiplier, rb.velocity.z);
        }
    }
    private void DoSpeedControl()
    {
        //Code adapted from https://www.youtube.com/watch?v=f473C43s8nE
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVelocity.magnitude > moveSpeed)
        {
            float vel = dashing ? moveSpeed * 1.1f : sprinting ? sprintSpeed : moveSpeed;
            Vector3 limitedVelocity = flatVelocity.normalized * vel;
            rb.velocity = new Vector3(limitedVelocity.x, 0f, limitedVelocity.z);
        }
    }

    private bool FindLockableTarget(Transform origin, Vector3 direction, int LayerMask, float angleTolerance, bool ignoreCurrentTarget, string method = "close")
    {
        GameObject[] lockables = GameObject.FindGameObjectsWithTag("TargetPoint");
        Debug.DrawRay(origin.position, direction * 10, Color.green, 3f);
        GameObject potentialTarget = null;
        foreach (var tg in lockables)
        {
            bool inRange = Vector3.Distance(player.position, tg.transform.position) < myMaxLockonRange;
            //only lock onto enemies that are within angle tolerance degrees of the current camera orientation.
            bool ahead = Vector3.Angle(direction, (tg.transform.position - origin.position).normalized) <= angleTolerance;
            //Debug.Log(ahead + " " + Vector3.Angle(direction, (tg.transform.position - origin.position).normalized));
            Physics.Raycast(origin.position, (tg.transform.position - origin.position).normalized, out RaycastHit hit, 100, LayerMask);
            bool los = hit.collider && hit.collider.gameObject == tg;
            //finds closest available tg

            bool closest = false;
            if (method == "angle")
            {
                if (currentTargetLock)
                {
                    closest = !potentialTarget || Vector3.Angle((currentTargetLock.transform.position - origin.position).normalized, (tg.transform.position - origin.position).normalized) < Vector3.Angle((currentTargetLock.transform.position - origin.position).normalized, (potentialTarget.transform.position - origin.position).normalized);

                }
                closest = !potentialTarget || Vector3.Angle(direction, (tg.transform.position - origin.position).normalized) < Vector3.Angle(direction, (potentialTarget.transform.position - origin.position).normalized);
            }
            else
            {
                closest = !potentialTarget || Vector3.Distance(player.position, tg.transform.position) < Vector3.Distance(player.position, potentialTarget.transform.position);

            }

            if (inRange && ahead && closest && los)
            {
                if ((currentTargetLock && tg != currentTargetLock.gameObject) || !ignoreCurrentTarget)
                {
                    potentialTarget = tg;
                    Debug.DrawLine(origin.position, potentialTarget.transform.position, Color.black, 3f);
                }


            }

        }
        if (potentialTarget != null)
        {
            //Debug.Log("in find tg, found a tg. " + potentialTarget.transform.name);
            if (currentTargetLock) //place lookAt position between player and target if a previous target didn't exist
            {
                currentTargetLock = potentialTarget.transform;
            }
            else
            {
                currentTargetLock = potentialTarget.transform;
                actualLookPosition.position = Vector3.Lerp(player.position, currentTargetLock.position, 0.5f);
            }


            //combatLockCamera.m_LookAt = actualLookPosition;
            //aerialCombatCamera.m_LookAt = actualLookPosition;
            //aerialCloseCamera.m_LookAt = actualLookPosition;
            return true;
        }
        return false;
    }
    private void AdjustActualLookPos()
    {
        if (currCamMode != CameraMode.Locked || !currentTargetLock) { return; }

        if (Vector3.Distance(actualLookPosition.position, currentTargetLock.position) <= 0.1f)
        {
            actualLookPosition.position = currentTargetLock.position;
        }
        else
        {
            actualLookPosition.position = Vector3.Lerp(actualLookPosition.position, currentTargetLock.position, rotationSpeed / 2 * Time.deltaTime);
        }

    }
    private void DetectTargetBumps()
    {
        if (currCamMode != CameraMode.Locked) { return; }

        Vector2 LookDelta = playerInput.actions.FindAction("Look").ReadValue<Vector2>();
        if (invertX) { LookDelta.x *= -1; }
        if (invertY) { LookDelta.y *= -1; }
        if (Mathf.Abs(LookDelta.x) < 0.01 && Mathf.Abs(LookDelta.y) < 0.01)
        {
            bumpDuration = 0;
        }
        else
        {
            bumpDuration += Time.deltaTime;
        }

        if (bumpDuration >= doubleTapDelay / 2)
        {
            bumpDuration = -0.5f;
            Vector2 adjustedBumpDirection = (LookDelta.y * Vector2.down + Vector2.right * LookDelta.x).normalized * 30; // transfer 30 degrees then check 30 degrees
            Vector3 lookDirection = Quaternion.AngleAxis(adjustedBumpDirection.x, Vector3.up) * orientation.transform.forward;
            lookDirection = Quaternion.AngleAxis(adjustedBumpDirection.y, Vector3.right) * lookDirection;
            lookDirection.Normalize();
            Debug.DrawRay(orientation.transform.position, lookDirection * 30, Color.magenta, 3f);


            FindLockableTarget(orientation.transform, lookDirection, LayerMask.GetMask("TargetPoint", "CameraObstacle", "WalkableTerrain"), 30, true, "angle");
        }
    }
    private void MovePlayer()
    {
        if (dashing) { return; } //If player is mid-dash do not adjust movement

        Vector3 moveDirection = orientation.forward * rawMoveInput.y + orientation.right * rawMoveInput.x;
        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);
        Debug.DrawRay(playerObj.position, moveDirection.normalized, Color.yellow);
        if (isGrounded)
        {

            if (sprinting)
            {
                rb.AddForce(moveDirection.normalized * sprintSpeed, ForceMode.Force);
            }
            else
            {
                rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
            }
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }
    }
    // New input functions
    public void OnMove(InputValue v) //Does not actually move player - only updates movement vector
    {
        rawMoveInput = v.Get<Vector2>();
        if (rawMoveInput.magnitude == 0 || dashing)
        {
            sprinting = false;
            playerObj.GetComponentInChildren<Animator>().SetBool("Sprinting", false);

        }
        else if (InputSystem.actions.FindAction("Sprint").ReadValue<float>() == 1)
        {
            sprinting = true;
            playerObj.GetComponentInChildren<Animator>().SetBool("Sprinting", true);
        }
    }

    public void OnSprint(InputValue v)
    {
        if (v.Get<float>() == 1)
        {
            sprintDuration += Time.deltaTime;
        }
        else
        {
            sprintDuration = 0;
        }
        //TODO: Move animator updates to a different script
        if (v.Get<float>() == 1 && rawMoveInput.magnitude > 0 && isGrounded && !dashing && sprintDuration > 0.3f)
        {
            sprinting = true;
            playerObj.GetComponentInChildren<Animator>().SetBool("Sprinting", true);

        }
        else
        {
            playerObj.GetComponentInChildren<Animator>().SetBool("Sprinting", false);
            sprinting = false;
        }

    }

    public void OnCameraLock()
    {
        if (ActiveCameraMode == CameraMode.Free)
        {
            ActiveCameraMode = CameraMode.Locked;
        }
        else
        {
            ActiveCameraMode = CameraMode.Free;
        }
    }
    public void EnemyKilledEvent(CombatEntity c)
    {

        if (autoFindNewTarget && ActiveCameraMode == CameraMode.Locked)
        {
            TargetPoint[] tgs = c.gameObject.transform.GetComponentsInChildren<TargetPoint>();
            if (tgs.Contains(currentTargetLock.GetComponent<TargetPoint>()))
            {
                //if true, The currently locked target point is a child of the Shootable that just got killed.
                //Therefore, 
                //Debug.Log("In enemy killed event, confirmed lock, looking for new tg");
                if (!FindLockableTarget(actualCamera.transform, (actualLookPosition.position - actualCamera.transform.position).normalized, LayerMask.GetMask("TargetPoint", "CameraObstacle", "WalkableTerrain"), 30, true, "angle"))
                {
                    //Debug.Log("Did not find an enemy to lock");
                    ActiveCameraMode = CameraMode.Free;
                }
            }

        }
        else
        {
            ActiveCameraMode = CameraMode.Free;
        }

    }
    public void OnDodge()
    {
        if (!allowedToDash) { return; }
        //Tap once to dash, tap once with no directional input to backstep. Dash input ignored when button is held.'

        if (rawMoveInput.magnitude == 0)
        {
            dashing = true;
            allowedToDash = false;

            //do backstep
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            Vector3 dashDir = orientationFlat.forward * -1;
            dashDir.y = 0;
            dashDir.Normalize();
            rb.AddForce(dashDir * dashForce / 2, ForceMode.Impulse);
            StartCoroutine(DashCooldown(0.15f));
        }
        else if (isGrounded || (!isGrounded && currMidairBoosts > 0))
        {
            dashing = true;
            allowedToDash = false;
            //do full directional dash

            //Get direction of the dash based on player input
            Vector3 dashDir = orientationFlat.transform.right * rawMoveInput.x + orientationFlat.transform.forward * rawMoveInput.y;
            dashDir.Normalize();
            rb.AddForce(dashDir * dashForce, ForceMode.Impulse);
            //subtract if not grounded
            if (!isGrounded) { currMidairBoosts--; }
            StartCoroutine(DashCooldown(0.25f));
        }
    }
    private IEnumerator DashCooldown(float dashDur)
    {
        yield return new WaitForSeconds(dashDur);
        dashing = false;
        yield return new WaitForSeconds(0.1f);
        allowedToDash = true;
    }
    public void OnJump()
    {
        if (!canJump) { return; }
        if (isGrounded)
        {
            playerObj.GetComponentInChildren<Animator>().SetTrigger("Jump");
            StartCoroutine(DoActualJump());
        }
        else if (!isGrounded && currMidairBoosts > 0)
        {
            //Particle trigger here? Or perhaps separate script.
            //Debug.Log("Starting Boost");
            lastBoostCo = StartCoroutine(Boost());
        }
        canJump = false;
        StartCoroutine(JumpCooldown(0.2f));
    }
    private IEnumerator JumpCooldown(float dur)
    {
        yield return new WaitForSeconds(dur);
        canJump = true;
    }
    private IEnumerator DoActualJump()
    {
        yield return new WaitForSeconds(0.2f);
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(player.transform.up * jumpForce, ForceMode.Impulse);
    }
    private IEnumerator Boost()
    {
        yield return new WaitForSeconds(0.3f);
        hovering = false;
        currMidairBoosts--;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(1.5f * airJumpForce * player.transform.up, ForceMode.Impulse);
    }
    public void OnHover()
    {
        if (!isGrounded)
        {
            //Debug.Log("got a hover input");
            StopCoroutine(lastBoostCo);
            hovering = !hovering;
        }
    }
    public void OnDebugReset()
    {
        rb.position = GameObject.FindWithTag("SpawnPoint").transform.position;
        rb.velocity = Vector3.zero;
    }

    //  Public Getters & Setters
    public int BoostsRemaining
    {
        get
        {
            return currMidairBoosts;
        }
    }
    public bool IsDashing
    {
        get
        {
            return dashing;
        }
    }
    public bool CanDash
    {
        get
        {
            return allowedToDash;
        }
    }
    public CameraMode ActiveCameraMode
    {
        get
        {
            return currCamMode;
        }
        set
        {

            if (value == CameraMode.Free)
            {
                currentTargetLock = null;

                currCamMode = value;
            }
            else
            {
                if (FindLockableTarget(actualCamera.transform, actualCamera.transform.forward, LayerMask.GetMask("TargetPoint", "CameraObstacle"), 30, false))
                {
                    currCamMode = value;
                }
            }
        }
    }
    public Vector3 DistanceFromCurrentTarget
    {
        get
        {
            if (currentTargetLock != null)
            {
                return currentTargetLock.position - player.transform.position;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
    private float flatDist
    {
        get
        {
            if (!currentTargetLock) { return -1; }
            Vector3 tg = currentTargetLock.position;
            Vector3 here = player.transform.position;
            return Vector2.Distance(new(tg.x, tg.z), new(here.x, here.z));
        }
    }

    public void SetInvertBumps(bool[] inverts)
    {
        //Debug.Log(inverts[0]);
        //Debug.Log(!inverts[1]);

        invertX = inverts[0];
        invertY = !inverts[1];
    }

    public bool IsSprinting
    {
        get
        {
            return sprinting;
        }
    }
}


