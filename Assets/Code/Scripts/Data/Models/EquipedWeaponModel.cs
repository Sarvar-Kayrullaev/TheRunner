using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class EquipedWeaponModel
    {
        public int ID;
        public WeaponEnum WeaponEnum;
        public int MagazineBulletCount;
        public bool Suppressed;
        public bool Scoped;
    }
}