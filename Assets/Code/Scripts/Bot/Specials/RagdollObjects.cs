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

        public void SetDrag(float _drag)
        {
            foreach (var ragdoll in objects)
            {
                Rigidbody rigidbody = ragdoll.GetComponent<Rigidbody>();
                rigidbody.linearDamping = _drag;
            }
        }

        public void SetKinematic(bool kinematic)
        {
            foreach (var ragdoll in objects)
            {
                Rigidbody rigidbody = ragdoll.GetComponent<Rigidbody>();
                rigidbody.isKinematic = kinematic;
            }
        }

    }
}

