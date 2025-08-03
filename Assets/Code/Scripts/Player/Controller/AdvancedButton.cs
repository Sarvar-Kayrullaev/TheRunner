using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AdvancedButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [HideInInspector] public Vector2 vector;
    [HideInInspector] public bool isPressing = false;
    public new ControllerCases name;
    public PlayerData data;
    public Transform Player;
    ButtonListener buttonListener;
    private int frameFirst, frameSecond;

    private void Start()
    {
        if (Player)
        {
            if (Player.GetComponent<ButtonListener>() != null)
            {
                buttonListener = Player.GetComponent<ButtonListener>();
            }
        }
    }

    private void Update()
    {
        if (frameFirst == frameSecond)
        {
            vector = Vector2.zero;
            //data.transform.GetComponent<PlayerController>().holster.currentWeapon.Sway(Vector2.zero);
            return;
        };
        frameFirst = frameSecond;
    }

    public void LookAround(Transform transformX, Transform transformY, float lookSensitivityX, float lookSensitivityY)
    {

        if (frameSecond == 0) return;

        // vertical (pitch) rotation
        data.cameraPitch = Mathf.Clamp(data.cameraPitch - vector.y * lookSensitivityY, -90f, 90f);
        transformY.localRotation = Quaternion.Euler(data.cameraPitch, 0, 0);

        // horizontal (yaw) rotation
        transformX.Rotate(transform.up, vector.x * lookSensitivityX);
    }

    public void OnDrag(PointerEventData eventData)
    {
        vector = eventData.delta;
        //data.transform.GetComponent<PlayerController>().holster.currentWeapon.Sway(vector);
        frameSecond++;
        if (buttonListener != null)
        {
            buttonListener.OnDragChange(eventData, name);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        vector = eventData.delta;
        isPressing = true;
        if (buttonListener != null)
        {
            buttonListener.OnClickDown(name);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        vector = Vector2.zero;
        frameSecond = 0;
        isPressing = false;
        if (buttonListener != null)
        {
            buttonListener.OnClickUp(name);
        }
    }
}