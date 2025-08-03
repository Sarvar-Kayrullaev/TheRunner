using UnityEngine;
using UnityEngine.UI;

namespace Shader
{
    [ExecuteInEditMode]
    public class SniperFocus : MonoBehaviour
    {
        [Range(0,1)]
        public float Power = 0;
        [Range(0,1)]
        public float Saturation = 0;
        [Range(0,5)]
        public float Size = 1;
        [Range(0,5)]
        public float SmoothSize = 1;

        [Space]
        [SerializeField] float TransitionSpeed = 1;

        private Image image;
        private Material material;
        public bool isFocused = false;
	    void Awake () {
		    if (TryGetComponent(out Image image)) {
                material = image.material;
		    }
	    }

	    void Update () {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            float percentage = screenHeight/screenWidth;


            if(material)
            {
                if(isFocused)
                {
                    material.SetFloat ("_Power", Mathf.Lerp(material.GetFloat("_Power"), Power, Time.deltaTime * TransitionSpeed));
                    material.SetFloat ("_Saturation", Mathf.Lerp(material.GetFloat("_Saturation"), Saturation, Time.deltaTime * TransitionSpeed));
                    material.SetFloat ("_Size", Mathf.Lerp(material.GetFloat("_Size"), Size, Time.deltaTime * TransitionSpeed));
                    material.SetFloat ("_Smooth2", Mathf.Lerp(material.GetFloat("_Smooth2"), SmoothSize, Time.deltaTime * TransitionSpeed));
                }
                else
                {
                    material.SetFloat ("_Power", Mathf.Lerp(material.GetFloat("_Power"), 0, Time.deltaTime * TransitionSpeed));
                    material.SetFloat ("_Saturation", Mathf.Lerp(material.GetFloat("_Saturation"), 0, Time.deltaTime * TransitionSpeed));
                    material.SetFloat ("_Size", Mathf.Lerp(material.GetFloat("_Size"), 1, Time.deltaTime * TransitionSpeed));
                    material.SetFloat ("_Smooth2", Mathf.Lerp(material.GetFloat("_Smooth2"), 1, Time.deltaTime * TransitionSpeed));
                    if(material.GetFloat("_Power") <= 0.05f) gameObject.SetActive(false);
                }
            }
	    }

        public void SetFocus(bool value)
        {
            gameObject.SetActive(true);
            isFocused = value;
        }
    }
}

