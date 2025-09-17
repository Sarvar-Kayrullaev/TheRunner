using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class MovePosition : MonoBehaviour
    {
        public Vector3 lastPosition;
        public Transform Owner;
        private BotSetup setup;
        private int currentPathID;
        private bool IsStealthlyPosition = false;

        private void Start()
        {
            setup = Owner.GetComponent<BotSetup>();

            if (Owner.TryGetComponent(out BotSetup setupComponent))
            {
                setup =  setupComponent;
                setup.movePosition = this;
            }
            else
            {
                Debug.LogError($"{Owner.name} has no BotSetup component");
            }
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetPositionStealthly(Vector3 position)
        {
            lastPosition = position;
            IsStealthlyPosition = true;
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                setup.agent.nextPathId++;
                currentPathID = setup.agent.nextPathId;
                transform.hasChanged = false;
                IsStealthlyPosition = false;
            }
        }

        public Vector3 GetPosition()
        {
            return IsStealthlyPosition ? lastPosition : transform.position;
        }
    }
}

