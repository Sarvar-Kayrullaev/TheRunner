using Data;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [Header("Required Objects")]
    [SerializeField] GameObject FPSDisplayer;
    [SerializeField] GameObject CloudDisplayer;
    [SerializeField] GameObject AtmosphericFogDisplayer;
    [SerializeField] Light SunLight;
    
    public void Awake()
    {
        ApplySettings();
    }

    private void ApplySettings()
    {
        SettingsModel settings = DataProvider.LoadSettingsData();
        SetShowFPS(settings.Options.ShowFPS);
        SetShowADS(settings.Options.ShowRewardedAdvertising);
        SetFramesPerSecondLimit(settings.GraphicOptions.FramesPerSecond);
        SetAntiAliasing(settings.GraphicOptions.AntiAliasing);
        SetTextureResolution(settings.GraphicOptions.TextureResolution);
        SetAnisotropicTextures(settings.GraphicOptions.AnisotropicTextures);
        SetShadow(settings.GraphicOptions.Shadow);
        SetShadowResolution(settings.GraphicOptions.ShadowResolution);
        SetShadowDistance(settings.GraphicOptions.ShadowDistance);
        SetLodBias(settings.GraphicOptions.LodBias);
        SetShowCloud(settings.GraphicOptions.Cloud);
        SetAtmosphericFOG(settings.GraphicOptions.FOG);
        SetLensFlare(settings.GraphicOptions.LensFlare);
        SetSunRays(settings.GraphicOptions.SunRays);
        //SetAmbienceOcclustion(settings.GraphicOptions.AmbienceOcclusion);
        //SetMotionBlur(settings.GraphicOptions.MotionBlur);
        //SetBloom(settings.GraphicOptions.Bloom);
    }

    private void SetShowFPS(bool value) => FPSDisplayer?.SetActive(value);
    private void SetShowADS(bool value) { }

    private void SetFramesPerSecondLimit(int value) => GameSettingsModifier.SetFramesPerSecond(value);
    private void SetAntiAliasing(int value) => GameSettingsModifier.SetAntiAliasing(value);
    private void SetTextureResolution(int value) => GameSettingsModifier.SetTextureResolution(value);
    private void SetAnisotropicTextures(int value) => GameSettingsModifier.SetAnisotropicTextures(value);
    private void SetShadow(int value) => GameSettingsModifier.SetShadow(value);
    private void SetShadowResolution(int value) => GameSettingsModifier.SetShadowResolution(value);
    private void SetShadowDistance(int value) => GameSettingsModifier.SetShadowDistance(value);
    private void SetLodBias(int value) => GameSettingsModifier.SetLodBias(value);
    private void SetShowCloud(bool value) => CloudDisplayer.SetActive(value);
    private void SetAtmosphericFOG(bool value) => AtmosphericFogDisplayer.SetActive(value);
    private void SetLensFlare(bool value) { if (SunLight.TryGetComponent(out LensFlare lensFlare)) lensFlare.enabled = value; }
    private void SetSunRays(bool value) { }
    //private void SetAmbienceOcclustion(bool value) { if (PostProcessVolume.profile.TryGetSettings(out AmbientOcclusion ao)) ao.active = value; }
    //private void SetMotionBlur(bool value) { if (PostProcessVolume.profile.TryGetSettings(out MotionBlur mb)) mb.active = value; }
    //private void SetBloom(bool value) { if (PostProcessVolume.profile.TryGetSettings(out Bloom bloom)) bloom.active = value; }
}
