using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class EquipedWeaponModel
    {
        public int ID;
        [FormerlySerializedAs("WeaponEnum")] public WeaponName weaponName;
        public int MagazineBulletCount;
        public SuppressorModel Suppressor;
        public SightModel Sight;
    }
}