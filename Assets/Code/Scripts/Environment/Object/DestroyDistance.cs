using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment
{
    public class DestroyDistance : MonoBehaviour
    {
        public float SelfDestructionDistance = 60;

        private new Transform camera;
        void Start()
        {
            camera = Camera.main.transform;
            InvokeRepeating(nameof(DestroySelf), 0, 5);
        }

        void DestroySelf()
        {
            float distance = Vector3.Distance(transform.position, camera.position);
            if (distance > SelfDestructionDistance)
                Destroy(gameObject);
        }
    }
}

