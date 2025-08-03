using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Shader
{
    [ExecuteInEditMode]
    public class Blur : MonoBehaviour {

        public Color color = new Color(1,1,1,1);
        [Range(0,10)]
	    public float Distortion = 0;
        [Range(0,1)]
        public float Saturation = 0;
        [Range(0,2)]
        public float Bloom = 1;

	// Use this for initialization
        private Image image;
        private Material material;
	    void Awake () {
		    if (TryGetComponent(out Image image)) {
                material = image.material;
		    }
	    }

	// Update is called once per frame
	    void Update () {
            if(material)
            {
                material.SetColor ("_Color", color);
		        material.SetFloat ("_Distortion", Distortion);
                material.SetFloat ("_Saturation", Saturation);
                material.SetFloat ("_Bloom", Bloom);
            }
	    }

        void OnDisable()
        {
            if(material)
            {
                material.SetColor ("_Color", Color.white);
		        material.SetFloat ("_Distortion", 0);
                material.SetFloat ("_Saturation", 0);
                material.SetFloat ("_Bloom", 1f);
            }
        }
    }
}