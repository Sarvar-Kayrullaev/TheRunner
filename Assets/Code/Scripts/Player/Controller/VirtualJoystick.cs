using System.Collections;
using System.Collections.Generic;
using PlayerRoot;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform backgroundJoystick;
    public RectTransform handleJoystick;
    public Vector2 movement;
    Vector3 inputDir;

    void Start()
    {
        inputDir = Vector3.zero;

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPosition = transform.InverseTransformPoint(eventData.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(backgroundJoystick, eventData.position, eventData.pressEventCamera, out currentPosition);

        currentPosition.x = currentPosition.x / backgroundJoystick.sizeDelta.x;
        currentPosition.y = currentPosition.y / backgroundJoystick.sizeDelta.y;

        float x = currentPosition.x * 2;
        float y = currentPosition.y * 2;

        inputDir = new Vector3(x, y, 0);
        inputDir = (inputDir.magnitude > 1) ? inputDir.normalized : inputDir;

        handleJoystick.anchoredPosition = new Vector3(inputDir.x * (backgroundJoystick.sizeDelta.x / (backgroundJoystick.sizeDelta.x / 100)),
                inputDir.y * (backgroundJoystick.sizeDelta.y / (backgroundJoystick.sizeDelta.y / 100)), 0);

        
        float horizontal = handleJoystick.localPosition.x / 100;
        float vertical = handleJoystick.localPosition.y / 100;
        movement = new Vector2(horizontal, vertical);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localPosition = transform.InverseTransformPoint(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputDir = Vector3.zero;
        handleJoystick.anchoredPosition = Vector3.zero;
        movement = Vector2.zero;
    }

    public void moveArround(Transform _transform)
    {
        Player player = _transform.GetComponent<Player>();
        float horizontal = handleJoystick.localPosition.x / 100;
        float vertical = handleJoystick.localPosition.y / 100;
        
        player.vectorMovement = new Vector2(horizontal, vertical);
    }
}
