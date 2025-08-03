using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ScreenPoint : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = Vector3.zero;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Initialize (Sprite sprite)
    {
        mainCamera = Camera.main;
        Transform child = transform.GetChild(0);
        if(child) if(child.TryGetComponent(out Image image)) image.sprite = sprite;
    }

    public void Initialize (Sprite sprite, Vector3 offset)
    {
        this.offset = offset;
        mainCamera = Camera.main;
        Transform child = transform.GetChild(0);
        if(child) if(child.TryGetComponent(out Image image)) image.sprite = sprite;
    }

    private void Update()
    {
        Vector2 pos = mainCamera.WorldToScreenPoint(target.position + offset);
        float dot = Vector3.Dot(target.position - mainCamera.transform.position, mainCamera.transform.forward);
        
        if (dot < 0)
        {
            Activation(false);
        }
        else
        {
            Activation(true);
        }

        transform.position = pos;
    }

    void Activation(bool value)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }
}
