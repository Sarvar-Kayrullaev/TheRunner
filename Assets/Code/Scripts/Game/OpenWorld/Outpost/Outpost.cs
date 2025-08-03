using System;
using BotRoot;
using Data;
using PlayerRoot;
using UnityEngine;

public class Outpost : MonoBehaviour
{
    public String Name;
    public bool Destroyed;

    [Header("Params")]
    public float CullingDistance;

    [Header("Objects")]
    public Transform indicatorPosition;

    [Header("Setup")]
    public OverallController overallController;

    [HideInInspector] public GameObject InitializedIndicator;

    private OutpostManager manager;
    private Player player;
    private bool initialized = false;

    [Obsolete]
    public void Initialize(OutpostManager manager, OutpostModel outpostModel)
    {
        this.manager = manager;
        this.player = manager.player;
        Name = outpostModel.Name;
        Destroyed = outpostModel.Destroyed;

        if (Destroyed)
        {
            overallController.IsSpawnable = false;
        }
        else
        {
            overallController.IsSpawnable = true;
        }
        initialized = true;
    }

    void OnEnable()
    {
        InvokeRepeating(nameof(UpdateOutpost), 0, 1);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(UpdateOutpost));
    }

    public void UpdateOutpost()
    {
        if (!initialized) return;
        if (Destroyed)
        {

        }
        else
        {
            float playerDistance = Vector3.Distance(player.transform.position, indicatorPosition.position);
            if (playerDistance > CullingDistance)
            {
                overallController.Unspawn();
            }
            else
            {
                overallController.Initialize();
            }

            if (overallController.authors.Count <= 0)
            {
                if (playerDistance < CullingDistance)
                {
                    SetDestroyed();
                    CancelInvoke(nameof(UpdateOutpost));
                }
            }
        }
    }

    public void SetDestroyed()
    {
        manager.SetDestroyOutpost(this);
    }

}
