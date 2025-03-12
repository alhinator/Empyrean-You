using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

//Contains modified code from https://www.youtube.com/watch?v=JdGgrMWIknE
//and https://www.youtube.com/watch?v=eVMy_Umjcys

public class HelperPopupPanel : MonoBehaviour
{
    private PlayerInput _playerInput;
    [SerializeField] private InputBindingHelper.DeviceType activeDevice = InputBindingHelper.DeviceType.Keyboard;
    public Vector2 idealPosition;

    private StringTable msgStrings;

    public float moveSpeed;
    [SerializeField] Vector2 OnScreenPos, OffScreenPos;

    [SerializeField] TMP_Text messageText, dismissText;
    private string currentOriginalMessage;
    [SerializeField] ListOfTmpSpriteAssets listOfTmpSpriteAssets;

    Queue<string> AlertList;
    private bool currentlyDisplaying = false;

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
                messageText.text = CompleteTextWithButtonPromptSprite.ReplaceAllBindings(currentOriginalMessage, activeDevice, _playerInput, listOfTmpSpriteAssets);
                dismissText.text = CompleteTextWithButtonPromptSprite.ReplaceAllBindings(msgStrings.GetEntry("ui.dismiss").Value, activeDevice, _playerInput, listOfTmpSpriteAssets);
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        msgStrings = LocalizationSettings.StringDatabase.GetTable("Messages");
        idealPosition = OffScreenPos;
        transform.localPosition = idealPosition;
        AlertList = new();


        dismissText.text = CompleteTextWithButtonPromptSprite.ReplaceAllBindings(
            msgStrings.GetEntry("ui.dismiss").Value,
            activeDevice,
            _playerInput,
            listOfTmpSpriteAssets);

        //StartCoroutine(QueueInitial());

    }
    private IEnumerator QueueInitial()
    {
        yield return new WaitForSeconds(0.5f);
        QueueAlert(msgStrings.GetEntry("tutorial.first").Value);
        //QueueAlert(msgStrings.GetEntry("tutorial.second").Value);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, idealPosition, moveSpeed * Time.deltaTime);
        if (!currentlyDisplaying && (Vector2)transform.localPosition == OffScreenPos && AlertList.Count > 0)
        {
            QueueAlert(AlertList.Dequeue());
        }
    }

    public void QueueAlert(string alert)
    {
        if (!currentlyDisplaying)
        {
            currentlyDisplaying = true;
            idealPosition = OnScreenPos;
            SetText(alert);
        }
        else
        {
            AlertList.Enqueue(alert);
        }
        dismissText.text = CompleteTextWithButtonPromptSprite.ReplaceAllBindings(
            msgStrings.GetEntry("ui.dismiss").Value,
            activeDevice,
            _playerInput,
            listOfTmpSpriteAssets);
    }
    public void OnDismissPopup(InputValue v)
    {
        if (v.Get<float>() == 1)
        {
            currentlyDisplaying = false;
            idealPosition = OffScreenPos;
        }
    }

    public void SetText(string message)
    {
        if ((int)activeDevice > listOfTmpSpriteAssets.SpriteAssets.Count - 1)
        {
            //missing sprite asset for this device type.
            return;
        }
        currentOriginalMessage = message;
        messageText.text = CompleteTextWithButtonPromptSprite.ReplaceAllBindings(message, activeDevice, _playerInput, listOfTmpSpriteAssets);
    }




}
