using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class DataManager : MonoBehaviour
{
    [HideInInspector] public SettingsModel settingsModel;
    [HideInInspector] public PlayerModel playerModel;
    [HideInInspector] public OpenWorldModel openWorldModel;
    
    [HideInInspector] public bool LoadingCompleted = false;

    private StartData startData;
    public void Awake()
    {
        startData = FindFirstObjectByType<StartData>();
        StartCoroutine(LoadData());
    }

    IEnumerator LoadData()
    {
        settingsModel = DataProvider.LoadSettingsData();
        playerModel = DataProvider.LoadPlayerData(startData.PlayerData);
        openWorldModel = DataProvider.LoadOpenWorldData(startData.OpenWorldData);

        while (settingsModel == null|| playerModel == null|| openWorldModel == null)
        {
            yield return null;
        }
        
        LoadingCompleted = true;
    }

    public void UpdatePlayerModel(PlayerModel _playerModel)
    {
        playerModel = _playerModel;
        DataProvider.SavePlayerData(_playerModel);
    }

    public void UpdateOpenWorldModel(OpenWorldModel _openWorldModel)
    {
        openWorldModel = _openWorldModel;
        DataProvider.SaveOpenWorldData(openWorldModel);
    }

    public void UpdateSettingsModel(SettingsModel _settingsModel)
    {
        settingsModel = _settingsModel;
        DataProvider.SaveSettingsData(settingsModel);
    }
}
