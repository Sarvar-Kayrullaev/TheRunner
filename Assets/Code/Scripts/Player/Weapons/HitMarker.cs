using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class HitMarker : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float minSize;
    [SerializeField] float maxSize;
    [SerializeField] float lifeTime;
    [SerializeField] Color hitColor;
    [SerializeField] Color diedColor;
    [SerializeField] AudioClip[] hitSound;
    [SerializeField] AudioClip[] diedSound;
    [SerializeField] float soundVolume;

    private AudioSource audio;
    private RectTransform rectTransform;
    private float currentSize;
    private bool died;
    private float getSizeDelta;

    void Awake()
    {
        SetActive(false);
        rectTransform = GetComponent<RectTransform>();
        audio = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (died)
        {
            currentSize = Mathf.Lerp(currentSize, minSize * 1.5f, Time.deltaTime * speed);
        }
        else
        {
            currentSize = Mathf.Lerp(currentSize, minSize, Time.deltaTime * speed);
        }

        rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
    }

    public void Hit()
    {
        CancelInvoke("Disable");
        int selectHitSound = Random.Range(0, hitSound.Length);
        audio.PlayOneShot(hitSound[selectHitSound],soundVolume);
        currentSize = maxSize;
        ChangeColor(hitColor);
        SetActive(true);
        Invoke("Disable", lifeTime);
        died = false;
    }
    public void Died()
    {
        CancelInvoke("Disable");
        int selectDiedSound = Random.Range(0, diedSound.Length);
        audio.PlayOneShot(diedSound[selectDiedSound],soundVolume);
        ChangeColor(diedColor);
        currentSize = maxSize;
        SetActive(true);
        Invoke("Disable", lifeTime);
        died = true;
    }
    void Disable()
    {
        SetActive(false);
    }
    void SetActive(bool enabled)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(enabled);
        }
    }

    void ChangeColor(Color color)
    {
        foreach(Transform child in transform)
        {
            child.GetComponent<Image>().color = color;
        }
    }
}
