using System;
using BotRoot;
using UnityEngine;
using UnityEngine.Serialization;

namespace Environment
{
    public class Destructor : MonoBehaviour
    {
        public bool isVelocity = false;
        public int velocityMultipler = 1;
        public int destruction = 100;

        private Rigidbody _rb;

        private void Start()
        {
            if (TryGetComponent(out Rigidbody rb)) this._rb = rb;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Destructible"))
            {
                if (!other.gameObject.TryGetComponent(out Fracture fracture)) return;
                var force = isVelocity ? (int)_rb.linearVelocity.magnitude * velocityMultipler : destruction;
                fracture.TakeHealth(force);
            }
            else if (other.gameObject.CompareTag("Environment"))
            {
                if (other.TryGetComponent(out Object element))
                {
                    element.PlaySound();
                    element.Fracturing();
                }
            }
            else if (other.gameObject.CompareTag("Live/Evil"))
            {
                if (other.TryGetComponent(out HitableObject hitable))
                {
                    hitable.HitBullet(5000, transform);
                }
            }
        }
    }
}