using System;
using System.Collections.Generic;
using Data;
using PlayerRoot;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class Dragable : MonoBehaviour
{
    public DragableType dragableType;
    public WeaponType weaponType;
    public GameObject prefab;
    public int id;
    public int currentAmmoSize;
    public SuppressorModel suppressorModel;
    public SightModel sightModel;
    public List<CollectibleItem> items;
    public Material collectedMaterial;
    public SkinnedMeshRenderer meshRenderer;

    public bool isRefillable = false;
    public bool isCollected = false;
    public int bullets;

    public float listenerDistance;
    private Player _player;
    private DataManager _dataManager;
    private SealedData _sealedData;
    private WeaponHolster _weaponHolster;
    private PlayerAudio _playerAudio;
    private ToastManager _toastManager;
    private Animator _animator;

    [HideInInspector] public bool isThrowed = false;

    public void Start()
    {
        _player = FindFirstObjectByType<Player>();
        _dataManager = FindFirstObjectByType<DataManager>();
        _sealedData = FindFirstObjectByType<SealedData>();
        _weaponHolster = FindFirstObjectByType<WeaponHolster>();
        _playerAudio = FindFirstObjectByType<PlayerAudio>();
        _toastManager = FindFirstObjectByType<ToastManager>();

        if (TryGetComponent(out Animator animator)) _animator = animator;

        InvokeRepeating(nameof(UpdateListener), 0, 0.4f);
        if (!isThrowed)
        {
            InitializeBulletAmout(weaponType);
        }

        if (!isThrowed) id = Random.Range(100000, 999999);
    }

    public void UpdateListener()
    {
        var distance = Vector3.Distance(_player.transform.position, transform.position);
        if (distance > listenerDistance) return;

        if (dragableType == DragableType.Item)
        {
        }
        else if (dragableType == DragableType.Weapon)
        {
            DragBullet();
            if (_player.PendingDragableWeapon)
            {
                if (_player.PendingDragableWeapon.gameObject == gameObject)
                {
                    _player.PendingDragableWeapon = this;
                    _player.dragableWeapon.Register(this);
                }
                else
                {
                    var anotherDragableDistance = Vector3.Distance(_player.PendingDragableWeapon.transform.position,
                        _player.transform.position);
                    if (anotherDragableDistance > distance)
                    {
                        _player.PendingDragableWeapon = this;
                        _player.dragableWeapon.Register(this);
                    }
                }
            }
            else
            {
                _player.PendingDragableWeapon = this;
                _player.dragableWeapon.Register(this);
            }
        }
        else if (dragableType == DragableType.Bullet)
        {
            DragBullet();
        }
        else if (dragableType == DragableType.Box)
        {
        }
    }

    public void DragItems()
    {
        var toasts = new List<PickupToast>();
        foreach (var collectibleItem in items)
        {
            if (collectibleItem.itemType == ItemType.Money)
            {
                _dataManager.playerModel.Funds.money += collectibleItem.amount;
                var toastObject = Instantiate(_toastManager.pickupToastPrefab, _toastManager.pickupToastParent);
                if (!toastObject.TryGetComponent(out PickupToast toast)) continue;
                toast.Initialize($"Money: +{collectibleItem.amount}", _toastManager.moneySprite);
                toasts.Add(toast);
            }
            else if (collectibleItem.itemType == ItemType.Gold)
            {
                _dataManager.playerModel.Funds.gold += collectibleItem.amount;
                var toastObject = Instantiate(_toastManager.pickupToastPrefab, _toastManager.pickupToastParent);
                if (!toastObject.TryGetComponent(out PickupToast toast)) continue;
                toast.Initialize($"Gold: +{collectibleItem.amount}", _toastManager.goldSprite);
                toasts.Add(toast);
            }
        }

        _toastManager.PlayPickupToasts(toasts, 3);
        _animator.CrossFade("Open", 0);
        _playerAudio.audio.PlayOneShot(_playerAudio.CLIP_PICK_AMMO, _playerAudio.Volume);
        _dataManager.UpdatePlayerModel(_dataManager.playerModel);
        transform.tag = "Untagged";
        meshRenderer.material = collectedMaterial;
        isCollected = true;
    }

    private void InitializeBulletAmout(WeaponType bulletWeaponType)
    {
        var bulletBagLevel = _dataManager.playerModel.PlayerAbility.bulletBagLevel;
        switch (bulletWeaponType)
        {
            case WeaponType.Handgun:
            {
                var maxBulletSize = _sealedData.BulletBag[bulletBagLevel].PistolSize;
                bullets = isRefillable ? maxBulletSize : Random.Range(2, 15);
                break;
            }
            case WeaponType.Shotgun:
            {
                var maxBulletSize = _sealedData.BulletBag[bulletBagLevel].ShotgunSize;
                bullets = isRefillable ? maxBulletSize : Random.Range(2, 12);
                break;
            }
            case WeaponType.SMG:
            {
                var maxBulletSize = _sealedData.BulletBag[bulletBagLevel].SMGSize;
                bullets = isRefillable ? maxBulletSize : Random.Range(10, 40);
                break;
            }
            case WeaponType.Rifle:
            {
                var maxBulletSize = _sealedData.BulletBag[bulletBagLevel].RifleSize;
                bullets = isRefillable ? maxBulletSize : Random.Range(5, 30);
                break;
            }
            case WeaponType.Machinegun:
            {
                var maxBulletSize = _sealedData.BulletBag[bulletBagLevel].MashineGunSize;
                bullets = isRefillable ? maxBulletSize : Random.Range(20, 60);
                break;
            }
            case WeaponType.Sniper:
            {
                var maxBulletSize = _sealedData.BulletBag[bulletBagLevel].SniperSize;
                bullets = isRefillable ? maxBulletSize : Random.Range(1, 10);
                break;
            }
            case WeaponType.Launcher:
            {
                var maxBulletSize = 5;
                bullets = isRefillable ? maxBulletSize : Random.Range(1, 5);
                break;
            }
            case WeaponType.Special:
            case WeaponType.Signatured:
            default:
                bullets = 0;
                break;
        }
    }

    private void DragBullet()
    {
        if (bullets <= 0) return;
        var bulletBagLevel = _dataManager.playerModel.PlayerAbility.bulletBagLevel;
        
        AddAmmo(weaponType, bulletBagLevel);
        

        if (_weaponHolster.currentWeapon) _weaponHolster.RebuildBulletText(_weaponHolster.currentWeapon.weaponName);
        if (dragableType == DragableType.Bullet)
        {
            if (bullets <= 0) Destroy(gameObject);
        }
    }

    public void AddAmmo(WeaponType type, int bulletBagLevel)
    {
        // Get player and sealed data references for convenience
        var model = _dataManager.playerModel;
        var bagData = _sealedData.BulletBag[bulletBagLevel];

        // Determine current and max ammo based on weapon type
        int current = 0;
        int maxCapacity = 0;
        string weaponLabel = "";

        switch (type)
        {
            case WeaponType.Handgun:
                current = model.BulletBag.PistolSize;
                maxCapacity = bagData.PistolSize;
                weaponLabel = "Handgun";
                break;
            case WeaponType.Shotgun:
                current = model.BulletBag.ShotgunSize;
                maxCapacity = bagData.ShotgunSize;
                weaponLabel = "Shotgun";
                break;

            case WeaponType.SMG:
                current = model.BulletBag.SMGSize;
                maxCapacity = bagData.SMGSize;
                weaponLabel = "SMG";
                break;

            case WeaponType.Rifle:
                current = model.BulletBag.RifleSize;
                maxCapacity = bagData.RifleSize;
                weaponLabel = "Rifle";
                break;
            case WeaponType.Sniper:
                current = model.BulletBag.SniperSize;
                maxCapacity = bagData.SniperSize;
                weaponLabel = "Shotgun";
                break;
            case WeaponType.Machinegun:
                current = model.BulletBag.MashineGunSize;
                maxCapacity = bagData.MashineGunSize;
                weaponLabel = "Machinegun";
                break;

            case WeaponType.Launcher:
            case WeaponType.Special:
            case WeaponType.Signatured:
            default:
                Debug.LogWarning($"Unsupported weapon type: {type}");
                return;
        }

        // If already full, skip
        if (current >= maxCapacity) return;

        // Calculate how many bullets we can actually add
        int spaceLeft = maxCapacity - current;
        int takenBulletSize = Mathf.Min(bullets, spaceLeft);

        // Update current ammo and leftover bullets
        current += takenBulletSize;
        bullets = Mathf.Max(bullets - takenBulletSize, 0);

        // Write updated ammo back to the correct weapon type
        switch (type)
        {
            case WeaponType.Handgun: model.BulletBag.PistolSize = current; break;
            case WeaponType.SMG: model.BulletBag.SMGSize = current; break;
            case WeaponType.Rifle: model.BulletBag.RifleSize = current; break;
            case WeaponType.Shotgun: model.BulletBag.ShotgunSize = current; break;
        }

        // Save player data
        _dataManager.UpdatePlayerModel(model);

        // Play pickup sound
        _playerAudio.audio.PlayOneShot(_playerAudio.CLIP_PICK_AMMO, _playerAudio.Volume);

        // --- Toast Notification ---
        var pickupToasts = new List<PickupToast>();
        var toastObject = Instantiate(_toastManager.pickupToastPrefab, _toastManager.pickupToastParent);

        if (toastObject.TryGetComponent(out PickupToast toast))
        {
            toast.Initialize($"{weaponLabel} Ammo: +{takenBulletSize}", _toastManager.bulletSprite);
            pickupToasts.Add(toast);
            _toastManager.PlayPickupToasts(pickupToasts, 3);
        }
    }


    private WeaponType WeaponTypeConverter(WeaponName weaponName)
    {
        foreach (var weaponModel in _sealedData.WeaponBasics)
        {
            if (weaponModel.weaponName == weaponName)
            {
                return weaponModel.weaponType;
            }
        }

        return WeaponType.Special;
    }
}

[Serializable]
public class CollectibleItem
{
    public ItemType itemType;
    public int amount;
    [Range(0, 1)] public float yieldPercent;
}

public enum ItemType
{
    Money,
    Gold,
    Bullet,
    Grenade,
    SprintBoost,
    HealingBoost,
    Tier
}

public enum DragableType
{
    Item,
    Weapon,
    Bullet,
    Box
}