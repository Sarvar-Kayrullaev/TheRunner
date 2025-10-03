using System;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class PlayerAbilityModel
    {
        public int healthLevel;
        public float moveSpeed;
        public float jumpPower;
        public int bulletBagLevel;
        public int fundsBagLevel;
        public int weaponSlotLevel;
        public bool canThrowRock;
        public bool canClimb;
    }
}