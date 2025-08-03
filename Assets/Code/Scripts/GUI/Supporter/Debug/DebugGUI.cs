using UnityEngine;

public class DebugGUI: MonoBehaviour
{
    public void Signal()
    {
        Debug.Log("Signal: " + Random.Range(0, 100));
    }
}
