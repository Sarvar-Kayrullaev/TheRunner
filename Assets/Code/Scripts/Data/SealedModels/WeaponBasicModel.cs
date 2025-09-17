using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class WeaponBasicModel
    {
        public int ID;
        public string Name;
        [Multiline]
        public string Description;
        [FormerlySerializedAs("WeaponEnum")] public WeaponName weaponName;
        public WeaponType WeaponType;
        public SuppressorType SuppressorType;
        public int WeaponPrice;
        public Sprite SpriteReference;
        public GameObject WeaponPrefab;
        public GameObject DroppedWeaponPrefab;
        public WeaponAttributeModel WeaponAttribute;
    }
}