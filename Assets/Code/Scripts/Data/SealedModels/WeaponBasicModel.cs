using System;
using UnityEngine;
namespace Data
{
    [Serializable]
    public class WeaponBasicModel
    {
        public int ID;
        public string Name;
        public WeaponEnum WeaponEnum;
        public WeaponType WeaponType;
        public ScopeType ScopeType;
        public SuppressorType SuppressorType;
        public int WeaponPrice;
        public Sprite SpriteReference;
        public GameObject WeaponPrefab;
        public GameObject DroppedWeaponPrefab;
        public WeaponAttributeModel WeaponAttribute;
    }
}