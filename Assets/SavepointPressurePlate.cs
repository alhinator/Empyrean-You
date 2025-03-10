using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavepointPressurePlate : MonoBehaviour
{
    [SerializeField] Savepoint sp;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sp.openDoor();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sp.closeDoor();
        }
    }
}
