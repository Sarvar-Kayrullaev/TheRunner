using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class StartData : MonoBehaviour
    {
        public bool CreatePlayerData = false;
        public Transform OutpostParent;
        public OpenWorldModel OpenWorldData = new();
        public PlayerModel PlayerData = new();


        public ShopWeaponModel GetShopWeaponModel(List<ShopWeaponModel> shopWeaponModels, WeaponName weaponName)
        {
            return shopWeaponModels.FirstOrDefault(shopWeaponModel => shopWeaponModel.weaponName == weaponName);
        }
    }
}

