using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public Player3PCam player3PCam;
    public Canvas staticPlayerHud;
    public TMP_Text BoostBars;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //this needs to be converted to an event-based system to avoid hud updates every frame.
        UpdateBasicHudText();
    }

    private void UpdateBasicHudText()
    {
        //Boost bar squares
        string tmp = "";
        for (int i = 0; i < player3PCam.BoostsRemaining; i++) { tmp += "*"; }
        BoostBars.text = tmp;
    }
}
