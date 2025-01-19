using System;
using System.Collections;
using UnityEngine;

public class Player3PCam : MonoBehaviour
{
    [Header("ThirdPersonCameraVariables")]
    public Transform orientation;
    public Transform player;
    public Transform combatFocus;
    public Transform playerObj;
    public float rotationSpeed;


    [Header("PlayerMovementVariables")]
    public Rigidbody rb;
    public float moveSpeed;
    public float sprintSpeed;
    public float playerHeight;
    public LayerMask terrainMask;
    public float groundDrag;
    private bool isGrounded;
    public float jumpForce;
    public float airMultiplier;
    public float jumpCooldown;
    private float canJump;
    public float doubleTapDelay;
    private float dashTapCount;
    public float dashForce;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = player.GetComponent<Rigidbody>();
        rb.freezeRotation = true;

    }
    private void Update()
    {
        Do3PCameraMovement();
        AssignGroundDrag();
        DoSpeedControl();
        DoJumpCheck();
        DoDashCheck();

        //DEBUG CODE ONLY
        if (Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            player.transform.position = new Vector3(0, 2f, 0);
            rb.velocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    private void Do3PCameraMovement()
    {
        //Code adapted from https://www.youtube.com/watch?v=UCwwn2q4Vys

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        if (horizontalInput != 0 || verticalInput != 0)
        {
            Vector3 offset = new Vector3(orientation.forward.x, orientation.forward.y, orientation.forward.z);
            if (Input.GetAxis("SPRINT") > 0 && verticalInput > 0)
            {
                offset = Quaternion.AngleAxis(45 * horizontalInput, Vector3.up) * offset;
            }
            playerObj.forward = Vector3.Slerp(playerObj.forward, offset, Time.deltaTime * rotationSpeed);

        }

    }
    private void MovePlayer()
    {
        //Code adapted from https://www.youtube.com/watch?v=f473C43s8nE
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float sprinting = Input.GetAxis("SPRINT");

        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded)
        {
            if (sprinting > 0 && verticalInput > 0)
            {
                playerObj.GetComponentInChildren<Animator>().SetBool("Sprinting", true);
                rb.AddForce(moveDirection.normalized * sprintSpeed, ForceMode.Force);
            }
            else
            {
                playerObj.GetComponentInChildren<Animator>().SetBool("Sprinting", false);
                rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);
            }


        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
        }
    }
    private void AssignGroundDrag()
    {
        //Code adapted from https://www.youtube.com/watch?v=f473C43s8nE
        isGrounded = Physics.Raycast(player.transform.position, Vector3.down, playerHeight * 0.5f + 0.05f, terrainMask);
        playerObj.GetComponentInChildren<Animator>().SetBool("Grounded", isGrounded);


        rb.drag = isGrounded ? groundDrag : groundDrag / 3;
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
        if (Input.GetButtonDown("DODGE"))
        {
            if (dashTapCount == 0)
            {
                Debug.Log("Taps = 1");
                dashTapCount = 1;
                StartCoroutine(DashTap());
            }
            else if (dashTapCount == 1)
            {
                Debug.Log("Taps = 2");
                dashTapCount = 2;
                StartCoroutine(DashTap());
            }
        }
    }

    private IEnumerator DashTap()
    {
        yield return new WaitForSeconds(doubleTapDelay);
        if (dashTapCount == 1)
        {
            if (Input.GetButton("SPRINT"))
            { //Means they held the button down
            }
            else
            {
                //do dash stuff here.
                rb.AddForce(playerObj.transform.forward * -1 * dashForce / 2, ForceMode.Impulse);
            }
        }
        else if (dashTapCount == 2)
        {
            StopCoroutine(DashTap()); //Stop the other occurence of this coroutine.

            if (Input.GetButton("SPRINT"))
            { //Means they held the button down
            }
            else
            {
                rb.AddForce((playerObj.transform.right * Input.GetAxis("Horizontal") + playerObj.transform.forward * Input.GetAxis("Vertical")).normalized * dashForce, ForceMode.Impulse);
                //do extended dash stuff here.
            }

        }
        dashTapCount = 0;


    }

}


