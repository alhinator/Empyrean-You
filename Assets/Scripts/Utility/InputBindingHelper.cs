using UnityEngine;
using UnityEngine.InputSystem;
public static class InputBindingHelper
{
    public enum DeviceType
    {
        Keyboard = 0,
        Gamepad = 1
    }
    public static InputBinding GetBinding(string actionName, DeviceType deviceType, PlayerInput _playerInput)
    {
        return _playerInput.actions[actionName].bindings[(int)deviceType];
    }

    
}