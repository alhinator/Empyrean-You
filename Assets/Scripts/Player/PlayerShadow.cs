using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    [SerializeField] private Player3PCam player;


    void Update()
    {
        Vector3 origin = player.transform.position;
        Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("WalkableTerrain", "CameraObstacle"));
        if(hit.point != null)
        {
            transform.position = hit.point;
            //this line from https://discussions.unity.com/t/rotate-object-to-normal-of-face-below/752531/2
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
    }
}
