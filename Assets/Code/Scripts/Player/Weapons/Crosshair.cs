using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class Crosshair : MonoBehaviour
{
    public float restingSize;
    public float shootSize;
    public float walkSize;
    public float runSize;
    public float aimAccuracyRate;

    public float transitionSpeed;

    public bool aiming;
    public bool shooting;
    [SerializeField] float shootEndTime;
    float currentShootEndTime;
    public bool walking;
    public bool running;


    [SerializeField] Transform top,bottom,right,left;

    [HideInInspector] public Weapon weapon;
    private RectTransform rectTransform;
    private float currentSize;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (running)
        {
            float size = aiming ? runSize / aimAccuracyRate: runSize;
            currentSize = Mathf.Lerp(currentSize, size, Time.deltaTime * transitionSpeed);
        }
        else if(walking)
        {
            float size = aiming ? walkSize / aimAccuracyRate : walkSize;
            currentSize = Mathf.Lerp(currentSize, size, Time.deltaTime * transitionSpeed);
        }
        else if (shooting)
        {
            float size = aiming ? shootSize / aimAccuracyRate : shootSize;
            currentSize = Mathf.Lerp(currentSize, size, Time.deltaTime * transitionSpeed);

            currentShootEndTime -= Time.deltaTime;
            if (currentShootEndTime <= 0)
            {
                shooting = false;
            }
        }
        else
        {
            float size = aiming ? restingSize / aimAccuracyRate : restingSize;
            currentSize = Mathf.Lerp(currentSize, size, Time.deltaTime * transitionSpeed);
        }



        if (weapon)
        {
            weapon.accuracy = currentSize;
        }
        rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }

    public void Shooting()
    {
        currentShootEndTime = shootEndTime;
        shooting = true;
    }
    public void Aiming(bool aiming)
    {
        SetActive(!aiming);
        this.aiming = aiming;
    }
    public void Running(bool running)
    {
        this.running = running;
    }
    public void Walking(bool walking)
    {
        this.walking = walking;
    }
    void SetActive(bool enabled)
    {
        foreach (Transform child in transform)
        {
            
            child.gameObject.SetActive(enabled);
        }
    }
}
