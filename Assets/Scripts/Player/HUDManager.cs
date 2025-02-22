using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Player3PCam player3PCam;
    public PlayerCombatManager playerCombatManager;
    public Camera mainCamera;
    public Canvas staticPlayerHud;
    public TMP_Text BoostBar;
    public TMP_Text HealthBar;
    // Start is called before the first frame update
    public TMP_Text AlertBar;
    private float timeSinceAlert;

    [Header("Reticle Variables")]
    public float reticleSpeed;
    public Image reticle;
    Vector2 reticleIdealPosition;

    private Queue<string> notificationQueue;
    private Queue<Color> notificationQueueColors;
    void Start()
    {
        notificationQueue = new();
        notificationQueueColors = new();
        ClearAlerts();
    }

    // Update is called once per frame
    void Update()
    {
        //this needs to be converted to an event-based system to avoid hud updates every frame.
        UpdateBasicHudText();
        SetReticleIdealPosition();
        MoveReticle();
        timeSinceAlert += Time.deltaTime;
        if(timeSinceAlert > 3){ ClearAlerts();}
    }

    private void UpdateBasicHudText()
    {
        //Boost bar squares
        string tmp = "";
        for (int i = 0; i < player3PCam.BoostsRemaining; i++) { tmp += "*"; }
        BoostBar.text = tmp;

        //Health bar squares
        string tmp2 = "";
        for (int i = 0; i < playerCombatManager.CurrentHP; i++) { tmp2 += "*"; }
        HealthBar.text = tmp2;

    }
    private void SetReticleIdealPosition()
    {
        if (player3PCam.currentTargetLock)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(staticPlayerHud.GetComponent<RectTransform>(), mainCamera.WorldToScreenPoint(player3PCam.currentTargetLock.position), mainCamera, out Vector2 temp);
            reticleIdealPosition = new Vector3(temp.x, temp.y, 0);
        }
        else
        {
            reticleIdealPosition = new Vector3(0, 0, 0);
        }
    }
    private void MoveReticle()
    {
        float dist = Vector2.Distance(reticle.transform.localPosition, reticleIdealPosition);
        float adjustedReticleSpeed = reticleSpeed * Time.deltaTime;
        if (dist < 30)
        {
            adjustedReticleSpeed *= 3;
        }
        reticle.rectTransform.localPosition = Vector3.Lerp(reticle.transform.localPosition, reticleIdealPosition, adjustedReticleSpeed);

    }

    public void QueueAlert(string alertText, Color c, bool priority)
    {
        if (!priority)
        {
            notificationQueue.Enqueue(alertText);
            notificationQueueColors.Enqueue(c);
            StartCoroutine(AlertUpdater());
        }
        else
        {
            DisplayAlert(alertText, c);
        }
    }
    private void DisplayAlert(string alertText, Color c)
    {
        AlertBar.text = alertText;
        AlertBar.color = c;
        timeSinceAlert = 0;
    }
    public void ClearAlerts()
    {
        AlertBar.text = "";
        AlertBar.color = Color.white;
    }
    private IEnumerator AlertUpdater()
    {
        while (timeSinceAlert < 2)
        {
            yield return new WaitForSeconds(0.2f);
        }
        
        DisplayAlert(notificationQueue.Dequeue(), notificationQueueColors.Dequeue());
        

    }
}
