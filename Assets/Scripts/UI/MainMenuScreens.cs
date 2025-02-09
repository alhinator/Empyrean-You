using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class MainMenuScreens : MonoBehaviour
{
    private string savedLetters = "";
    [SerializeField] private TMP_Text ColorSelectHeader;
    [SerializeField] private TMP_Text LeftScreenHeader;

    void Start()
    {
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

}