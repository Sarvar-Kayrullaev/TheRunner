using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BotRoot
{
    public class BotEnum
    {
        public enum MentalState
        {
            Quiet,
            Suspicion,
            Panic
        }
        public enum Purpose
        {
            Attacking,
            Patrolling,
            Checking,
            Alarm,
            Avoid,
            Command
        }
        public enum Command
        {
            Guarding,
            Working,
            Delivery,
            Attacking,
            Special
        }
        public enum AssaultCommand
        {
            Front,
            Point,
            Avoid
        }

        public enum PatrollCommand
        {
            MainPoint,
            PatrollPoint,
            Avoid
        }

        public enum BotType
        {
            Basic,
            Armored,
            Sniper,
            Boss
        }
    }
}

