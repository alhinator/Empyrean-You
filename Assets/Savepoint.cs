using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Savepoint : MonoBehaviour
{
    public bool isOneWay = false;
    private float doorSpeed = 10;
    [SerializeField] GameObject door;
    private Vector3 idealDoorPos;
    private Vector3 openPos = new Vector3(-1.25f, -3, -4.1f);
    private Vector3 closedPos = new Vector3(-1.25f, -0.2f, -4.1f);

    private void Start()
    {
        if (isOneWay)
        {
            idealDoorPos = closedPos;
            door.transform.localPosition = closedPos;
        } else
        {
            idealDoorPos = openPos;
            door.transform.localPosition = openPos;
        }
    }
    private void Update()
    {
        door.transform.localPosition = Vector3.MoveTowards(door.transform.localPosition, idealDoorPos, doorSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player inside savepoint!");
            var player = other.GetComponent<PlayerCombatManager>();
            player.Heal((int) player.MaximumHP);
            player.ReplenishAmmo();

           
        }
    }

    public void openDoor()
    {
        StartCoroutine(SetDoor(openPos));
    }
    public void closeDoor()
    {
        StartCoroutine(SetDoor(closedPos));

    }
    private IEnumerator SetDoor(Vector3 newPos)
    {
        yield return new WaitForSeconds(0.1f);
        idealDoorPos = newPos;
    }
}
