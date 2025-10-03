using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.JsonUtility;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class ShopWeaponModel
    {
        public WeaponName weaponName;
        public bool isUnlocked;
        public bool isPurchased;
        
        public SightModel sight;
        public SuppressorModel suppressor;
        
        public List<SightModel> purchasedSights;
        public List<SuppressorModel> purchasedSuppressors;
        
        
        public ShopWeaponModel(ShopWeaponModel other)
        {
            var json = ToJson(other);
            var cloned = FromJson<ShopWeaponModel>(json);
            weaponName = cloned.weaponName;
            isUnlocked = cloned.isUnlocked;
            isPurchased = cloned.isPurchased;
            sight = cloned.sight;
            suppressor = cloned.suppressor;
            purchasedSights = cloned.purchasedSights;
            purchasedSuppressors = cloned.purchasedSuppressors;
        }

        public ShopWeaponModel()
        {
            
        }
        
    }
    [Serializable]
    public class SightModel
    {
        public SightName name;
        public bool equipped;
    }
    [Serializable]
    public class SuppressorModel
    {
        public SuppressorName name;
        public bool equipped;
    }

    [Serializable]
    public class SealedSightModel
    {
        public string title;
        public string description;
        public SightName name;
        public WeaponName[] compatibleWeaponsName;
        public int price;
        public int quantity;
        public GameObject prefab;
        public Sprite referenceImage;
    }

    [Serializable]
    public class SealedSuppressorModel
    {
        public string title;
        public string description;
        public SuppressorName name;
        public WeaponName[] compatibleWeaponsName;
        public int price;
        public GameObject prefab;
        public Sprite referenceImage;
    }
}