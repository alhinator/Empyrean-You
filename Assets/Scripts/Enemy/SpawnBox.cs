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
            en.transform.position = centerPoint.position;
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
