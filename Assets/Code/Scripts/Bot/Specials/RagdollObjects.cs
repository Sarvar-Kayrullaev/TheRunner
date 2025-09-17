using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class RagdollObjects : MonoBehaviour
    {
        public GameObject[] objects;
        public float drag;
        public bool rigidbodyActive;
        void Start()
        {
            SetDrag(drag);
            SetKinematic(true);
        }

        public void SetDrag(float drag)
        {
            foreach (var ragdoll in objects)
            {
                if (ragdoll.TryGetComponent(out Rigidbody component))
                {
                    component.linearDamping = drag;
                }
            }
        }

        public void SetKinematic(bool kinematic)
        {
            foreach (var ragdoll in objects)
            {
                if (ragdoll.TryGetComponent(out Rigidbody component))
                {
                    component.isKinematic = kinematic;
                }
            }
        }

    }
}

