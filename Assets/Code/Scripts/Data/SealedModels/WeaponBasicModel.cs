using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class WeaponBasicModel
    {
        public int id;
        public string name;
        [Multiline]
        public string description;
        public WeaponName weaponName;
        public WeaponType weaponType;
        public SuppressorName suppressorName;
        public int weaponPrice;
        public Sprite spriteReference;
        public GameObject weaponPrefab;
        public GameObject droppedWeaponPrefab;
        public WeaponAttributeModel weaponAttribute;
    }
}