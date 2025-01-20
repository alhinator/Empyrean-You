using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code for the third person camera control taken from https://www.youtube.com/watch?v=UCwwn2q4Vys

public class PlayerController : MonoBehaviour
{
    private Animator animController;



    private void Start()
    {
        animController = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        AlignAnimatorInputs();

    }

    private void AlignAnimatorInputs()
    {
        animController.SetFloat("InputXAxis", Input.GetAxis("Horizontal"));
        animController.SetFloat("InputYAxis", Input.GetAxis("Vertical"));
    }
}
