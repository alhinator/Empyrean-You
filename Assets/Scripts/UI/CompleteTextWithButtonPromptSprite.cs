//Code taken & modified from https://www.youtube.com/watch?v=JdGgrMWIknE
// and https://www.youtube.com/watch?v=eVMy_Umjcys

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using static System.Collections.Specialized.BitVector32;

public static class CompleteTextWithButtonPromptSprite
{

    private static readonly string ACTION_PATTERN = @"\{(.*?)\}";
    private static readonly RegexOptions OPT = RegexOptions.Multiline;

    private static Regex REGEX = new Regex(ACTION_PATTERN, OPT);

    // public static string ReadAndReplaceBinding(string textToDisplay, string actionString, InputBinding actionNeeded, TMP_SpriteAsset spriteAsset)
    // {
    //     string stringButtonName = actionNeeded.ToString();
    //     stringButtonName = RenameInput(stringButtonName);

    //     textToDisplay = textToDisplay.Replace(
    //         oldValue: "BUTTONPROMPT",
    //         newValue: $"<sprite=\"{spriteAsset.name}\" name=\"{stringButtonName}\">");

    //     return textToDisplay;
    // }

    public static string GetSpriteTag(string actionName, InputBindingHelper.DeviceType deviceType, PlayerInput _playerInput, ListOfTmpSpriteAssets spriteAssets)
    {

        List<InputBinding> dynamicBindings = InputBindingHelper.GetBinding(actionName, deviceType, _playerInput);
        //string stringButtonName = String.Join(" ", dynamicBindings);
        TMP_SpriteAsset spriteAsset = spriteAssets.SpriteAssets[(int)deviceType];
        string retString = "";


        foreach (InputBinding stringButtonName in dynamicBindings)
        {
            string renamedName = RenameInput(stringButtonName.effectivePath);
            if (dynamicBindings.Count > 1) { Debug.Log("long bind, heres me:" + renamedName); }

            retString +=  $"<sprite=\"{spriteAsset.name}\" name=\"{renamedName}\"> ";
        }
        return retString;
        
    }

    private static string RenameInput(string stringButtonName)
    {
        stringButtonName = stringButtonName.Replace(oldValue: "<Keyboard>/", newValue: "Keyboard_");
        stringButtonName = stringButtonName.Replace(oldValue: "<Mouse>/", newValue: "Mouse_");
        stringButtonName = stringButtonName.Replace(oldValue: "<Gamepad>/", newValue: "Gamepad_");
        //Debug.Log("returning: " + stringButtonName);

        return stringButtonName;
    }

    public static string ReplaceAllBindings(string textWithActions, InputBindingHelper.DeviceType deviceType, PlayerInput _playerInput, ListOfTmpSpriteAssets spriteAssets)
    {
        MatchCollection matches = REGEX.Matches(textWithActions);
        string replacedText = textWithActions;
        foreach (Match m in matches)
        {
            var withBraces = m.Groups[0].Captures[0].Value;
            var noBraces = m.Groups[1].Captures[0].Value;
            //Debug.LogFormat("match is {0} / {1}", withBraces, noBraces);
            var tagText = GetSpriteTag(noBraces, deviceType, _playerInput, spriteAssets);
            replacedText = replacedText.Replace(withBraces, tagText);
        }

        return replacedText;
    }

}