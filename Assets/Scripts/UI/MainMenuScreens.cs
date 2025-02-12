using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

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
    private StringTable gunStrings;

    public enum FRAME { UNSELECTED, BAST, LAST }
    private FRAME selectedFrame = FRAME.UNSELECTED;

    public enum GUN { UNSELECTED, GUANYIN, ARTEMIS, LAST }
    private GUN selectedGun = GUN.UNSELECTED;

    void Start()
    {
        abilStrings = LocalizationSettings.StringDatabase.GetTable("Abilities", null);
        gunStrings = LocalizationSettings.StringDatabase.GetTable("Guns", null);

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
    public void DisplayGunDetails(GUN g)
    {
        switch (g)
        {
            case GUN.ARTEMIS:
                NavHeader.text = gunStrings.GetEntry("artemis.name").Value;
                NavBody.text = gunStrings.GetEntry("artemis.splash").Value;
                break;
            case GUN.GUANYIN:
                NavHeader.text = gunStrings.GetEntry("guanyin.name").Value;
                NavBody.text = gunStrings.GetEntry("guanyin.splash").Value;
                break;
        }
        selectedGun = g;
    }
    public void NavNext()
    {
        if (MainMenuScript.currState == MainMenuScript.STATE.POWERSELECT)
        { //on frame select

            selectedFrame++;
            if (selectedFrame == FRAME.LAST)
            {
                selectedFrame = FRAME.UNSELECTED;
                selectedFrame++;
            }
            Debug.Log(selectedFrame);
            DisplayFrameDetails(selectedFrame);

        }
        else
        { //on gun select
            selectedGun++;
            if (selectedGun == GUN.LAST)
            {
                selectedGun = GUN.UNSELECTED;
                selectedGun++;
            }
            Debug.Log(selectedGun);
            DisplayGunDetails(selectedGun);
        }


    }
    public void NavPrev()
    {
        if (MainMenuScript.currState == MainMenuScript.STATE.POWERSELECT)
        { //on frame select

            selectedFrame--;
            if (selectedFrame == FRAME.UNSELECTED)
            {
                selectedFrame = FRAME.LAST;
                selectedFrame--;
            }
            DisplayFrameDetails(selectedFrame);

        }
        else
        { //on gun select
            selectedGun--;
            if (selectedGun == GUN.UNSELECTED)
            {
                selectedGun = GUN.LAST;
                selectedGun--;
            }
            DisplayGunDetails(selectedGun);
        }
    }
    public void NavSubmit()
    {

        switch (MainMenuScript.currState)
        {
            case MainMenuScript.STATE.POWERSELECT:
                MainMenuScript.SetSelectedFrame(selectedFrame);
                break;
            case MainMenuScript.STATE.GUNSELECT:
                MainMenuScript.SetSelectedGun(selectedGun, true);
                break;
            case MainMenuScript.STATE.GUNSELECT2:
                MainMenuScript.SetSelectedGun(selectedGun, false);
                break;
        }
    }
}