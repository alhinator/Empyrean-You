using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingOscillateScript : MonoBehaviour
{
    public float verticalSpeed;
    public float horizontalSpeed;
    public float zSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Mathf.Sin(Time.time) * horizontalSpeed * Time.deltaTime, Mathf.Sin(Time.time) * verticalSpeed * Time.deltaTime, Mathf.Sin(Time.time) * zSpeed * Time.deltaTime);
    }
}
