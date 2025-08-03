using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class HolsterModel
    {
        public int Index;
        public bool IsOccupied;
        public bool IsLocked;
        public EquipedWeaponModel EquipedWeapon;
    }
}