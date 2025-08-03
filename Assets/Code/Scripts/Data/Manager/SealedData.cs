using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class SealedData : MonoBehaviour
    {
        public List<BulletBagModel> BulletBag;
        public List<WeaponBasicModel> WeaponBasics;

        public WeaponBasicModel GetWeaponBasicModelByName(WeaponEnum weaponEnum)
        {
            foreach (WeaponBasicModel item in WeaponBasics)
            {
                if (item.WeaponEnum == weaponEnum)
                {
                    return item;
                }
            }
            return null;
        }
    }
}