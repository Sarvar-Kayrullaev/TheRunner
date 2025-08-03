using System;

namespace Data
{
    [Serializable]
    public class PlayerAbilityModel
    {
        public int HealthLevel;
        public float MoveSpeed;
        public float JumpPower;
        public int BulletBagLevel;
        public int FundsBagLevel;
        public int WeaponSlotLevel;
        public bool CanThrowRock;
        public bool CanClimb;
    }
}