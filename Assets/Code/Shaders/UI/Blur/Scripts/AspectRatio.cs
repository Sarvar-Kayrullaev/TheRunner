using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Shader
{
    [ExecuteInEditMode]
    public class SetBlurUI : MonoBehaviour
    {
        [Range(0,1)]
        public float CornerRadius;
        private Material material;
        public void Awake()
        {
            if (TryGetComponent(out Image image))
            {
                material = image.material;
                RectTransform rect = transform as RectTransform;
                float aspectRatioX = Mathf.Clamp01(rect.rect.size.y / rect.rect.size.x);
                float aspectRatioY = Mathf.Clamp01(rect.rect.size.x / rect.rect.size.y);
                Vector4 cornerRadius = new(CornerRadius * aspectRatioX,CornerRadius * aspectRatioY);
                image.material.SetVector("_CornerRadius", cornerRadius);
            }else
            {
                Debug.LogWarning("Material Not Found");
            }
        }
        void Update()
        {
            if (material)
            {
                RectTransform rect = transform as RectTransform;
                float aspectRatioX = Mathf.Clamp01(rect.rect.size.y / rect.rect.size.x);
                float aspectRatioY = Mathf.Clamp01(rect.rect.size.x / rect.rect.size.y);
                Vector4 cornerRadius = new(CornerRadius * aspectRatioX,CornerRadius * aspectRatioY);
                material.SetVector("_CornerRadius", cornerRadius);
            }
            else
            {
                Debug.LogWarning("Material Not Found");
            }
        }

        public void SetColor(Color color)
        {
            if(material)
            {
                material.SetColor("_Color", color);
            }
            else
            {
                Debug.LogWarning("Material Not Found");
            }
        }
    }
}