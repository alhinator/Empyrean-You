using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

public class TooltipPopup : MonoBehaviour
{
    [SerializeField] private string TableName;
    [SerializeField] private string TableEntry;
    public bool requiresInteraction;

    private string actualMessage;
    private bool playerNear;


    void Start()
    {
        actualMessage = LocalizationSettings.StringDatabase.GetTable(TableName).GetEntry(TableEntry).Value;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            playerNear = true;
            if (!requiresInteraction)
            {
                GameObject.FindGameObjectWithTag("HelperPopupPanel").GetComponent<HelperPopupPanel>().QueueAlert(actualMessage);
                Destroy(this.gameObject);
            }
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            playerNear = false;
        }
    }
    public void DoMyMessage()
    {
        if (playerNear)
        {
            HelperPopupPanel h = GameObject.FindGameObjectWithTag("HelperPopupPanel").GetComponent<HelperPopupPanel>();
            h.QueueAlert(actualMessage);
        }
    }
}


