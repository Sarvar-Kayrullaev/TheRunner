using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    public List<Transform> IndicatorParents;
    [Header("IndicatorTypes")]
    public GameObject ObjectiveIndicatorPrefab;
    public GameObject OutpostIndicatorPrefab;
    public GameObject ItemIndicatorPrefab;
    public GameObject HumanIndicatorPrefab;

    void Awake()
    {

    }

    public void ResetCamera(Camera camera)
    {
        foreach (Transform parent in IndicatorParents)
        {
            foreach (Transform indicator in parent)
            {
                if (indicator.TryGetComponent(out ObjectiveIndicator objectiveIndicator)) objectiveIndicator.ResetCamera(camera);
            }
        }
    }

}
