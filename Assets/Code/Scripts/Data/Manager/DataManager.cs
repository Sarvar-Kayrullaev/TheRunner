using UnityEngine;
using Data;

public class DataManager : MonoBehaviour
{
    [HideInInspector] public SettingsModel settingsModel;
    [HideInInspector] public PlayerModel playerModel;
    [HideInInspector] public OpenWorldModel openWorldModel;

    private StartData startData;
    public void Awake()
    {
        startData = FindFirstObjectByType<StartData>();
        settingsModel = DataProvider.LoadSettingsData();
        playerModel = DataProvider.LoadPlayerData();


        openWorldModel = DataProvider.LoadOpenWorldData(startData.OpenWorldData);
    }
}
