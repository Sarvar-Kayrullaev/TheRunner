using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class TargetStatus : MonoBehaviour
    {
        public bool isDied = false;
        public bool crouch = false;
        public Movement movement;
        private void Start()
        {
            movement = Movement.Idle;
        }
    }

    public enum Movement
    {
        CrouchIdle,
        CrouchWalking,
        CrouchRunning,
        Idle,
        Walking,
        Running
    }
}
