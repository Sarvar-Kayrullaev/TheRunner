using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastManager : MonoBehaviour
{
    public GameObject pickupToastPrefab;
    public Transform pickupToastParent;

    public Sprite moneySprite;
    public Sprite goldSprite;
    public Sprite bulletSprite;

    // Keeps track of total stacked toast time
    private float _stackTime;

    // Time when the last group of toasts was played
    private float _lastPlayTime;

    public void PlayPickupToasts(List<PickupToast> toasts, float duration)
    {
        // Time passed since the last toast group played
        var elapsed = Time.time - _lastPlayTime;

        // If too much time passed, reset the stack timing
        if (elapsed > _stackTime)
        {
            _stackTime = 0;
            elapsed = 0;
        }

        // Calculate how long to wait before playing the next toast group
        // (if previous ones are still showing)
        var delay = Mathf.Max(_stackTime - elapsed, 0);

        // Extend the stack time to include all upcoming toasts
        // Subtract elapsed to keep timing consistent between calls
        _stackTime += duration * toasts.Count - elapsed;

        // Remember when we last played toasts
        _lastPlayTime = Time.time;

        // Schedule each toast to play in sequence
        foreach (var toast in toasts)
        {
            StartCoroutine(PickupToastPlayer(toast, delay, duration));
            delay += duration; // Add delay for next toast
        }
    }

    private IEnumerator PickupToastPlayer(PickupToast toast, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        toast.Play(duration);
    }
}