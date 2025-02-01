using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Player3PCam player3PCam;
    public Camera mainCamera;
    public Canvas staticPlayerHud;
    public TMP_Text BoostBars;
    // Start is called before the first frame update


    [Header("Reticle Variables")]
    public float reticleSpeed;
    public Image reticle;
    Vector2 reticleIdealPosition;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //this needs to be converted to an event-based system to avoid hud updates every frame.
        UpdateBasicHudText();
        SetReticleIdealPosition();
        MoveReticle();

    }

    private void UpdateBasicHudText()
    {
        //Boost bar squares
        string tmp = "";
        for (int i = 0; i < player3PCam.BoostsRemaining; i++) { tmp += "*"; }
        BoostBars.text = tmp;
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
}
