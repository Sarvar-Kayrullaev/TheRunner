using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LookController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    public PlayerData data;
    [HideInInspector] public Vector2 vector;
    [HideInInspector] public bool isPressing = false;
    private int frameFirst, frameSecond;
    private void Start () {
        
    }

    private void Update () {
        if (frameFirst == frameSecond) {
            vector = Vector2.zero;
            return;
        };
        frameFirst = frameSecond;
        
    }

    public void LookAround (Transform transformX, Transform transformY, float lookSensitivityX, float lookSensitivityY) {
        if (frameSecond == 0) return;
        // vertical (pitch) rotation
        data.cameraPitch = Mathf.Clamp (data.cameraPitch - vector.y * lookSensitivityY, -90f, 90f);
        transformY.localRotation = Quaternion.Euler (data.cameraPitch, 0, 0);

        // horizontal (yaw) rotation
        transformX.Rotate (transform.up, vector.x * lookSensitivityX);
    }

    public void OnDrag (PointerEventData eventData) {
        vector = eventData.delta;
        frameSecond++;
    }

    public void OnPointerDown (PointerEventData eventData) {
        vector = eventData.delta;
        isPressing = true;
    }

    public void OnPointerUp (PointerEventData eventData) {
        vector = Vector2.zero;
        frameSecond = 0;
        isPressing = false;
    }
}