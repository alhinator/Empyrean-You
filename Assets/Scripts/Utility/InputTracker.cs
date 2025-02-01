using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTracker : MonoBehaviour
{

    public Player3PCam player3PCam;
    public TMP_Text camTracker;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        string txt = "horiz:" + InputSystem.actions.FindAction("Move").ReadValue<Vector2>().x + "\nvert:" + InputSystem.actions.FindAction("Move").ReadValue<Vector2>().y + " \npan L/R:" + InputSystem.actions.FindAction("Look").ReadValue<Vector2>().x + " \npan U/D:" + InputSystem.actions.FindAction("Look").ReadValue<Vector2>().y;
        txt += "\ndashing:" + player3PCam.IsDashing;
        txt += "\ncan dash:" + player3PCam.CanDash;
        Vector3 d = player3PCam.DistanceFromCurrentTarget;
        txt += "\n Distance to current target: (" + d.x + ", " + d.y + ", " + d.z;
        if (InputSystem.actions.FindAction("Dodge").IsPressed()) { txt += "\ndodge or sprint"; }
        if (InputSystem.actions.FindAction("Jump").IsPressed()) { txt += "\njump"; }
        if (InputSystem.actions.FindAction("CameraLock").IsPressed()) { txt += "\ncam lock"; }
        GetComponent<TMP_Text>().text = txt;


        camTracker.text = "free:" + player3PCam.unlockLookCamera.Priority +
        "\nlock:" + player3PCam.combatLockCamera.Priority + 
        "\nlockaerial:" + player3PCam.aerialCombatCamera.Priority +
        "\naerialclose:" + player3PCam.aerialCloseCamera.Priority;
    }
}
