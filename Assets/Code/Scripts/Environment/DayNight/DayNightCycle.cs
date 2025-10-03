using System;
using AtmosphericHeightFog;
using UnityEngine;

namespace Code.Scripts.Environment.DayNight
{
    public class DayNightCicle : MonoBehaviour
    {
        private static readonly int CloudColorKey = UnityEngine.Shader.PropertyToID("_color");
        private static readonly int DarknessThresholdKey = UnityEngine.Shader.PropertyToID("_darknessThreshold");
        private static readonly int SunDirectionBlendKey = UnityEngine.Shader.PropertyToID("_sunDirectionBlend");
        private static readonly int DensityScaleKey = UnityEngine.Shader.PropertyToID("_densityScale");
        private static readonly int BaseColorKey = UnityEngine.Shader.PropertyToID("_Alpha");

        // Public variables to control the speed and objects
        [Header("Options")]
        public float cycleDuration = 600f;
        [Range(0,1)] public float startPeriod = 0.25f;
        [Header("Setup")]
        public Light sun;
        public ParticleSystem starParticles;
        public Material starMaterial;
        public Material skyMaterial;
        public HeightFogGlobal heightFog;
        public Material cloudMaterial;

        [Header("AmbientColor")] 
        public Gradient sunColor;
        public AnimationCurve lightIntensityCurve;
        // These variables will appear in the Inspector to let you pick colors
        public Gradient skyColor;
        public Gradient equatorColor;
        public Gradient groundColor;
        public Gradient fogColor;
        public Gradient heightFogColor;
        public Gradient heightFogDirectionColor;
        public AnimationCurve heightSkyboxFogFillCurve;
        public Gradient cloudColor;
        public AnimationCurve cloudDarknessCurve;
        public AnimationCurve cloudSunDirectionBlendCurve;
        public AnimationCurve cloudDensityScaleCurve;
        public AnimationCurve starOpacityCurve;

        private Transform _sunTransform;
        private float _timeOfDay = 0f;
        private void Start()
        {
            _sunTransform = sun.transform;
            _timeOfDay = startPeriod;
        }

        private void Update()
        {
            _timeOfDay += Time.deltaTime / cycleDuration;
            if (_timeOfDay >= 1f)
            {
                _timeOfDay = 0f; // Reset the cycle
            }

            // Rotate the sun
            var sunRotation = _timeOfDay * 360f;
            _sunTransform.rotation = Quaternion.Euler(new Vector3(sunRotation, 200, -26));
            
            // Adjust light intensity
            sun.intensity = lightIntensityCurve.Evaluate(_timeOfDay);
            sun.color = sunColor.Evaluate(_timeOfDay);

            // Adjust ambient lighting
            RenderSettings.ambientSkyColor = skyColor.Evaluate(_timeOfDay);
            RenderSettings.ambientEquatorColor = equatorColor.Evaluate(_timeOfDay);
            RenderSettings.ambientGroundColor = groundColor.Evaluate(_timeOfDay);
            RenderSettings.fogColor = fogColor.Evaluate(_timeOfDay);
            heightFog.fogColorStart =  heightFogColor.Evaluate(_timeOfDay);
            heightFog.directionalColor = heightFogDirectionColor.Evaluate(_timeOfDay);
            heightFog.skyboxFogFill = heightSkyboxFogFillCurve.Evaluate(_timeOfDay);
            
            // Adjust Cloud
            cloudMaterial.SetColor(CloudColorKey, cloudColor.Evaluate(_timeOfDay));
            cloudMaterial.SetFloat(DarknessThresholdKey, cloudDarknessCurve.Evaluate(_timeOfDay));
            cloudMaterial.SetFloat(SunDirectionBlendKey, cloudSunDirectionBlendCurve.Evaluate(_timeOfDay));
            cloudMaterial.SetFloat(DensityScaleKey, cloudDensityScaleCurve.Evaluate(_timeOfDay));
            
            // Adjust Star
            starMaterial.SetFloat(BaseColorKey,starOpacityCurve.Evaluate(_timeOfDay));
        }
    }
}
