using System;
namespace Data
{
    [Serializable]
    public class WeaponAttributeModel
    {
        public int Damage;
        public float ReloadTime;
        public float FireRate;
        public float Accuracy;
        public int MagazineSize;
        public float Mobility;
    }
}