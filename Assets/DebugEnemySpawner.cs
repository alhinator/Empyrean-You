using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugEnemySpawner : MonoBehaviour
{
    GameObject[] thingsToRespawn;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnRespawn()
    {
        foreach (var thing in thingsToRespawn)
        {
        }
    }


}
