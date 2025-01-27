using UnityEngine;
using UnityEngine.InputSystem;

//Code for the third person camera control taken from https://www.youtube.com/watch?v=UCwwn2q4Vys

public class PlayerController : MonoBehaviour
{
    private Animator animController;



    private void Start()
    {
        animController = GetComponentInChildren<Animator>();
    }


    public void OnMove(InputValue v)
    {
        Vector2 rawInput = v.Get<Vector2>();
        animController.SetFloat("InputXAxis", rawInput.x);
        animController.SetFloat("InputYAxis", rawInput.y);
    }
}
