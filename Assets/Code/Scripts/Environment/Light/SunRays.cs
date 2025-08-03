using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class SunRays : MonoBehaviour
    {
        public GameObject rayReference;
        public float rayThickness = 0.5f;
        public float rayLength = 5.0f;
        public int rayCount = 1;
        public float rayRadius = 0.5f;
        public float RotatePlaneOffset;
        public float CenterOffset = 0;

        private Transform sun;
        private new Transform camera;
        void Start()
        {
            sun = RenderSettings.sun.transform;
            camera = Camera.main.transform;
            GenerateSunRays();
        }

        void Update()
        {
            UpdateSunRays();
        }

        void GenerateSunRays()
        {
            rayReference.transform.localScale = new Vector3(rayThickness, rayLength, 1f); // Note: Set localScale in one line for conciseness
            for (int i = 1; i <= rayCount; i++)
            {
                GameObject ray = Instantiate(rayReference, rayReference.transform.position, rayReference.transform.rotation);
                ray.transform.parent = transform;
                ray.transform.localEulerAngles = new Vector3(90, 0f, 0f); // Specify all three angles for clarity
                ray.transform.localPosition = new Vector3(Random.Range(-rayRadius, rayRadius), Random.Range(-rayRadius, rayRadius), CenterOffset);
                ray.transform.localScale *= Random.Range(0.2f, 1.0f);
                ray.name = "(cloned) ray" + i;
            }
        }

        void UpdateSunRays()
        {
            transform.rotation = sun.rotation; // Direct assignment for rotation
            foreach (Transform ray in transform)
            {
                ray.localEulerAngles = new(camera.localEulerAngles.y + RotatePlaneOffset, 90, 90);
            }
        }
    }

}
