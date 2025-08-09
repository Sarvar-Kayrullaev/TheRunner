using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[ExecuteAlways]
public class GraphicInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textFPS;

    public int Granularity = 5; // how many frames to wait until you re-calculate the FPS

    public void Start()
    {
        Application.targetFrameRate = 240;
        QualitySettings.vSyncCount = 0;
        Invoke(nameof(TargetFrame), 3f);
    }

    void TargetFrame()
    {
        Application.targetFrameRate = 240;
        QualitySettings.vSyncCount = 0;
    }

    // The number of frames to average.
    private const int frameRange = 60;

    // Use a Queue to store the last 'frameRange' deltaTimes.
    private Queue<float> frameTimes = new(frameRange);

    // A variable to hold the calculated average FPS.
    private float averageFps;

    // You can expose this property to other scripts if needed.
    private float AverageFps => averageFps;

    private void Awake()
    {
        // Initialize the queue.
        frameTimes = new Queue<float>(frameRange);
    }

    private void Update()
    {
        // Enqueue the current frame's deltaTime.
        frameTimes.Enqueue(Time.deltaTime);

        // If the queue size exceeds the frameRange, dequeue the oldest value.
        if (frameTimes.Count > frameRange)
        {
            frameTimes.Dequeue();
        }

        // Calculate the total time for all frames in the queue.
        float totalTime = 0f;
        foreach (float deltaTime in frameTimes)
        {
            totalTime += deltaTime;
        }

        // Calculate the average FPS.
        // FPS = number of frames / total time for those frames.
        averageFps = frameTimes.Count / totalTime;
        int FPS = (int)averageFps;

        textFPS.text = "FPS: "+ FPS;
    }
}
