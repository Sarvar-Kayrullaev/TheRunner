using System;
using System.Collections.Generic;
using Code.Scripts.Data.Language;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class SettingsModel
    {
        public OptionsModel Options = new();
        public AudioOptionsModel AudioOptions = new();
        public GraphicOptionsModel GraphicOptions = new();
        public ControllerOptionsModel ControllerOptions = new();
    }

    [Serializable]
    public class OptionsModel
    {
        public Language.LanguageCode Language;
        public bool ShowFPS;
        public bool ShowRewardedAdvertising;
    }

    [Serializable]
    public class AudioOptionsModel
    {
        public int MusicVolume;
        public int SoundEffectVolume;
        public bool InGameMusicOfRadio;
        public bool SoundOfNPC;
    }

    [Serializable]
    public class GraphicOptionsModel
    {
        public int FramesPerSecond;
        public int AntiAliasing;
        public int TextureResolution;
        public int AnisotropicTextures;
        public int Shadow;
        public int ShadowResolution;
        public int ShadowDistance;
        public int LodBias;
        public bool Cloud;
        public bool FOG;
        public bool LensFlare;
        public bool SunRays;
        public bool AmbienceOcclusion;
        public bool MotionBlur;
        public bool Bloom;
    }

    [Serializable]
    public class ControllerOptionsModel
    {

    }
}