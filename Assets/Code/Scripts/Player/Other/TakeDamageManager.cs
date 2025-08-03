using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakeDamageManager : MonoBehaviour
{
    [SerializeField] Image BloodFrame;
    public float BackTimerSpeed;
    public float BloodProgress;

    [Header("Hit Sound Options")]
    public float SoundVolume;
    public AudioClip[] DamageTakenSounds;
    public AudioSource audioSource;
    public float RateStabilityTime = 0.8f;

    private Color bloodFrameColor;
    private float currentRateStabilityTime;
    void Start()
    {
        bloodFrameColor = BloodFrame.color;
        currentRateStabilityTime = 0;
    }

    void Update()
    {
        if (BloodProgress <= 0) return;
        BloodProgress = Mathf.Lerp(BloodProgress, 0, BackTimerSpeed);
        BloodFrame.color = new Color(bloodFrameColor.r, bloodFrameColor.g, bloodFrameColor.b, BloodProgress);
    }

    public void DamageTaken()
    {
        BloodProgress = 1;

        if (currentRateStabilityTime < Time.time)
        {
            currentRateStabilityTime = Time.time+RateStabilityTime;
            int randomSelect = Random.Range(0, DamageTakenSounds.Length);
            audioSource.PlayOneShot(DamageTakenSounds[randomSelect], SoundVolume);
        }
    }

    public void FallingDamageTaken()
    {
        BloodProgress = 1;

        if (currentRateStabilityTime < Time.time)
        {
            currentRateStabilityTime = Time.time+RateStabilityTime;
            int randomSelect = Random.Range(0, DamageTakenSounds.Length);
            audioSource.PlayOneShot(DamageTakenSounds[randomSelect], SoundVolume);
        }
    }
}
