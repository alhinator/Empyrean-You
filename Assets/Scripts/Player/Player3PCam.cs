using System;
using System.Collections;
using System.Diagnostics.SymbolStore;
using System.Net.WebSockets;
using Cinemachine;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.Video;

public class Player3PCam : MonoBehaviour
{
    [Header("ThirdPersonCameraVariables")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public float rotationSpeed;
    public bool xLookInvert = false;
    public bool yLookInvert = true;
    public enum CameraMode
    {
        Free,
        Locked
    }
    public CameraMode currCamMode;
    public Transform currentTargetLock;
    public bool autoFindNewTarget = true;
    private CinemachineFreeLook myCamera;

    public CinemachineFreeLook combatLockCamera;


    [Header("PlayerMovementVariables")]
    public Rigidbody rb;
    public float moveSpeed;
    public float sprintSpeed;
    public float playerHeight;
    public LayerMask terrainMask;
    public float groundDrag;
    public float jumpForce;
    public float airMultiplier;
    public float hoverMultiplier;
    public float jumpCooldown;
    public float doubleTapDelay;
    public float dashForce;
    public int maxMidairBoosts;

    private int currMidairBoosts;
    private bool hovering;
    private bool isGrounded;
    private float canJump;
    private float dashTapCount;
    private float boostTapCount;
    private float sprintDuration;

    public bool inputLocked;
    private bool dashing = false;
    private bool allowedToDash = true;


    //Unity Functions
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = player.GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        myCamera = GetComponent<CinemachineFreeLook>();
        ActiveCameraMode = CameraMode.Free;
    }
    private void Update()
    {
        Do3PCameraMovement();
        GroundedCheckAndDrag();
        DoSpeedControl();
        DoJumpCheck();
        DoDashCheck();

        if (Input.GetButtonDown("CAMLOCK"))
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

        //DEBUG CODE ONLY
        if (Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            rb.position = new Vector3(0, 2f, 0);
            rb.velocity = Vector3.zero;

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            xLookInvert = !xLookInvert;
            myCamera.m_XAxis.m_InvertInput = xLookInvert;
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            yLookInvert = !yLookInvert;
            myCamera.m_YAxis.m_InvertInput = yLookInvert;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    //Private Functions
    private void Do3PCameraMovement()
    {
        //Code adapted from https://www.youtube.com/watch?v=UCwwn2q4Vys

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");


        if (currCamMode == CameraMode.Free)
        {
            Vector3 viewDir = player.position - new Vector3(myCamera.transform.position.x, player.position.y, myCamera.transform.position.z);
            orientation.forward = viewDir.normalized;
            if (horizontalInput != 0 || verticalInput != 0 || !isGrounded)
            {
                Vector3 offset = new Vector3(orientation.forward.x, orientation.forward.y, orientation.forward.z);
                if (Input.GetAxis("SPRINT") > 0 && sprintDuration > doubleTapDelay / 2 && isGrounded)
                {
                    if (verticalInput >= 0)
                    {
                        myCamera.m_Follow = playerObj;
                        myCamera.m_RecenterToTargetHeading.m_enabled = true;
                    }
                    else
                    {
                        myCamera.m_RecenterToTargetHeading.m_enabled = false;
                        myCamera.m_Follow = player;
                    }


                    bool a, b, c, d, e, f;
                    a = verticalInput == 0;
                    b = horizontalInput == 0;
                    c = verticalInput == 1;
                    d = horizontalInput == 1;
                    e = verticalInput == -1;
                    f = horizontalInput == -1;
                    float offAngle = (a && b) || (b && c) ? 0 : a && d ? 90 : a && f ? -90 : b && e ? 180 : c && d ? 45 : c && f ? -45 : e && f ? -135 : d && e ? 135 : 0;
                    offset = Quaternion.AngleAxis(offAngle, Vector3.up) * offset;
                }
                else
                {
                    myCamera.m_RecenterToTargetHeading.m_enabled = false;
                    myCamera.m_Follow = player;
                }


                playerObj.forward = Vector3.Slerp(playerObj.forward, offset, Time.deltaTime * rotationSpeed);
            }
        }
        else if (currCamMode == CameraMode.Locked)
        {
            combatLockCamera.m_RecenterToTargetHeading.m_enabled = true;
            combatLockCamera.m_Follow = playerObj;
            Vector3 viewDir = currentTargetLock.position - new Vector3(player.position.x, player.position.y, player.position.z);
            if (isGrounded)
            {
                viewDir.y = 0;
            }
            orientation.forward = viewDir.normalized;

            playerObj.forward = Vector3.Slerp(playerObj.forward, orientation.forward, Time.deltaTime * rotationSpeed);
        }







        Debug.DrawRay(player.position, player.forward * 10, Color.green);
        Debug.DrawRay(orientation.position, orientation.forward * 10, Color.red);
        Debug.DrawRay(playerObj.position, playerObj.forward, Color.cyan);



    }
    private void MovePlayer()
    {
        if (dashing) { return; }
        //Code adapted from https://www.youtube.com/watch?v=f473C43s8nE
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        float sprinting = Input.GetAxis("SPRINT");


        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);

        Debug.DrawRay(playerObj.position, moveDirection.normalized, Color.yellow);

        if (isGrounded)
        {
            if (sprinting > 0 && (horizontalInput != 0 || verticalInput != 0))
            {
                sprintDuration += Time.deltaTime;
            }
            else if (sprinting == 0 || (horizontalInput == 0 && verticalInput == 0))
            {
                sprintDuration = 0;
                playerObj.GetComponentInChildren<Animator>().SetBool("Sprinting", false);
            }
            if (sprinting > 0 && sprintDuration > doubleTapDelay)
            {
                playerObj.GetComponentInChildren<Animator>().SetBool("Sprinting", true);
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
            playerObj.GetComponentInChildren<Animator>().SetBool("Sprinting", false);
            sprintDuration = 0;

        }
    }
    private void GroundedCheckAndDrag()
    {
        //Code adapted from https://www.youtube.com/watch?v=f473C43s8nE
        isGrounded = Physics.Raycast(player.transform.position, Vector3.down, playerHeight * 0.5f + 0.05f, terrainMask);
        playerObj.GetComponentInChildren<Animator>().SetBool("Grounded", isGrounded);
        if (isGrounded) { currMidairBoosts = maxMidairBoosts; }
        if (isGrounded) { hovering = false; }
        rb.drag = isGrounded ? groundDrag : groundDrag / 3;
        if (!isGrounded && hovering)
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
            float forwardsBoost = Input.GetAxis("Vertical") == 1 && isGrounded ? moveSpeed * 2 : 0;
            Vector3 limitedVelocity = flatVelocity.normalized * (moveSpeed + forwardsBoost);
            rb.velocity = new Vector3(limitedVelocity.x, 0f, limitedVelocity.z);
        }
    }

    private void DoJumpCheck()
    {
        canJump = Mathf.Clamp(canJump - Time.deltaTime, 0, jumpCooldown);
        if (Input.GetAxis("SPACE") == 1 && isGrounded && canJump <= 0)
        {
            canJump = jumpCooldown;
            playerObj.GetComponentInChildren<Animator>().SetTrigger("Jump");
            StartCoroutine(DoActualJump());
        }
        else if (Input.GetButtonDown("SPACE") && !isGrounded)
        {
            DoMidairBoostCheck();
        }

    }
    private void DoMidairBoostCheck()
    {
        if (boostTapCount == 0)
        {
            Debug.Log("boost Taps = 1");
            boostTapCount = 1;
            StartCoroutine(BoostOrHover());
        }
        else if (boostTapCount == 1)
        {
            Debug.Log("Taps = 2");
            boostTapCount = 2;
            StartCoroutine(BoostOrHover());
        }
    }
    private IEnumerator BoostOrHover()
    {
        yield return new WaitForSeconds(doubleTapDelay);
        if (boostTapCount == 1 && currMidairBoosts > 0)
        { //means they want to boost upwards
            hovering = false;
            currMidairBoosts--;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(1.5f * jumpForce * player.transform.up, ForceMode.Impulse);
        }
        else if (boostTapCount == 2 || (boostTapCount == 1 && currMidairBoosts <= 0))
        {
            StopCoroutine(BoostOrHover());
            hovering = !hovering;
        }
        boostTapCount = 0;

    }
    private IEnumerator DoActualJump()
    {
        yield return new WaitForSeconds(0.2f);
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(player.transform.up * jumpForce, ForceMode.Impulse);
    }
    private void DoDashCheck()
    {
        //Code modified from https://discussions.unity.com/t/single-tap-double-tap-script/440934/5
        if (Input.GetButtonDown("DODGE") && !dashing && allowedToDash)
        {
            // if (dashTapCount == 0)
            // {
            //     Debug.Log("Taps = 1");
            //     dashTapCount = 1;
            //     StartCoroutine(DashTap());
            // }
            // else if (dashTapCount == 1)
            // {
            //     Debug.Log("Taps = 2");
            //     dashTapCount = 2;
            //     StartCoroutine(DashTap());
            // }

            //Active code version: Tap once to dash, and tap with no directional inputs to backstep.
            StartCoroutine(DashTap());
        }
    }
    private IEnumerator DashTap()
    {

        // if (dashTapCount == 1 || (!isGrounded && dashTapCount == 2 && currMidairBoosts <= 0))
        // {
        //     if (dashTapCount == 2)
        //     {
        //         StopCoroutine(DashTap()); //Stop the other occurence of this coroutine.
        //     }
        //     if (Input.GetButton("SPRINT"))
        //     { //Means they held the button down
        //     }
        //     else
        //     {
        //         //do dash stuff here.
        //         rb.velocity = new Vector3(0, rb.velocity.y, 0);
        //         rb.AddForce(playerObj.transform.forward * -1 * dashForce / 2, ForceMode.Impulse);
        //         dashing = true;
        //     }
        // }
        // else if (dashTapCount == 2)
        // {
        //     StopCoroutine(DashTap()); //Stop the other occurence of this coroutine.


        //     Vector3 dashDir = (playerObj.transform.right * Input.GetAxis("Horizontal") + playerObj.transform.forward * Input.GetAxis("Vertical")).normalized * dashForce;
        //     if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
        //     {
        //         dashDir = (playerObj.transform.forward * -1).normalized * dashForce;
        //     }
        //     //do extended dash stuff here.
        //     if (isGrounded)
        //     {
        //         rb.AddForce(dashDir, ForceMode.Impulse);
        //     }
        //     else if (!isGrounded && currMidairBoosts > 0)
        //     {
        //         currMidairBoosts--;
        //         rb.AddForce(dashDir, ForceMode.Impulse);
        //     }
        //     dashing = true;
        // }
        // dashTapCount = 0;
        // yield return new WaitForSeconds(0.25f);
        // dashing = false;
        //Active code: Tap once to dash, tap once with no directional input to backstep. Dash input ignored when button is held.'
        allowedToDash = false;
        yield return new WaitForSeconds(doubleTapDelay / 2);
        if (Input.GetButton("DODGE")) { allowedToDash = true; yield break; }
        dashing = true;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        if (horizontalInput == 0 && verticalInput == 0)
        {
            //do backstep
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            Vector3 dashDir = playerObj.transform.forward * -1;
            //Backstepping is not affected by player rotation
            dashDir.y = 0;
            dashDir.Normalize();
            rb.AddForce(dashDir * dashForce / 2, ForceMode.Impulse);
            yield return new WaitForSeconds(0.2f);


        }
        else if (isGrounded || (!isGrounded && currMidairBoosts > 0))
        {
            //do full directional dash

            //Get direction of the dash based on player input
            Vector3 dashDir = playerObj.transform.right * Input.GetAxis("Horizontal") + playerObj.transform.forward * Input.GetAxis("Vertical");

            dashDir.Normalize();
            rb.AddForce(dashDir * dashForce, ForceMode.Impulse);
            //subtract if not grounded
            if (!isGrounded) { currMidairBoosts--; }
            yield return new WaitForSeconds(0.25f);

        }
        dashing = false;
        yield return new WaitForSeconds(0.2f);
        allowedToDash = true;

    }

    private void FindLockableTarget()
    {
        GameObject[] lockables = GameObject.FindGameObjectsWithTag("TargetPoint");
        currentTargetLock = lockables[0].transform;

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
            currCamMode = value;
            if (value == CameraMode.Free)
            {
                currentTargetLock = null;
                myCamera.enabled = true;
                combatLockCamera.enabled = false;
            }
            else
            {
                combatLockCamera.enabled = true;
                FindLockableTarget();
            }
        }
    }

}


