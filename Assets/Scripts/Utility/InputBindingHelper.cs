using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public static class InputBindingHelper
{
    public enum DeviceType
    {
        Keyboard = 0,
        Gamepad = 1
    }
    public static List<InputBinding> GetBinding(string actionName, DeviceType deviceType, PlayerInput _playerInput)
    {
        //uses modified code from https://www.reddit.com/r/Unity3D/comments/18zdq3k/on_the_new_input_system_how_do_i_get_a_display/

        //old code:
        //return _playerInput.actions[actionName].bindings[(int)deviceType];
        List<InputBinding> ib = new();

        var action = _playerInput.actions[actionName];
        //Debug.Log("We are looking at all the bindings of " + action.name);
        int count = action.bindings.Count;
        int foundComposites = 0;
        for (int i = 0; i < count; i++)
        {
            InputBinding binding = action.bindings[i];

            if (!binding.isComposite && !binding.isPartOfComposite && SchemeMatchesDevice(binding, deviceType))
            {
                ib.Add(binding);
                //Debug.Log("non-composite found that matches device type: " + binding.effectivePath);
                return ib;
            }

            if (binding.isComposite && !binding.isPartOfComposite)
            {
                //this means we have found a composite binding.
                foundComposites++;
                // Debug.Log("found composite: " + binding.name);

                //we've found X composites which means we've fouind the correct device. now look for its constituents
                int constituent = i + 1;
                while (constituent < count)
                {
                    InputBinding c = action.bindings[constituent];
                    if (c.isPartOfComposite && SchemeMatchesDevice(c, deviceType))
                    {
                        ib.Add(c);
                        Debug.Log("found constituent: " + c.effectivePath);
                    }
                    constituent++;


                }
                //Debug.Log(ib.Count);
                //Debug.Log(String.Join(" ", ib));

                if (ib.Count > 0)
                {
                return ib;

                }


            }
        }

        return null;
    }

    private static bool SchemeMatchesDevice(InputBinding binding, DeviceType device)
    {
        string scheme = binding.groups;
        if (scheme.Contains("Keyboard&Mouse") && device == DeviceType.Keyboard)
        {
            return true;
        }
        else if (scheme.Contains("Gamepad") && device == DeviceType.Gamepad)
        {
            return true;
        }
        return false;
    }

}