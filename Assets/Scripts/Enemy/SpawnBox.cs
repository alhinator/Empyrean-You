using UnityEngine;

public class SpawnBox : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public GameObject EnemyParent;
    public int numToSpawn;
    [SerializeField] private Transform centerPoint;


    private void SpawnMyDudes(){
        for(int i = 0 ; i < numToSpawn ; i++){
            GameObject en = Instantiate(EnemyPrefab, EnemyParent.transform);          
            en.transform.position = centerPoint.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter Spawnbox");
        if(other.CompareTag("Player")){
            SpawnMyDudes();
            Destroy(this.gameObject);
        }
    }   
}
