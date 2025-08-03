using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Shader
{
    [ExecuteInEditMode]
    public class Saturation : MonoBehaviour {

        public Color color = new Color(1,1,1,1);
        [Range(0,1)]
        public float _Saturation = 0;

        private Image image;
        private Material material;
	    void Awake () {
		    if (TryGetComponent(out Image image)) {
                material = image.material;
		    }
	    }

	    void Update () {
            if(material)
            {
                material.SetColor ("_Color", color);
                material.SetFloat ("_Saturation", _Saturation);
            }
	    }
    }
}