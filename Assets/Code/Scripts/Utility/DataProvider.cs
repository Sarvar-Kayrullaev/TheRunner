using UnityEngine;
using System;
using Code.Scripts.Data.Language;
using Data;

public static class DataProvider
{
    static readonly string SettingsFile = "Settings.json";
    static readonly string PlayerFile = "Player.json";
    static readonly string OpenWorldFile = "OpenWorld.json";

    public static void SaveSettingsData(SettingsModel settingsModel)
    {
        FileHandler.SaveToJSON(settingsModel, SettingsFile);
    }

    public static void SavePlayerData(PlayerModel playerModel)
    {
        FileHandler.SaveToJSON(playerModel, PlayerFile);
    }

    public static void SaveOpenWorldData(OpenWorldModel openWorldModel)
    {
        FileHandler.SaveToJSON(openWorldModel, OpenWorldFile);
    }
    public static SettingsModel LoadSettingsData()
    {
        if (FileHandler.Exists(SettingsFile))
        {
            return FileHandler.ReadFromJSON<SettingsModel>(SettingsFile);
        }
        else
        {
            SettingsModel settings = new SettingsModel();
            settings.Options.Language = Language.LanguageCode.English;
            settings.Options.ShowFPS = true;
            settings.Options.ShowRewardedAdvertising = true;
            settings.AudioOptions.MusicVolume = 10;
            settings.AudioOptions.SoundEffectVolume = 10;
            settings.AudioOptions.InGameMusicOfRadio = true;
            settings.AudioOptions.SoundOfNPC = true;
            settings.GraphicOptions.FramesPerSecond = 3;
            settings.GraphicOptions.AntiAliasing = 1;
            settings.GraphicOptions.TextureResolution = 1;
            settings.GraphicOptions.AnisotropicTextures = 0;
            settings.GraphicOptions.Shadow = 2;
            settings.GraphicOptions.ShadowResolution = 3;
            settings.GraphicOptions.ShadowDistance = 2;
            settings.GraphicOptions.LodBias = 1;
            settings.GraphicOptions.Cloud = true;
            settings.GraphicOptions.FOG = true;
            settings.GraphicOptions.LensFlare = true;
            settings.GraphicOptions.SunRays = true;
            settings.GraphicOptions.AmbienceOcclusion = true;
            settings.GraphicOptions.MotionBlur = false;
            settings.GraphicOptions.Bloom = false;

            SaveSettingsData(settings);
            return settings;
        }
    }

    public static PlayerModel LoadPlayerData()
    {
        if (FileHandler.Exists(PlayerFile))
        {
            return FileHandler.ReadFromJSON<PlayerModel>(PlayerFile);
        }
        else
        {
            SavePlayerData(new PlayerModel());
            return new PlayerModel();
        }
    }

    public static OpenWorldModel LoadOpenWorldData(OpenWorldModel startData)
    {
        if (FileHandler.Exists(OpenWorldFile))
        {
            return FileHandler.ReadFromJSON<OpenWorldModel>(OpenWorldFile);
        }
        else
        {
            SaveOpenWorldData(startData);
            return startData;
        }
    }
}
