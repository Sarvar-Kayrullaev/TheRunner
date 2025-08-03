using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class PlayerModel
    {
        public PlayerAbilityModel PlayerAbility;
        public FundsModel Funds;
        public BulletBagModel BulletBag;
        public int SelectedWeaponIndex;
        public List<HolsterModel> Holster = new();
    }
}

