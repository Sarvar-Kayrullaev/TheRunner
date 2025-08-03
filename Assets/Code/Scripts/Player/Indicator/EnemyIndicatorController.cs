using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyIndicatorController : MonoBehaviour
{
    public Image progressBackground;
    public Image progressForeground;

    [Range(0, 30)]
    private float angle = 0;
    private float maxAngle = 30;
    [Range(0, 100)]
    public float progress;

    [Range(0, 100)]
    public float backgroundProgress;
    float backgroundAngle;

    public Gradient colorOfDanger;

    void Awake()
    {
        progressBackground.fillClockwise = true;
        progressForeground.fillClockwise = true;
        progressBackground.fillOrigin = 2;
        progressForeground.fillOrigin = 2;
        SetAngleValue(progressBackground, 30, 360);
        SetAngleValue(progressForeground, 0, 360);
    }

    void Update()
    {
        angle = (maxAngle/100)*progress;
        progressForeground.fillAmount = (angle / 360) * 1;
        progressForeground.transform.localEulerAngles = new Vector3(0, 0, angle / 2);
        progressBackground.color = colorOfDanger.Evaluate((angle / maxAngle) * 1);

        backgroundAngle = (maxAngle / 100) * backgroundProgress;
        progressBackground.fillAmount = (backgroundAngle / 360) * 1;
        progressBackground.transform.localEulerAngles = new Vector3(0, 0, backgroundAngle / 2);
    }
    void SetAngleValue(Image image, float value, float maxValue)
    {
        image.fillAmount = (value / maxValue) * 1;
        image.transform.localEulerAngles = new Vector3(0, 0, value / 2);
    }
}