using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[ExecuteAlways]
public class GraphicInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textFPS;

    public int Granularity = 5; // how many frames to wait until you re-calculate the FPS
    List<double> times;
    int counter = 5;

    public void Start()
    {
        times = new List<double>();
        Application.targetFrameRate = 240;
        QualitySettings.vSyncCount = 0;
        Invoke("TargetFrame",3f);
    }

    void TargetFrame()
    {
        Application.targetFrameRate = 240;
        QualitySettings.vSyncCount = 0;
    }

    public void Update()
    {
        if (counter <= 0)
        {
            CalcFPS();
            counter = Granularity;
        }

        times.Add(Time.deltaTime);
        counter--;
    }

    public void CalcFPS()
    {
        double sum = 0;
        foreach (double F in times)
        {
            sum += F;
        }

        double average = sum / times.Count;
        double fps = 1 / average;
        int FPS = (int)fps;
        textFPS.text = "FPS: " + FPS;
        // update a GUIText or something
    }
}
