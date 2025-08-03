using BotRoot;
using UnityEngine;

namespace Environment
{
    public class Destructor : MonoBehaviour
    {

        public bool IsVelocity = false;
        public int VelocityMultipler = 1;
        public int Destruction = 100;

        private Rigidbody rb;

        void Start()
        {
            if (TryGetComponent(out Rigidbody rb)) this.rb = rb;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Fracture")
            {
                Fracture fracture = other.gameObject.GetComponent<Fracture>();
                int force = IsVelocity ? (int)rb.linearVelocity.magnitude * VelocityMultipler : Destruction;
                fracture.TakeHealth(force);
            }
            else if (other.gameObject.tag == "Environment")
            {
                if (other.TryGetComponent(out Object _object)) _object.Fracturing();
            }
            else if (other.gameObject.tag == "Live/Evil")
            {
                if (other.TryGetComponent(out HitableObject hitable))
                {
                    hitable.HitBullet(5000, transform);
                }
            }
        }
    }
}
