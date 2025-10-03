using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class EquipedWeaponModel
    {
        public int id;
        public WeaponName weaponName;
        public int magazineBulletCount;
        public SuppressorModel suppressor;
        public SightModel sight;
    }
}