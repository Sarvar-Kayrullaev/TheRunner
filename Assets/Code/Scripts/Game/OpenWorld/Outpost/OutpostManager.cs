using System.Collections.Generic;
using BotRoot;
using Data;
using PlayerRoot;
using UnityEngine;

public class OutpostManager : MonoBehaviour
{
    [Header("Marker Params")]
    [SerializeField] float minDrawDistance = 100;
    [SerializeField] float maxDrawDistance = 500;

    [Header("Setup")]
    [SerializeField] Transform OutpostParent;
    [HideInInspector] public List<Outpost> Outposts;

    [HideInInspector] public IndicatorManager indicatorManager;
    [HideInInspector] public Player player;
    [HideInInspector] public DataManager dataManager;
    [HideInInspector] public List<OutpostModel> outpostModels;

    void Start()
    {

        dataManager = FindFirstObjectByType<DataManager>();
        indicatorManager = FindFirstObjectByType<IndicatorManager>();
        player = FindFirstObjectByType<Player>();
        InvokeRepeating(nameof(UpdateIndicators), 1, 2);

        UpdateOutpost();
    }

    void UpdateOutpost()
    {
        outpostModels = dataManager.openWorldModel.outposts;
        int index = 0;
        foreach (Transform child in OutpostParent)
        {
            if (child.TryGetComponent(out Outpost outpostComponent))
            {
                Outposts.Add(outpostComponent);
                outpostComponent.Initialize(this, outpostModels[index]);
                index++;
            }
        }
    }

    public void SetDestroyOutpost(Outpost outpost)
    {
        int index = outpost.transform.GetSiblingIndex();
        OpenWorldModel openWorldModel = dataManager.openWorldModel;
        openWorldModel.outposts[index].Destroyed = true;
        DataProvider.SaveOpenWorldData(openWorldModel);
        UpdateOutpost();
    }

    void UpdateIndicators()
    {
        foreach (Outpost outpost in Outposts)
        {
            bool destroyed = outpost.Destroyed;
            bool Initialized = outpost.InitializedIndicator != null;

            if (!destroyed)
            {
                float playerDistance = Vector3.Distance(player.transform.position, outpost.indicatorPosition.position);
                bool InRange = playerDistance > minDrawDistance && playerDistance < maxDrawDistance;

                if (InRange)
                {
                    if (Initialized) return;

                    GameObject indicator = Instantiate(indicatorManager.OutpostIndicatorPrefab, indicatorManager.IndicatorParents[0]);
                    outpost.InitializedIndicator = indicator;

                    if (indicator.TryGetComponent(out ObjectiveIndicator objectiveIndicator))
                    {
                        objectiveIndicator.Initialize(outpost.indicatorPosition, indicatorManager);
                    }
                    else
                    {
                        Debug.LogWarning("<ObjectiveIndicator> component is not found!");
                    }
                }
                else
                {
                    Destroy(outpost.InitializedIndicator);
                }
            }
        }
    }
}