using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Player3PCam player3PCam;
    public PlayerCombatManager playerCombatManager;
    [SerializeField] HelperPopupPanel helperPopupPanel;
    public Camera mainCamera;
    public Canvas staticPlayerHud;
    public TMP_Text BoostBar;
    public TMP_Text BoostLabel;
    public TMP_Text HealthBar;
    // Start is called before the first frame update
    public TMP_Text AlertBar;
    private float timeSinceAlert;

    private Image everythingBlocker;
    private float idealBlockerOpacity = 0f;

    [Header("Reticle Variables")]
    public float reticleSpeed;
    public Image reticle;
    Vector2 reticleIdealPosition;

    private Queue<string> notificationQueue;
    private Queue<Color> notificationQueueColors;
    void Start()
    {
        everythingBlocker = GameObject.FindGameObjectWithTag("EverythingBlocker").GetComponent<Image>();
        //set opacity manually to 1 so that we get a fade-in
        everythingBlocker.color = new Color(everythingBlocker.color.r, everythingBlocker.color.g, everythingBlocker.color.b, 1);
        LightsUp();


        notificationQueue = new();
        notificationQueueColors = new();
        ClearAlerts();
    }
    void OnEnable()
    {
        InputSystem.onActionChange += TrackActions;

    }
    void OnDisable()
    {
        InputSystem.onActionChange -= TrackActions;

    }
    // Update is called once per frame
    void Update()
    {
        //this needs to be converted to an event-based system to avoid hud updates every frame.
        UpdateBasicHudText();
        SetReticleIdealPosition();
        MoveReticle();
        timeSinceAlert += Time.deltaTime;
        if (timeSinceAlert > 3) { ClearAlerts(); }

        //set alpha of everythign blocker
        float new_alpha = Mathf.MoveTowards(everythingBlocker.color.a, idealBlockerOpacity, Time.deltaTime/5);
        everythingBlocker.color = new Color(everythingBlocker.color.r, everythingBlocker.color.g, everythingBlocker.color.b, new_alpha);
    }

    private void UpdateBasicHudText()
    {
        //Boost bar squares
        string tmp = "";
        for (int i = 0; i < player3PCam.BoostsRemaining; i++) { tmp += "*"; }
        BoostBar.text = tmp;
        BoostLabel.color = player3PCam.BoostsRemaining == 0 ? Color.red : Color.green;

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

        //debug only:
        //reticle.rectTransform.localPosition = reticleIdealPosition;


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
    private void OnDismissPopup(InputValue v)
    {
        helperPopupPanel.OnDismissPopup(v);
    }
    private void TrackActions(object obj, InputActionChange change)
    {
        helperPopupPanel.TrackActions(obj, change);
    }

    public void Blackout()
    {
        idealBlockerOpacity = 1f;
    }
    public void LightsUp()
    {
        idealBlockerOpacity = 0f;
    }
}
