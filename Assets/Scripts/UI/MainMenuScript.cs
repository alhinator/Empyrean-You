using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public enum STATE { BOOTUP, ELEVATOR, COLORSELECT, COLORSELECT2, POWERSELECT, GUNSELECT, NAMESELECT };
    private static MainMenuScript Singleton;
    [Header("Canvases")]
    public Canvas worldCanvas;
    public Canvas mainMenuCanvas;
    public Canvas leftScreen;
    public Canvas rightScreen;
    public Canvas remappingCanvas;

    [Header("Objects")]
    public Transform elevator;
    public EventSystem eventSystem;

    [Header("State")]
    private static STATE currState;

    [Header("Main Menu Variables")]


    [Header("Color Select Variables")]
    public GameObject selectMeAfterElevator;
    private MainMenuScreens mainMenuScreens;

    [Header("Localization")]
    private StringTable mmStrings;
    public TMP_Text subtitle;

    void Start()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        mmStrings = LocalizationSettings.StringDatabase.GetTable("Title Screen", null);
        mainMenuScreens = GetComponent<MainMenuScreens>();
        Singleton.TransitionToState(STATE.BOOTUP);
    }

    /// <summary>
    /// The manager for the FFSM: Fake Finite State Machine. Unfortunately very smelly but I don't have the leisure to implement a better option atm.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public void TransitionToState(STATE newState)
    {
        //First, anything that needs to be done when leaving the previous state?
        switch (currState)
        {
            case STATE.BOOTUP:
                break;
        }
        //now, anything that happens when entering the next state:
        currState = newState;
        switch (currState)
        {
            case STATE.BOOTUP:
                Singleton.subtitle.enabled = false;
                Singleton.worldCanvas.enabled = false;
                Singleton.mainMenuCanvas.enabled = true;
                Singleton.leftScreen.enabled = false;
                Singleton.rightScreen.enabled = false;
                Singleton.remappingCanvas.enabled = false;
                break;
            case STATE.ELEVATOR:
                Singleton.remappingCanvas.enabled = false;
                Singleton.mainMenuCanvas.enabled = false;
                Singleton.eventSystem.SetSelectedGameObject(null);
                Singleton.StartCoroutine(DescendToHangar());
                break;
            case STATE.COLORSELECT:
                Singleton.mainMenuCanvas.enabled = false;
                Singleton.remappingCanvas.enabled = false;
                Singleton.worldCanvas.enabled = true;
                Singleton.worldCanvas.sortingOrder = 2;
                Singleton.leftScreen.enabled = true;
                Singleton.eventSystem.SetSelectedGameObject(Singleton.selectMeAfterElevator);

                ///play a voiceline here / display text
                Singleton.subtitle.enabled = true;
                Singleton.subtitle.text = mmStrings.GetEntry("title_screen.subtitle.color_select").Value;
                Singleton.mainMenuScreens.ColorHeader = mmStrings.GetEntry("title_screen.left_screen.color_select_1").Value;


                break;
            case STATE.COLORSELECT2:
                mainMenuScreens.ClearText();
                Singleton.mainMenuScreens.ColorHeader = mmStrings.GetEntry("title_screen.left_screen.color_select_2").Value;
                break;

            case STATE.POWERSELECT:
                mainMenuScreens.ClearText();
                Singleton.subtitle.text = mmStrings.GetEntry("title_screen.subtitle.power_select").Value;
                Singleton.mainMenuScreens.ColorHeader = mmStrings.GetEntry("title_screen.left_screen.power_select").Value;
                SceneManager.LoadScene("GrayboxMap");
                break;
        }
    }
    public void StartDescent()
    {
        TransitionToState(STATE.ELEVATOR);
    }
    private IEnumerator DescendToHangar()
    {

        for (float percent = 0; percent <= 1000; percent++)
        {

            Singleton.elevator.position = new Vector3(Singleton.elevator.position.x, Mathf.Lerp(300, 15, percent / 1000), Singleton.elevator.position.z);
            yield return new WaitForSeconds(0.01f);
        }
        Singleton.TransitionToState(STATE.COLORSELECT);

    }

    public void ToggleRemappingCanvas()
    {
        Singleton.remappingCanvas.enabled = !Singleton.remappingCanvas.enabled;
    }

    /// <summary>
    /// A helper function to hook into UI button presses. 
    /// </summary>
    /// <param name="s">The hex string to set as a color.</param>
    /// <param name="setBase">if true, set the fill color of the mech. If false, set the line color</param>
    public void SetPlayerColor(Color c)
    {
        switch (currState)
        {
            case STATE.COLORSELECT:
                GetComponent<PlayerMatSwapper>().SetSavedColor(c, true);
                GetComponent<PlayerMatSwapper>().LerpPlayerColor();

                TransitionToState(STATE.COLORSELECT2);
                break;
            case STATE.COLORSELECT2:
                GetComponent<PlayerMatSwapper>().SetSavedColor(c, false);
                GetComponent<PlayerMatSwapper>().LerpPlayerColor();
                TransitionToState(STATE.POWERSELECT);
                break;
            default:
                break;
        }
    }

}
