using Code.Scripts.Data.Language;
using Data;
using UnityEngine;

public class SettingsDataModifier : MonoBehaviour
{

    #region Options

    public void Set_Language(Language.LanguageCode value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.Options.Language = value;
        DataProvider.SaveSettingsData(model);
    }
    public void Set_ShowFPS(bool value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.Options.ShowFPS = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_ShowRewardedADS(bool value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.Options.ShowRewardedAdvertising = value;
        DataProvider.SaveSettingsData(model);
    }
    #endregion

    #region AudioOptions
    public void Set_MusicVolume(float value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.AudioOptions.MusicVolume = (int)value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_SoundVolume(float value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.AudioOptions.SoundEffectVolume = (int)value;
        DataProvider.SaveSettingsData(model);
    }
    #endregion

    #region GraphicOptions
    public void Set_FramesPerSecond(int value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.FramesPerSecond = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_AntiAliasing(int value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.AntiAliasing = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_TextureResolution(int value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.TextureResolution = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_AnisotropicTextures(int value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.AnisotropicTextures = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_Shadow(int value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.Shadow = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_ShadowResolution(int value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.ShadowResolution = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_ShadowDistance(int value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.ShadowDistance = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_LodBias(int value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.LodBias = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_Cloud(bool value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.Cloud = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_FOG(bool value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.FOG = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_LensFlare(bool value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.LensFlare = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_SunRays(bool value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.SunRays = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_AmbienceOcclusion(bool value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.AmbienceOcclusion = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_MotionBlur(bool value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.MotionBlur = value;
        DataProvider.SaveSettingsData(model);
    }

    public void Set_Bloom(bool value)
    {
        SettingsModel model = DataProvider.LoadSettingsData();
        model.GraphicOptions.Bloom = value;
        DataProvider.SaveSettingsData(model);
    }
    #endregion
}
