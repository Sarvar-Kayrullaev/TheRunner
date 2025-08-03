using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface ButtonListener {
    void OnClickDown (ControllerCases name);
    void OnClickUp (ControllerCases name);
    void OnDragChange (PointerEventData eventData, ControllerCases name);
}
public enum ControllerCases
{
    Jump,
    Crouch,
    Shoot,
    Reload,
    Swipe,
    Aim,
    Throw,
    SwitchWeapon
}