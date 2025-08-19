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
        public List<SealedSightModel> Sights;
        public List<SealedSuppressorModel> Suppressors;

        public WeaponBasicModel GetWeaponBasicModelByName(WeaponName weaponName)
        {
            foreach (WeaponBasicModel item in WeaponBasics)
            {
                if (item.weaponName == weaponName)
                {
                    return item;
                }
            }
            return null;
        }
    }
}