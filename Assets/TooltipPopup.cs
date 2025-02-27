using UnityEngine;
using UnityEngine.Localization.Settings;

public class TooltipPopup : MonoBehaviour
{
    [SerializeField] private string TableName;
    [SerializeField] private string TableEntry;

    private string actualMessage;

    void Start(){
        actualMessage = LocalizationSettings.StringDatabase.GetTable(TableName).GetEntry(TableEntry).Value;
    }

    void OnTriggerEnter(Collider col){
        if(col.CompareTag("Player")){
            GameObject.FindGameObjectWithTag("HelperPopupPanel").GetComponent<HelperPopupPanel>().QueueAlert(actualMessage);
            Destroy(this.gameObject);
        }
    }
    
}
