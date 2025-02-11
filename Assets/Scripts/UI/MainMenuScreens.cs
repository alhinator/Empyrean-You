using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class MainMenuScreens : MonoBehaviour
{
    private string savedLetters = "";
    [SerializeField] private TMP_Text ColorSelectHeader;
    [SerializeField] private TMP_Text LeftScreenHeader;
    [SerializeField] private GameObject Keypad;
    [SerializeField] private GameObject Navigator;

    [SerializeField] private Image NavImage;
    [SerializeField] private TMP_Text NavHeader;
    [SerializeField] private TMP_Text NavBody;

    private StringTable abilStrings;

    public enum FRAME { UNSELECTED, BAST }
    private FRAME selectedFrame = FRAME.UNSELECTED;

    public enum GUN{UNSELECTED, ARTEMIS, GUANYIN}
    private GUN? selectedGun = GUN.UNSELECTED;

    void Start()
    {
        abilStrings = LocalizationSettings.StringDatabase.GetTable("Abilities", null);

        ClearText();
    }

    public void AppendLetter(string c)
    {
        if (savedLetters.Length < 6)
        {
            savedLetters += c;
            string s = "#" + savedLetters;
            for (int i = s.Length; i < 7; i++)
            {
                s += "_";
            }
            ColorSelectHeader.text = s;
        }


    }
    public void ClearText()
    {
        ColorSelectHeader.text = "#______";
        savedLetters = "";
    }
    public void SubmitToManager()
    {
        if (savedLetters.Length < 6) { return; }
        ColorUtility.TryParseHtmlString("#" + savedLetters, out Color c);
        if (c != null)
        {
            GetComponent<MainMenuScript>().SetPlayerColor(c);
        }
    }
    public string ColorHeader
    {
        get
        {
            return LeftScreenHeader.text;
        }
        set
        {
            LeftScreenHeader.text = value;
        }

    }

    public bool KeypadEnabled
    {
        get
        {
            return Keypad.activeInHierarchy;
        }
        set
        {
            Keypad.SetActive(value);
        }


    }
    public bool NavigatorEnabled
    {
        get
        {
            return Navigator.activeInHierarchy;
        }
        set
        {
            Navigator.SetActive(value);
        }
    }

    public void DisplayFrameDetails(FRAME f)
    {
        switch (f)
        {
            case FRAME.BAST:
                NavHeader.text = abilStrings.GetEntry("bast.name").Value;
                NavBody.text = abilStrings.GetEntry("bast.splash").Value;
                selectedFrame = FRAME.BAST;
                break;
        }
    }
    public void DisplayGunDetails(GUN g){

    }
    public void NavNext(){

    }
    public void NavPrev(){

    }
    public void NavSubmit(){
        if(selectedFrame != FRAME.UNSELECTED && selectedGun == GUN.UNSELECTED)
        {
            DisplayGunDetails(GUN.ARTEMIS);
            MainMenuScript.SetSelectedFrame(selectedFrame);
        }
    }
}