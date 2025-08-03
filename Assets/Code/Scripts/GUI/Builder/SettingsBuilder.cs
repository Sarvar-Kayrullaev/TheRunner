using Data;
using UnityEngine;
using UnityEngine.UI;
using Widget;

public class SettingsBuilder : MonoBehaviour
{
    [Space][Header("Setup GUI")]
    public ElectorBar TargetFrameRate;
    public ElectorBar AntiAliasing;
    public ElectorBar TextureResolution;
    public ElectorBar AnisotropicTexture;
    public ElectorBar Shadow;
    public ElectorBar ShadowResolution;
    public ElectorBar ShadowDistance;
    public ElectorBar LodBias;
    public ToggleButton Cloud;
    public ToggleButton FOG;
    public ToggleButton LensFlare;
    public ToggleButton SunRays;
    public ToggleButton AmbienceOcclusion;
    public ToggleButton MotionBlur;
    public ToggleButton Bloom;

    [Space]
    public Slider MusicVolume;
    public Slider SoundVolume;

    [Space]
    public ToggleButton ShowFPS;
    public ToggleButton ShowRewardedAdvertising;


    public void Awake()
    {
        SettingsModel model = DataProvider.LoadSettingsData();

        if(TargetFrameRate)             TargetFrameRate.            Build(model.GraphicOptions.FramesPerSecond);
        if(AntiAliasing)                AntiAliasing.               Build(model.GraphicOptions.AntiAliasing);
        if(TextureResolution)           TextureResolution.          Build(model.GraphicOptions.TextureResolution);
        if(AnisotropicTexture)          AnisotropicTexture.         Build(model.GraphicOptions.AnisotropicTextures);
        if(Shadow)                      Shadow.                     Build(model.GraphicOptions.Shadow);
        if(ShadowResolution)            ShadowResolution.           Build(model.GraphicOptions.ShadowResolution);
        if(ShadowDistance)              ShadowDistance.             Build(model.GraphicOptions.ShadowDistance);
        if(LodBias)                     LodBias.                    Build(model.GraphicOptions.LodBias);
        if(Cloud)                       Cloud.                      Build(model.GraphicOptions.Cloud);
        if(FOG)                         FOG.                        Build(model.GraphicOptions.FOG);
        if(LensFlare)                   LensFlare.                  Build(model.GraphicOptions.LensFlare);
        if(SunRays)                     SunRays.                    Build(model.GraphicOptions.SunRays);
        if(AmbienceOcclusion)           AmbienceOcclusion.          Build(model.GraphicOptions.AmbienceOcclusion);
        if(MotionBlur)                  MotionBlur.                 Build(model.GraphicOptions.MotionBlur);
        if(Bloom)                       Bloom.                      Build(model.GraphicOptions.Bloom);

        if(MusicVolume)                 MusicVolume.value =         model.AudioOptions.MusicVolume;
        if(SoundVolume)                 SoundVolume.value =         model.AudioOptions.SoundEffectVolume;

        if(ShowFPS)                     ShowFPS.                    Build(model.Options.ShowFPS);
        if(ShowRewardedAdvertising)     ShowRewardedAdvertising.    Build(model.Options.ShowRewardedAdvertising);
    }
}

