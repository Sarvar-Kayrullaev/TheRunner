using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotRoot;
using UnityEngine;
using Data;
using Shader;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Weapon : MonoBehaviour
{
    [Header("Attachment Setup")]
    public Transform suppressorParentTransform;
    public Transform sightParentTransform;
    [Header("AttachmentOptions")]
    public SuppressorModel suppressorModel;

    public SightModel sightModel;
    [HideInInspector] public EquipedWeaponModel  equipedModel;
    [HideInInspector] public int equipedSlotIndex;
    
    [Space]
    public Animator animator;
    public WeaponName weaponName;
    [HideInInspector] public WeaponType weaponType;
    [SerializeField] private Transform muzzleFlashPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform bullerPrefab;
    
    [Header("Attachment")]
    [SerializeField] private GameObject ironSightPrefab;
    [Space]
    [Header("Attribute")]
    [SerializeField] private int damage;
    [SerializeField] private bool autoReload = false;
    
    [Space]
    [Header("Field Of View")]
    [SerializeField] public float fov = 60;
    [SerializeField] public float defaultAimFOV = 60;
    [SerializeField] public float aimFOV = 50;
    [Space]
    [SerializeField] public float stackFOV = 50;
    [SerializeField] public float defaultStackAimFOV = 50;
    [SerializeField] public float stackAimFOV = 50;
    [Space]
    [SerializeField]
    private float zoomSpeed = 10;
    [Space]
    [Header("Bullet Attributes")]
    [SerializeField]
    private float bulletSpeed;
    [SerializeField] private float bulletGravity;
    [SerializeField] private float fireRate;
    [SerializeField] private bool singleShot = true;
    [Space]
    [Header("Accuracy")]
    public float restingAccuracy;
    public float shootAccuracy;
    public float walkAccuracy;
    public float runAccuracy;
    public float aimAccuracyRate = 2;
    [HideInInspector] public float accuracy;
    [Space]
    [Header("Weapon Transform")]
    public Vector3 restPosition;
    public Vector3 defaultAimPosition;
    public Vector3 aimPosition;
    [SerializeField] private float positioningSpeed;
    
    [Space]
    [Header("Sway")]
    [Range(0, 1)]
    [SerializeField] public float reduceSwayOnAim = 0.1f;
    [Space]
    [Header("Mark")]
    [SerializeField] private LayerMask markLayer;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float markDistance = 100;
    [SerializeField] private float markAngle = 20;
    [SerializeField] private float markingTime = 1;
    [SerializeField] private AudioClip markSound;
    [Space]

    [SerializeField] private LayerMask hitableLayer;
    [HideInInspector] public Transform forward;
    [HideInInspector] public Camera stackCamera;
    [HideInInspector] public SniperFocus sniperFocus;
    [HideInInspector] public BotGlobal botGlobal;

    public bool aim;
    public int magazineSize;
    //public int allAmmoSize;

    // Variables //
    [HideInInspector] public int currentAmmo;
    [HideInInspector] public bool isReloading = false;
    [HideInInspector] public bool beforeShooting = true;
    [HideInInspector] public bool shootPressing = false;
    [HideInInspector] public bool shootOnce = false;
    [HideInInspector] public bool released = false;

    // PRIVATES //

    private float _nextTimeToFire = 0;
    private float _reloadAccessTime = 0.3f;
    public new Camera camera;
    private new AudioSource _audio;
    private bool _isScoped = false;
    private bool _isSilenced = false;

    [HideInInspector] public bool forceDraw = false;
    [HideInInspector] public Crosshair crosshair;
    [HideInInspector] public WeaponSway sway;
    [HideInInspector] public Recoil recoil;
    [HideInInspector] public WeaponHolster holster;

    [Space]
    [SerializeField]
    private float shootVolume;
    [SerializeField] private AudioClip[] fireSounds;
    [SerializeField] private AudioClip[] suppressedFireSounds;


    // Temps //
    private Bot _markingBot = null;
    private float _lastMarkingTime;
    
    [HideInInspector] public DataManager dataManager;
    private StartData _startData;
    private SealedData _sealedData;

    private void Start()
    {
        dataManager = FindFirstObjectByType<DataManager>();
        _startData = FindFirstObjectByType<StartData>();
        _sealedData = FindFirstObjectByType<SealedData>();
        
        CancelAim();
        camera = Camera.main;
        _audio = GetComponent<AudioSource>();
        animator.Play(released ? "ForceDraw" : "Draw");
        if (TryGetComponent(out Recoil recoil)) this.recoil = recoil;
        InvokeRepeating(nameof(Mark), 0, 0.2f);
        UpdateEquipment();
    }

    public void SetSuppressor(SuppressorModel suppressor)
    {
        _sealedData = FindFirstObjectByType<SealedData>();
        SealedSuppressorModel sealedSuppressor = _sealedData.Suppressors[(int) suppressor.name];
        suppressorModel = suppressor;
        
        if (sealedSuppressor.name == SuppressorName.None)
        {
            _isSilenced = false;
            if(suppressorParentTransform.childCount > 0) Destroy(suppressorParentTransform.GetChild(0).gameObject);
        }
        else
        {
            _isSilenced = true;
            GameObject suppressorObject = Instantiate(sealedSuppressor.prefab, suppressorParentTransform);
        }
    }

    public void SetSight(SightModel sight)
    {
        Debug.Log($"SightType: {sight.name}");
        _sealedData = FindFirstObjectByType<SealedData>();
        
        SealedSightModel sealedSight = _sealedData.Sights[(int) sight.name];
        sightModel = sight;
        if (sealedSight.name == SightName.IronSights)
        {
            _isScoped = false;
            if(sightParentTransform.childCount > 0) Destroy(sightParentTransform.GetChild(0).gameObject);

            if (ironSightPrefab)
            {
                GameObject sightObject = Instantiate(ironSightPrefab, sightParentTransform);   
            }
            
        }
        else
        {
            _isScoped = true;
            Debug.Log($"SightName: {sealedSight.title}");
            GameObject sightObject = Instantiate(sealedSight.prefab, sightParentTransform);
        }
    }
    private void OnEnable()
    {
        CancelAim();
        UpdateEquipment();
        animator.CrossFade(released ? "ForceDraw" : "Draw", 0, -1, 0);
        //animator.Play(released ? "ForceDraw" : "Draw");
    }

    private void Update()
    {
        if (autoReload && currentAmmo <= 0)
        {
            _reloadAccessTime -= Time.deltaTime;
            if (_reloadAccessTime <= 0)
            {
                Reload();
                _reloadAccessTime = 0.3f;
            }
        }
        Fire();
        WeaponPositioning();
        AimZoom();
        if (!holster.Mobile)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                Aim(true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                Aim(false);
            }
            if (Input.GetKeyDown(KeyCode.R)) Reload();
            shootPressing = Input.GetKey(KeyCode.Mouse0);
        }
        if (!shootPressing && !beforeShooting) _audio.PlayOneShot(fireSounds[2], shootVolume);
        if (!shootPressing) beforeShooting = true;

    }

    public void UpdateEquipment()
    {
        // SuppressorTransform.gameObject.SetActive();
        // ScopeTransform.gameObject.SetActive(isScoped);
        CancelAim();
    }
    public void Aim(bool aiming)
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("ReloadEmpty")
            || currentState.IsName("Reload")
            || currentState.IsName("ForceDraw")
            || currentState.IsName("Draw")) return;

        aim = aiming;
        crosshair.Aiming(aiming);
        animator.SetBool("Aim", aiming);
        
        if(weaponType == WeaponType.Sniper && _isScoped) if(sniperFocus) sniperFocus.SetFocus(aiming);
    }

    private void Mark()
    {
        if (!aim)
        {
            _lastMarkingTime = Time.time;
            _markingBot = null;
            return;
        }
        //Collider[] targets = Physics.OverlapSphere(transform.position, markDistance, markLayer);
        
        Bot nearestBot = null;
        var nearestAngle = Mathf.Infinity;
        foreach (var bot in botGlobal.bots)
        {
            if(botGlobal.markedBots.Contains(bot)) continue;
            
            var dirToTarget = (bot.transform.position - transform.position).normalized;
            var angle = Vector3.Angle(transform.forward, dirToTarget);
            
            if (angle < markAngle / 2)
            {
                foreach (var body in bot.setup.author.bodies)
                {
                    var distance = Vector3.Distance(camera.transform.position, body.transform.position);
                    if (!Physics.Raycast(camera.transform.position, body.transform.position - camera.transform.position, distance, obstacleMask))
                    {
                        if (angle < nearestAngle)
                        {
                            nearestAngle = angle;
                            nearestBot = bot;
                        }
                    }
                }
            }
        }
        
        if (nearestBot)
        {
            if (_markingBot == nearestBot)
            {
                if (Time.time - _lastMarkingTime >= markingTime)
                {
                    BotSetup setup = nearestBot.setup;
                    if (setup.author.marked) return;
                    if (setup.health.died) return;
                    if (setup.TryGetComponent(out Outline outline))
                    {
                        outline.enabled = true;
                        setup.author.SetMark(camera);
                        botGlobal.markedBots.Add(nearestBot);
                        _audio.PlayOneShot(markSound);
                    }
                }
            }
            else
            {
                _markingBot = nearestBot;
                _lastMarkingTime = Time.time;
            }
        }
        else
        {
            _lastMarkingTime = Time.time;
        }
    }

    public void CancelAim()
    {
        aim = false;
        if(crosshair) crosshair.Aiming(false);
        animator.SetBool("Aim", false);
        if(sniperFocus) sniperFocus.SetFocus(false);
    }
    public void Fire()
    {
        if (currentAmmo <= 0) return;
        if (shootPressing)
        {
            if (singleShot)
            {
                if (shootOnce)
                {
                    shootOnce = false;
                    if (Time.time >= _nextTimeToFire)
                    {
                        _nextTimeToFire = Time.time + 1f / fireRate;
                        OneShoot();
                    }
                }
            }
            else
            {
                if (Time.time >= _nextTimeToFire)
                {
                    _nextTimeToFire = Time.time + 1f / fireRate;
                    OneShoot();
                }
            }
        }
    }

    private void AimZoom()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("ReloadEmpty")
            || currentState.IsName("Reload")
            || currentState.IsName("ForceDraw")
            || currentState.IsName("Draw"))
        {
            float changedFOV = Mathf.Lerp(camera.fieldOfView, fov, zoomSpeed * Time.deltaTime);
            float changedStackFOV = Mathf.Lerp(stackCamera.fieldOfView, stackFOV, zoomSpeed * Time.deltaTime);
            camera.fieldOfView = changedFOV;
            stackCamera.fieldOfView = changedStackFOV;
        }
        else
        {
            if (aim)
            {
                float changedFOV = Mathf.Lerp(camera.fieldOfView,_isScoped? aimFOV: defaultAimFOV, zoomSpeed * Time.deltaTime);
                float changedStackFOV = Mathf.Lerp(stackCamera.fieldOfView,_isScoped? stackAimFOV: defaultStackAimFOV, zoomSpeed * Time.deltaTime);
                camera.fieldOfView = changedFOV;
                stackCamera.fieldOfView = changedStackFOV;
            }
            else
            {
                float changedFOV = Mathf.Lerp(camera.fieldOfView, fov, zoomSpeed * Time.deltaTime);
                float changedStackFOV = Mathf.Lerp(stackCamera.fieldOfView, stackFOV, zoomSpeed * Time.deltaTime);
                camera.fieldOfView = changedFOV;
                stackCamera.fieldOfView = changedStackFOV;
            }
        }

    }
    public void OneShoot()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("ReloadEmpty") || currentState.IsName("Reload")) return;
        ShootSound();
        CallToEnemy();
        currentAmmo--;
        equipedModel.magazineBulletCount = currentAmmo;
        dataManager.UpdatePlayerModel(dataManager.playerModel);
        if (crosshair)
        {
            crosshair.Shooting();
        }
        recoil.Fire(aim);
        holster.TakeBullet(currentAmmo + 1);
        RaycastHit hit;
        float crosshairSize = accuracy * 0.001f;
        float randomX = Random.Range(-crosshairSize, crosshairSize);
        float randomY = Random.Range(-crosshairSize, crosshairSize);
        float randomZ = Random.Range(-crosshairSize, crosshairSize);
        Vector3 offset = new Vector3(randomX, randomY, randomZ);
        Ray ray = new(camera.transform.position, camera.transform.forward + offset);

        if (Physics.Raycast(ray, out hit, 200, hitableLayer))
        {
            firePoint.LookAt(hit.point);
        }
        else
        {
            firePoint.LookAt(forward.position + offset * 150);
        }

        animator.CrossFade(aim ? "AimShoot" : "Shoot", 0, -1, 0);
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, firePoint.position, firePoint.rotation, firePoint).gameObject;
        Destroy(muzzleFlash, 1);

        Transform bullet = Instantiate(bullerPrefab, firePoint.position, firePoint.rotation);
        if (bullet.TryGetComponent(out ParabolicBullet parabolicBulletComponent))
        {
            parabolicBulletComponent.Initialize(firePoint, bulletSpeed, bulletGravity, damage, holster.player);
        }
    }

    private void CallToEnemy()
    {
        if(_isSilenced) return;
        Collider[] targets = Physics.OverlapSphere(transform.position, 200, markLayer);
        foreach (Collider target in targets)
        {
            if(target.TryGetComponent(out BotAuthor author))
            {
                author.setup.sensor.SetEnemyPosition(holster.player.transform, 4, true);
            }
        }
    }

    public void ShootSound()
    {
        
        if (beforeShooting)
        {
            if(_isSilenced) _audio.PlayOneShot(suppressedFireSounds[0], shootVolume);
            else _audio.PlayOneShot(fireSounds[0], shootVolume);
            beforeShooting = false;
        }
        else
        {
            if(_isSilenced) _audio.PlayOneShot(suppressedFireSounds[1], shootVolume);
            else _audio.PlayOneShot(fireSounds[1], shootVolume);
        }

    }
    public void Reload()
    {
        if (GetAllAmmo() <= 0) return; // Ammo Not Enought
        if (currentAmmo >= magazineSize || isReloading) return; // Already full magazine
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("ReloadEmpty") || currentState.IsName("Reload")) return;

        aim = false;
        crosshair.Aiming(false);
        animator.SetBool("Aim", false);
        animator.CrossFade(currentAmmo == 0 ? "ReloadEmpty" : "Reload", 0.07f, -1, 0);
        if(weaponType == WeaponType.Sniper && _isScoped) if(sniperFocus) sniperFocus.SetFocus(false);

    }

    private void WeaponPositioning()
    {
        if (aim)
        {
            if(_isScoped) transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, positioningSpeed * Time.deltaTime);
            else transform.localPosition = Vector3.Lerp(transform.localPosition, defaultAimPosition, positioningSpeed * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, restPosition, positioningSpeed * Time.deltaTime);
        }
    }

    public int GetAllAmmo()
    {
        var type = WeaponTypeConverter(weaponName);
        return type switch
        {
            WeaponType.Handgun => dataManager.playerModel.BulletBag.PistolSize,
            WeaponType.Shotgun => dataManager.playerModel.BulletBag.ShotgunSize,
            WeaponType.SMG => dataManager.playerModel.BulletBag.SMGSize,
            WeaponType.Rifle => dataManager.playerModel.BulletBag.RifleSize,
            WeaponType.Sniper => dataManager.playerModel.BulletBag.SniperSize,
            WeaponType.Machinegun => dataManager.playerModel.BulletBag.MashineGunSize,
            _ => 0
        };
    }

    public void SetAllAmmo(int value, bool saveData)
    {
        var type = WeaponTypeConverter(weaponName);
        switch (type)
        {
            case WeaponType.Handgun:
                dataManager.playerModel.BulletBag.PistolSize = value;
                break;
            case WeaponType.Shotgun:
                dataManager.playerModel.BulletBag.ShotgunSize = value;
                break;
            case WeaponType.SMG:
                dataManager.playerModel.BulletBag.SMGSize = value;
                break;
            case WeaponType.Rifle:
                dataManager.playerModel.BulletBag.RifleSize = value;
                break;
            case WeaponType.Sniper:
                dataManager.playerModel.BulletBag.SniperSize = value;
                break;
            case WeaponType.Machinegun:
                dataManager.playerModel.BulletBag.MashineGunSize = value;
                break;
            case WeaponType.Launcher:
            case WeaponType.Special:
            case WeaponType.Signatured:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        equipedModel.magazineBulletCount = currentAmmo;
        if(saveData) dataManager.UpdatePlayerModel(dataManager.playerModel);
    }

    private WeaponType WeaponTypeConverter(WeaponName weaponName)
    {
        foreach (WeaponBasicModel weaponModel in _sealedData.WeaponBasics)
        {
            if(weaponModel.weaponName == weaponName)
            {
                return weaponModel.weaponType;
            }
        }
        return WeaponType.Special;
    }
}
