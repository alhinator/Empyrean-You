using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputTracker : MonoBehaviour
{

    public Player3PCam player3PCam;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        string txt = "horiz:" + Input.GetAxis("Horizontal") + "\nvert:" + Input.GetAxis("Vertical") + " \npan L/R:" + Input.GetAxis("Mouse X") + " \npan U/D:" + Input.GetAxis("Mouse Y");
        txt += "\ndashing:" + player3PCam.IsDashing;
        txt += "\ncan dash:" + player3PCam.CanDash;
        Vector3 d = player3PCam.DistanceFromCurrentTarget;
        txt += "\n Distance to current target: (" + d.x + ", " + d.y + ", " + d.z;
        if (Input.GetButton("DODGE")) { txt += "\ndodge or sprint"; }
        if (Input.GetButton("SPACE")) { txt += "\njump"; }
        if (Input.GetButton("CAMLOCK")) { txt += "\ncam lock"; }
        GetComponent<TMP_Text>().text = txt;
    }
}
