using Data;
using PlayerRoot;
using UnityEngine;
using UnityEngine.Serialization;

public class Dragable : MonoBehaviour
{
    public DragableType dragableType;
    public WeaponType weaponType;
    public GameObject prefab;
    public int id;
    public int currentAmmoSize;
    public SuppressorModel suppressorModel;
    public SightModel sightModel;

    public bool isRefillable = false;
    public int bullets;

    public float listenerDistance;
    private Player _player;
    private DataManager _dataManager;
    private SealedData _sealedData;
    private WeaponHolster _weaponHolster;
    private PlayerAudio _playerAudio;
    private Animator _animator;

    [FormerlySerializedAs("IsThrowed")] [HideInInspector]
    public bool isThrowed = false;

    public void Start()
    {
        _player = FindFirstObjectByType<Player>();
        _dataManager = FindFirstObjectByType<DataManager>();
        _sealedData = FindFirstObjectByType<SealedData>();
        _weaponHolster = FindFirstObjectByType<WeaponHolster>();
        _playerAudio = FindFirstObjectByType<PlayerAudio>();
        if(TryGetComponent(out Animator animator)) _animator = animator;

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
        else if(dragableType == DragableType.Bullet)
        {
            DragBullet();
        }
        else if(dragableType == DragableType.Box)
        {
            
        }
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
        if (weaponType == WeaponType.Handgun)
        {
            var maxBulletLevel = _sealedData.BulletBag[bulletBagLevel].PistolSize;
            var currentBulletBag = _dataManager.playerModel.BulletBag.PistolSize;
            if (currentBulletBag >= maxBulletLevel) return;
            int result;
            var outage = currentBulletBag + bullets > maxBulletLevel;
            if (outage)
            {
                result = maxBulletLevel;
                bullets = currentBulletBag + bullets - maxBulletLevel;
            }
            else
            {
                result = currentBulletBag + bullets;
                bullets = 0;
            }

            _dataManager.playerModel.BulletBag.PistolSize = result;
            _dataManager.UpdatePlayerModel(_dataManager.playerModel);
            _playerAudio.audio.PlayOneShot(_playerAudio.CLIP_PICK_AMMO, _playerAudio.Volume);
        }

        if (weaponType == WeaponType.Shotgun)
        {
            var maxBulletLevel = _sealedData.BulletBag[bulletBagLevel].ShotgunSize;
            var currentBulletBag = _dataManager.playerModel.BulletBag.ShotgunSize;
            if (currentBulletBag >= maxBulletLevel) return;
            int result;
            var outage = currentBulletBag + bullets > maxBulletLevel;
            if (outage)
            {
                result = maxBulletLevel;
                bullets = currentBulletBag + bullets - maxBulletLevel;
            }
            else
            {
                result = currentBulletBag + bullets;
                bullets = 0;
            }

            _dataManager.playerModel.BulletBag.ShotgunSize = result;
            _dataManager.UpdatePlayerModel(_dataManager.playerModel);
            _playerAudio.audio.PlayOneShot(_playerAudio.CLIP_PICK_AMMO, _playerAudio.Volume);
        }

        if (weaponType == WeaponType.SMG)
        {
            var maxBulletLevel = _sealedData.BulletBag[bulletBagLevel].SMGSize;
            var currentBulletBag = _dataManager.playerModel.BulletBag.SMGSize;
            if (currentBulletBag >= maxBulletLevel) return;
            int result;
            var outage = currentBulletBag + bullets > maxBulletLevel;
            if (outage)
            {
                result = maxBulletLevel;
                bullets = currentBulletBag + bullets - maxBulletLevel;
            }
            else
            {
                result = currentBulletBag + bullets;
                bullets = 0;
            }

            _dataManager.playerModel.BulletBag.SMGSize = result;
            _dataManager.UpdatePlayerModel(_dataManager.playerModel);
            _playerAudio.audio.PlayOneShot(_playerAudio.CLIP_PICK_AMMO, _playerAudio.Volume);
        }

        if (weaponType == WeaponType.Rifle)
        {
            var maxBulletLevel = _sealedData.BulletBag[bulletBagLevel].RifleSize;
            var currentBulletBag = _dataManager.playerModel.BulletBag.RifleSize;
            if (currentBulletBag >= maxBulletLevel) return;
            int result;
            var outage = currentBulletBag + bullets > maxBulletLevel;
            if (outage)
            {
                result = maxBulletLevel;
                bullets = currentBulletBag + bullets - maxBulletLevel;
            }
            else
            {
                result = currentBulletBag + bullets;
                bullets = 0;
            }

            _dataManager.playerModel.BulletBag.RifleSize = result;
            _dataManager.UpdatePlayerModel(_dataManager.playerModel);
            _playerAudio.audio.PlayOneShot(_playerAudio.CLIP_PICK_AMMO, _playerAudio.Volume);
        }

        if (weaponType == WeaponType.Sniper)
        {
            var maxBulletLevel = _sealedData.BulletBag[bulletBagLevel].SniperSize;
            var currentBulletBag = _dataManager.playerModel.BulletBag.SniperSize;
            if (currentBulletBag >= maxBulletLevel) return;
            int result;
            var outage = currentBulletBag + bullets > maxBulletLevel;
            if (outage)
            {
                result = maxBulletLevel;
                bullets = currentBulletBag + bullets - maxBulletLevel;
            }
            else
            {
                result = currentBulletBag + bullets;
                bullets = 0;
            }

            _dataManager.playerModel.BulletBag.SniperSize = result;
            _dataManager.UpdatePlayerModel(_dataManager.playerModel);
            _playerAudio.audio.PlayOneShot(_playerAudio.CLIP_PICK_AMMO, _playerAudio.Volume);
        }

        if (weaponType == WeaponType.Machinegun)
        {
            var maxBulletLevel = _sealedData.BulletBag[bulletBagLevel].MashineGunSize;
            var currentBulletBag = _dataManager.playerModel.BulletBag.MashineGunSize;
            if (currentBulletBag >= maxBulletLevel) return;
            int result;
            var outage = currentBulletBag + bullets > maxBulletLevel;
            if (outage)
            {
                result = maxBulletLevel;
                bullets = currentBulletBag + bullets - maxBulletLevel;
            }
            else
            {
                result = currentBulletBag + bullets;
                bullets = 0;
            }

            _dataManager.playerModel.BulletBag.MashineGunSize = result;
            _dataManager.UpdatePlayerModel(_dataManager.playerModel);
            _playerAudio.audio.PlayOneShot(_playerAudio.CLIP_PICK_AMMO, _playerAudio.Volume);
        }
        if (_weaponHolster.currentWeapon) _weaponHolster.RebuildBulletText(_weaponHolster.currentWeapon.weaponName);
        if (dragableType == DragableType.Bullet)
        {
            if(bullets <= 0) Destroy(gameObject);
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

public enum DragableType
{
    Item,
    Weapon,
    Bullet,
    Box
}