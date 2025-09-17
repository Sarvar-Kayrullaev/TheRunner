using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MarkerPoint : MonoBehaviour
{
    public Vector3 offset = Vector3.zero;
    public GameObject SignalPrefab;

    [HideInInspector] public Transform target;

    private Image image;

    private Sprite mainSprite;
    private Color signalColor;
    private float signalStartTime;
    private float repeatRate;

    private float currentRepeatedCount;
    private float currentRepeatRateTime;

    private bool isSignalInitialized = false;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void Initialize(Sprite sprite)
    {
        mainSprite = sprite;
        Transform child = transform.GetChild(0);
        if (child)
            if (child.TryGetComponent(out Image image))
            {
                this.image = image;
                this.image.sprite = sprite;
            }
    }

    public void Initialize(Sprite sprite, Vector3 offset)
    {
        this.offset = offset;
        mainCamera = Camera.main;
        mainSprite = sprite;
        Transform child = transform.GetChild(0);
        if (child)
            if (child.TryGetComponent(out Image image))
            {
                this.image = image;
                this.image.sprite = sprite;
            }
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

        if (currentRepeatedCount > 0)
        {
            if (signalStartTime <= Time.time)
            {
                if (Time.time >= currentRepeatRateTime)
                {
                    currentRepeatedCount--;
                    currentRepeatRateTime = Time.time + 1f / repeatRate;
                    if (Instantiate(SignalPrefab, transform).TryGetComponent(out Image signalImage))
                    {
                        signalImage.color = signalColor;   
                    }
                    if (currentRepeatedCount <= 0)
                    {
                        StartCoroutine(nameof(ResetSignal),1);
                    }
                }
            }
        }
    }

    public void SetSignal(Sprite signalSprite, Color signalColor, float signalStartTime, int repeatCount, float repeatRate)
    {
        if(isSignalInitialized) return;
        isSignalInitialized = true;
        this.signalColor = signalColor;
        this.signalStartTime = Time.time + signalStartTime;
        this.repeatRate = repeatRate;

        currentRepeatedCount = repeatCount;
        image.sprite = signalSprite;

        if(repeatCount <= 0)
        {
            StartCoroutine(ResetSignal(1));
        }
    }

    private IEnumerator ResetSignal(float time)
    {
        yield return new WaitForSeconds(time);
        isSignalInitialized = false;
        image.sprite = mainSprite;
    }

    private void Activation(bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);
        }
    }
}
