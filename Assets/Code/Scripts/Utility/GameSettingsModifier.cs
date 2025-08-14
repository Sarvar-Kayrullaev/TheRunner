using UnityEngine;
using System;
using Code.Scripts.Data.Language;

public class GameSettingsModifier
{
    public static void SetLanguage(Language.LanguageCode value)
    {
        
    }
    public static void SetFramesPerSecond(int value)
    {
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        switch (value)
        {
            case 0:
                Application.targetFrameRate = 30; // 30 FPS
                break;
            case 1:
                Application.targetFrameRate = 60; // 60 FPS
                break;
            case 2:
                Application.targetFrameRate = 120; // 120 FPS
                break;
            case 3:
                Application.targetFrameRate = 1000; // Unlimited FPS
                break;
            default:
                Application.targetFrameRate = 1000; // Unlimited FPS
                break;
        }
        QualitySettings.vSyncCount = 0;
    }

    public static void SetAntiAliasing(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.antiAliasing = 0; // Disabled
                break;
            case 1:
                Application.targetFrameRate = 2; // 2X
                break;
            case 2:
                Application.targetFrameRate = 4; // 4X
                break;
            case 3:
                Application.targetFrameRate = 8; // 8X
                break;
            default:
                Application.targetFrameRate = 0; // Disabled
                break;
        }
    }

    public static void SetTextureResolution(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.globalTextureMipmapLimit = 3; // Eighth resolution
                break;
            case 1:
                QualitySettings.globalTextureMipmapLimit = 2; // Quarter resolution
                break;
            case 2:
                QualitySettings.globalTextureMipmapLimit = 1; // Half resolution
                break;
            case 3:
                QualitySettings.globalTextureMipmapLimit = 0; // Full resolution
                break;
            default:
                QualitySettings.globalTextureMipmapLimit = 0; // Full resolution
                break;
        }
    }

    public static void SetAnisotropicTextures(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable; // Disabled
                break;
            case 1:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable; // Enabled
                break;
            case 2:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable; // Force Enabled
                break;
            default:
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable; // Disabled
                break;
        }
    }

    public static void SetShadow(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.shadows = ShadowQuality.Disable; // Disabled
                break;
            case 1:
                QualitySettings.shadows = ShadowQuality.HardOnly; // Hard Shadow
                break;
            case 2:
                QualitySettings.shadows = ShadowQuality.All; // Hard And Soft Shadow
                break;
            default:
                QualitySettings.shadows = ShadowQuality.All; // Hard And Soft Shadow
                break;
        }
    }

    public static void SetShadowResolution(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.shadowResolution = ShadowResolution.Low; // Low
                break;
            case 1:
                QualitySettings.shadowResolution = ShadowResolution.Medium;  // Medium
                break;
            case 2:
                QualitySettings.shadowResolution = ShadowResolution.High;  // Hight
                break;
            case 3:
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;  // Ultra
                break;
            default:
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;  // Ultra
                break;
        }
    }

    public static void SetShadowDistance(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.shadowDistance = 25;
                break;
            case 1:
                QualitySettings.shadowDistance = 50;
                break;
            case 2:
                QualitySettings.shadowDistance = 100;
                break;
            case 3:
                QualitySettings.shadowDistance = 150;
                break;
            case 4:
                QualitySettings.shadowDistance = 200;
                break;
            default:
                QualitySettings.shadowDistance = 50;
                break;
        }
    }

    public static void SetLodBias(int value)
    {
        switch (value)
        {
            case 0:
                QualitySettings.lodBias = 0.5f;
                break;
            case 1:
                QualitySettings.lodBias = 1;
                break;
            case 2:
                QualitySettings.lodBias = 2;
                break;
            case 3:
                QualitySettings.lodBias = 3;
                break;
            case 4:
                QualitySettings.lodBias = 4;
                break;
            default:
                QualitySettings.lodBias = 1;
                break;
        }
    }
}
