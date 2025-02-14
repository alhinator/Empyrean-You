using UnityEngine;

public class SpawnBox : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public GameObject EnemyParent;
    public int numToSpawn;
    [SerializeField] private Transform centerPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void SpawnMyDudes(){
        for(int i = 0 ; i < numToSpawn ; i++){
            GameObject en = Instantiate(EnemyPrefab, EnemyParent.transform);
            Vector3 spawnPos = centerPoint.position + Vector3.forward * 20;
            
            en.transform.position = spawnPos;
            en.transform.Rotate(centerPoint.position, 360/numToSpawn);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")){
            SpawnMyDudes();
            Destroy(this.gameObject);
        }
    }   
}
