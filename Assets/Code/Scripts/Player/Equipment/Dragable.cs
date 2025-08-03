using Data;
using PlayerRoot;
using UnityEngine;

public class Dragable : MonoBehaviour
{

    [Space][Header("Weapon Params")]
    public DragableType DragableType;
    public GameObject Prefab;
    public int ID;
    public int CurrentAmmoSize;
    public int Bullets;
    public bool Suppressed = false;
    public bool Scoped = false;

    [Space][Header("Dragable Params")]
    public float listenerDistance;
    public Player player;
    StartData data;
    SealedData sealedData;
    WeaponHolster weaponHolster;
    PlayerAudio playerAudio;

    [HideInInspector] public bool IsThrowed = false;

    public void Start()
    {
        player = FindFirstObjectByType<Player>();
        data = FindFirstObjectByType<StartData>();
        sealedData = FindFirstObjectByType<SealedData>();
        weaponHolster = FindFirstObjectByType<WeaponHolster>();
        playerAudio = FindFirstObjectByType<PlayerAudio>();

        InvokeRepeating(nameof(UpdateListener), 0, 0.4f);
        if(!IsThrowed) Bullets = Random.Range(0, 30);
        if(!IsThrowed) ID = Random.Range(100000, 999999);
    }

    public void UpdateListener()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > listenerDistance) return;

        if (DragableType == DragableType.Weapon)
        {
            DragBullet();
            if (player.PendingDragableWeapon)
            {
                if (player.PendingDragableWeapon.gameObject == gameObject)
                {
                    player.PendingDragableWeapon = this;
                    player.dragableWeapon.Register(this);
                }
                else
                {
                    float anotherDragableDistance = Vector3.Distance(player.PendingDragableWeapon.transform.position, player.transform.position);
                    if (anotherDragableDistance > distance)
                    {
                        player.PendingDragableWeapon = this;
                        player.dragableWeapon.Register(this);
                    }
                }
            }
            else
            {
                player.PendingDragableWeapon = this;
                player.dragableWeapon.Register(this);
            }
        }
    }

    void DragBullet()
    {
        if (Bullets <= 0) return;
        if (Prefab.TryGetComponent(out Weapon weapon))
        {
            WeaponType weaponType = WeaponTypeConverter(weapon.WeaponEnum);
            int bulletBagLevel = data.PlayerData.PlayerAbility.BulletBagLevel;
            if (weaponType == WeaponType.Handgun)
            {
                int maxBulletLevel = sealedData.BulletBag[bulletBagLevel].PistolSize;
                int currentBulletBag = data.PlayerData.BulletBag.PistolSize;
                if(currentBulletBag >= maxBulletLevel) return;
                int result;
                bool outage = currentBulletBag + Bullets > maxBulletLevel;
                if (outage)
                {
                    result = maxBulletLevel;
                    Bullets = currentBulletBag + Bullets - maxBulletLevel;
                }
                else
                {
                    result = currentBulletBag + Bullets;
                    Bullets = 0;
                }
                data.PlayerData.BulletBag.PistolSize = result;
                playerAudio.audio.PlayOneShot(playerAudio.CLIP_PICK_AMMO, playerAudio.Volume);
            }
            if (weaponType == WeaponType.Shotgun)
            {
                int maxBulletLevel = sealedData.BulletBag[bulletBagLevel].ShotgunSize;
                int currentBulletBag = data.PlayerData.BulletBag.ShotgunSize;
                if(currentBulletBag >= maxBulletLevel) return;
                int result;
                bool outage = currentBulletBag + Bullets > maxBulletLevel;
                if (outage)
                {
                    result = maxBulletLevel;
                    Bullets = currentBulletBag + Bullets - maxBulletLevel;
                }
                else
                {
                    result = currentBulletBag + Bullets;
                    Bullets = 0;
                }
                data.PlayerData.BulletBag.ShotgunSize = result;
                playerAudio.audio.PlayOneShot(playerAudio.CLIP_PICK_AMMO, playerAudio.Volume);
            }
            if (weaponType == WeaponType.SMG)
            {
                int maxBulletLevel = sealedData.BulletBag[bulletBagLevel].SMGSize;
                int currentBulletBag = data.PlayerData.BulletBag.SMGSize;
                if(currentBulletBag >= maxBulletLevel) return;
                int result;
                bool outage = currentBulletBag + Bullets > maxBulletLevel;
                if (outage)
                {
                    result = maxBulletLevel;
                    Bullets = currentBulletBag + Bullets - maxBulletLevel;
                }
                else
                {
                    result = currentBulletBag + Bullets;
                    Bullets = 0;
                }
                data.PlayerData.BulletBag.SMGSize = result;
                playerAudio.audio.PlayOneShot(playerAudio.CLIP_PICK_AMMO, playerAudio.Volume);
            }
            if (weaponType == WeaponType.Rifle)
            {
                int maxBulletLevel = sealedData.BulletBag[bulletBagLevel].RifleSize;
                int currentBulletBag = data.PlayerData.BulletBag.RifleSize;
                if(currentBulletBag >= maxBulletLevel) return;
                int result;
                bool outage = currentBulletBag + Bullets > maxBulletLevel;
                if (outage)
                {
                    result = maxBulletLevel;
                    Bullets = currentBulletBag + Bullets - maxBulletLevel;
                }
                else
                {
                    result = currentBulletBag + Bullets;
                    Bullets = 0;
                }
                data.PlayerData.BulletBag.RifleSize = result;
                playerAudio.audio.PlayOneShot(playerAudio.CLIP_PICK_AMMO, playerAudio.Volume);
            }
            if (weaponType == WeaponType.Sniper)
            {
                int maxBulletLevel = sealedData.BulletBag[bulletBagLevel].SniperSize;
                int currentBulletBag = data.PlayerData.BulletBag.SniperSize;
                if(currentBulletBag >= maxBulletLevel) return;
                int result;
                bool outage = currentBulletBag + Bullets > maxBulletLevel;
                if (outage)
                {
                    result = maxBulletLevel;
                    Bullets = currentBulletBag + Bullets - maxBulletLevel;
                }
                else
                {
                    result = currentBulletBag + Bullets;
                    Bullets = 0;
                }
                data.PlayerData.BulletBag.SniperSize = result;
                playerAudio.audio.PlayOneShot(playerAudio.CLIP_PICK_AMMO, playerAudio.Volume);
            }
            if (weaponType == WeaponType.Machinegun)
            {
                int maxBulletLevel = sealedData.BulletBag[bulletBagLevel].MashineGunSize;
                int currentBulletBag = data.PlayerData.BulletBag.MashineGunSize;
                if(currentBulletBag >= maxBulletLevel) return;
                int result;
                bool outage = currentBulletBag + Bullets > maxBulletLevel;
                if (outage)
                {
                    result = maxBulletLevel;
                    Bullets = currentBulletBag + Bullets - maxBulletLevel;
                }
                else
                {
                    result = currentBulletBag + Bullets;
                    Bullets = 0;
                }
                data.PlayerData.BulletBag.MashineGunSize = result;
                playerAudio.audio.PlayOneShot(playerAudio.CLIP_PICK_AMMO, playerAudio.Volume);
            }
            if (weaponHolster.currentWeapon) weaponHolster.RebuildBulletText(weaponHolster.currentWeapon.WeaponEnum);
        }
    }

    WeaponType WeaponTypeConverter(WeaponEnum weaponEnum)
    {
        foreach (WeaponBasicModel weaponModel in sealedData.WeaponBasics)
        {
            if (weaponModel.WeaponEnum == weaponEnum)
            {
                return weaponModel.WeaponType;
            }
        }
        return WeaponType.Special;
    }
}

public enum DragableType
{
    Item,
    Weapon,
    Bullet,
    Fund
}
