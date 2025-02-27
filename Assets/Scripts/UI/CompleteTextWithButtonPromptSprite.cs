//Code taken & modified from https://www.youtube.com/watch?v=JdGgrMWIknE
// and https://www.youtube.com/watch?v=eVMy_Umjcys

using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

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
        InputBinding dynamicBinding = InputBindingHelper.GetBinding(actionName, deviceType, _playerInput);
        TMP_SpriteAsset spriteAsset = spriteAssets.SpriteAssets[(int)deviceType];

        string stringButtonName = dynamicBinding.effectivePath;
        stringButtonName = RenameInput(stringButtonName);

        return $"<sprite=\"{spriteAsset.name}\" name=\"{stringButtonName}\">";
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