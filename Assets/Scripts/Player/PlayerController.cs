using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code for the third person camera control taken from https://www.youtube.com/watch?v=UCwwn2q4Vys

public class PlayerController : MonoBehaviour
{
    [Header("ThirdPersonCameraVariables")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    public float rotationSpeed;


    private void Update(){
        Do3PCameraMovement();
    }

    private void Do3PCameraMovement(){
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
    }
}
