using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class ControlHelperPanel : MonoBehaviour
{
    private PlayerInput _playerInput;
    [SerializeField] private InputBindingHelper.DeviceType activeDevice = InputBindingHelper.DeviceType.Keyboard;
    public Vector2 idealPosition;
    private StringTable msgStrings;
    [SerializeField] Vector2 OnScreenPos, OffScreenPos;
    [SerializeField] ListOfTmpSpriteAssets listOfTmpSpriteAssets;

    [SerializeField] TMP_Text messageText;
    void Awake()
    {
        _playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
    }
    void OnEnable()
    {
        _playerInput.currentActionMap.Enable();

    }
    public void TrackActions(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            InputAction inputAction = (InputAction)obj;
            InputControl activeControl = inputAction.activeControl;
            var last = activeDevice;
            if (activeControl.device is Keyboard)
            {
                activeDevice = InputBindingHelper.DeviceType.Keyboard;
            }
            if (activeControl.device is Mouse)
            {
                activeDevice = InputBindingHelper.DeviceType.Keyboard;
            }
            if (activeControl.device is Gamepad)
            {
                activeDevice = InputBindingHelper.DeviceType.Gamepad;
            }

            if (activeControl.device is Keyboard || activeControl.device is Mouse && last == InputBindingHelper.DeviceType.Gamepad
                || (activeControl.device is Gamepad && last == InputBindingHelper.DeviceType.Keyboard))
            { //input device different than previous active device? swap the text. hardcoded for now
                //Debug.Log("In track actions last is different!");
                //Debug.Log(activeDevice);

                string message = msgStrings.GetEntry("ui.controlPanel").Value;
                messageText.text = CompleteTextWithButtonPromptSprite.ReplaceAllBindings(message, activeDevice, _playerInput, listOfTmpSpriteAssets);
            }
        }
    }
    void Start()
    {
        msgStrings = LocalizationSettings.StringDatabase.GetTable("Messages");
        idealPosition = OffScreenPos;
        //transform.localPosition = idealPosition;
        string message = msgStrings.GetEntry("ui.controlPanel").Value;
        messageText.text = CompleteTextWithButtonPromptSprite.ReplaceAllBindings(message, activeDevice, _playerInput, listOfTmpSpriteAssets);

    }

    // Update is called once per frame
    void Update()
    {
        //transform.localPosition = Vector2.MoveTowards(transform.localPosition, idealPosition, 1 * Time.deltaTime);

    }
}
