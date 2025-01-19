using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputTracker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TMP_Text>().text = "horiz:" + Input.GetAxis("Horizontal") + "\nvert:"+ Input.GetAxis("Vertical") + " \npan L/R:"+ Input.GetAxis("Mouse X") + " \npan U/D:" + Input.GetAxis("Mouse Y");

    }
}
