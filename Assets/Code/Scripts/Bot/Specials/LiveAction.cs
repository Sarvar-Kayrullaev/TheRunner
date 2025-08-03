using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class LiveAction : MonoBehaviour
    {
        public float normalSpeed = 0;
        public bool died = false;

        public void SetSpeed(float currentSpeed, float maxSpeed)
        {
            normalSpeed = currentSpeed / maxSpeed;
        }
    }
}

