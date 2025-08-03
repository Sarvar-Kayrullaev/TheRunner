using UnityEngine;

namespace Environment
{
    public class Fracture : MonoBehaviour
    {
        public int Health = 2;
        public Transform DestructionParticle;
        void Start()
        {
            gameObject.tag = "Fracture";
        }

        public void TakeHealth(int value)
        {
            Health -= value;
            if (Health <= 0)
            {
                Transform destruction = Instantiate(DestructionParticle, transform.position, transform.rotation);
                Destroy(destruction.gameObject, 3);
                Unchild();
                Destroy(gameObject);
            }
        }

        void Unchild()
        {
            foreach (Transform child in transform)
            {
                if (transform.parent)
                {
                    child.parent = transform.parent;
                }
                else
                {
                    child.parent = null;
                }
            }
        }
    }
}
