using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.transform);
        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
    }
}
