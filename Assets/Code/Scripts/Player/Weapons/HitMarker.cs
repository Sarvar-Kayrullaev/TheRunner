using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class HitMarker : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float minSize;
    [SerializeField] private float maxSize;
    [SerializeField] private float lifeTime;
    [SerializeField] private Color hitColor;
    [SerializeField] private Color diedColor;
    [SerializeField] private AudioClip[] hitSound;
    [SerializeField] private AudioClip[] diedSound;
    [SerializeField] private float soundVolume;

    private AudioSource _audio;
    private RectTransform _rectTransform;
    private float _currentSize;
    private bool _died;
    private float _getSizeDelta;
    private readonly List<Image> _childrenImages = new List<Image>();
    private IEnumerator _coroutine;

    private void Awake()
    {
        SetActive(false);
        _rectTransform = GetComponent<RectTransform>();
        _audio = GetComponent<AudioSource>();
        _coroutine = Disable();
        foreach (var componentsInChild in transform.GetComponentsInChildren<Image>())
        {
            _childrenImages.Add(componentsInChild);
        }
    }

    private void Update()
    {
        if (_died)
        {
            _currentSize = Mathf.Lerp(_currentSize, minSize * 1.5f, Time.deltaTime * speed);
        }
        else
        {
            _currentSize = Mathf.Lerp(_currentSize, minSize, Time.deltaTime * speed);
        }

        _rectTransform.sizeDelta = new Vector2(_currentSize, _currentSize);
    }

    public void Hit()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        var selectHitSound = Random.Range(0, hitSound.Length);
        _audio.PlayOneShot(hitSound[selectHitSound],soundVolume);
        _currentSize = maxSize;
        ChangeColor(hitColor);
        SetActive(true);
        StartCoroutine(_coroutine);
        _died = false;
    }
    public void Died()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        var selectDiedSound = Random.Range(0, diedSound.Length);
        _audio.PlayOneShot(diedSound[selectDiedSound],soundVolume);
        ChangeColor(diedColor);
        _currentSize = maxSize;
        SetActive(true);
        StartCoroutine(_coroutine);
        _died = true;
    }

    private IEnumerator Disable()
    {
        while (true)
        {
            yield return new WaitForSeconds(lifeTime);
            SetActive(false);
        }
    }

    private void SetActive(bool enabled)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(enabled);
        }
    }

    private void ChangeColor(Color color)
    {
        foreach(var image in _childrenImages)
        {
            image.color = color;
        }
    }
}
