using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data
{
    [Serializable]
    public class HolsterModel
    {
        public int index;
        public bool isOccupied;
        public bool isLocked;
        public EquipedWeaponModel equipedWeapon;
    }
}