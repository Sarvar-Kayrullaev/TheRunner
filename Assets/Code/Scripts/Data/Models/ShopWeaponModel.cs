using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class ShopWeaponModel
    {
        [FormerlySerializedAs("WeaponEnum")] public WeaponName weaponName;
        public bool IsUnlocked;
        public bool IsPurchased;
        
        public SightModel Sight;
        public SuppressorModel Suppressor;
        
    }
    [Serializable]
    public class SightModel
    {
        public SightType Type;
        public bool Equipped;
    }
    [Serializable]
    public class SuppressorModel
    {
        public SuppressorType Type;
        public bool Equipped;
    }

    [Serializable]
    public class SealedSightModel
    {
        public string Name;
        public string Description;
        public SightType Type;
        public WeaponType[] compatibleWeaponsType;
        public int Price;
        public int Quantity;
        public GameObject Prefab;
        public Sprite ReferenceImage;
    }

    [Serializable]
    public class SealedSuppressorModel
    {
        public string Name;
        public string Description;
        public SuppressorType Type;
        public int Price;
        public GameObject Prefab;
        public Sprite ReferenceImage;
    }
}