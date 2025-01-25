using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//This code taken from https://discussions.unity.com/t/world-space-canvas-cursorlockmode-locked-incompatible/717500/6
public class RaycasterWorld : GraphicRaycaster
{
    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        eventData.position = new(Screen.width/2, Screen.height/2);
        base.Raycast(eventData, resultAppendList);
    }
}
