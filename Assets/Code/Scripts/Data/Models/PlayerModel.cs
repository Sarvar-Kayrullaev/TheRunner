using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;
using static UnityEngine.JsonUtility;

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
        public List<ShopWeaponModel>  ShopWeapons = new();

        public PlayerModel(PlayerModel other)
        {
            var json = ToJson(other);
            var cloned = FromJson<PlayerModel>(json);
            this.PlayerAbility = cloned.PlayerAbility;
            this.Funds = cloned.Funds;
            this.BulletBag = cloned.BulletBag;
            this.SelectedWeaponIndex = cloned.SelectedWeaponIndex;
            this.Holster = cloned.Holster;
            this.ShopWeapons = cloned.ShopWeapons;
        }
        
        public PlayerModel()
        {
            
        }
    }
}

