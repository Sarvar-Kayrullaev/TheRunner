using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotRoot;
using UnityEngine;
using Data;
using Shader;

public class Weapon : MonoBehaviour
{
    [Header("Equipment")]
    public Transform SuppressorTransform;
    public Transform ScopeTransform;
    [Header("EquipmentOptions")]
    public bool isSilenced;
    public bool isScoped;
    [Space]
    public Animator animator;
    public WeaponEnum WeaponEnum;
    [HideInInspector] public WeaponType WeaponType;
    [SerializeField] Transform muzzleFlashPrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] Transform bullerPrefab;
    [Space]
    [Header("Attribute")]
    [SerializeField] int damage;
    [SerializeField] bool autoReload = false;

    [Space]
    [Header("Field Of View")]
    [SerializeField] public float FOV = 60;
    [SerializeField] public float defaultAimFOV = 60;
    [SerializeField] public float aimFOV = 50;
    [Space]
    [SerializeField] public float stackFOV = 50;
    [SerializeField] public float defaultStackAimFOV = 50;
    [SerializeField] public float stackAimFOV = 50;
    [Space]
    [SerializeField] float zoomSpeed = 10;
    [Space]
    [Header("Bullet Attributes")]
    [SerializeField] float bulletSpeed;
    [SerializeField] float bulletGravity;
    [SerializeField] float fireRate;
    [SerializeField] bool singleShot = true;
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
    [SerializeField] float positioningSpeed;
    [Space]
    [Header("Sway")]
    [Range(0, 1)]
    [SerializeField] public float ReduceSwayOnAim = 0.1f;
    [Space]
    [Header("Mark")]
    [SerializeField] LayerMask markLayer;
    [SerializeField] LayerMask obstacleMask;
    [SerializeField] float markDistance = 100;
    [SerializeField] float markAngle = 20;
    [SerializeField] float markingTime = 1;
    [SerializeField] AudioClip markSound;
    [Space]

    [SerializeField] LayerMask hitableLayer;
    [HideInInspector] public Transform forward;
    [HideInInspector] public Camera stackCamera;
    [HideInInspector] public SniperFocus sniperFocus;

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

    float nextTimeToFire = 0;
    float reloadAccessTime = 0.3f;
    public new Camera camera;
    private new AudioSource audio;

    [HideInInspector] public bool forceDraw = false;
    [HideInInspector] public Crosshair crosshair;
    [HideInInspector] public WeaponSway sway;
    [HideInInspector] public Recoil recoil;
    [HideInInspector] public WeaponHolster holster;

    [Space]
    [SerializeField] float shootVolume;
    [SerializeField] AudioClip[] fireSounds;
    [SerializeField] AudioClip[] suppressedFireSounds;


    // Temps //
    Transform markingTarget = null;
    float lastMarkingTime;

    private StartData data;
    private SealedData sealedData;

    void Start()
    {
        data = FindFirstObjectByType<StartData>();
        sealedData = FindFirstObjectByType<SealedData>();
        CancelAim();
        camera = Camera.main;
        audio = GetComponent<AudioSource>();
        animator.Play(released ? "ForceDraw" : "Draw");
        if (TryGetComponent(out Recoil recoil)) this.recoil = recoil;
        InvokeRepeating(nameof(Mark), 0, 0.2f);
        UpdateEquipment();
    }
    private void OnEnable()
    {
        CancelAim();
        UpdateEquipment();
        animator.CrossFade(released ? "ForceDraw" : "Draw", 0, -1, 0);
        //animator.Play(released ? "ForceDraw" : "Draw");
    }

    void Update()
    {
        if (autoReload && currentAmmo <= 0)
        {
            reloadAccessTime -= Time.deltaTime;
            if (reloadAccessTime <= 0)
            {
                Reload();
                reloadAccessTime = 0.3f;
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
        if (!shootPressing && !beforeShooting) audio.PlayOneShot(fireSounds[2], shootVolume);
        if (!shootPressing) beforeShooting = true;

    }

    public void UpdateEquipment()
    {
        SuppressorTransform.gameObject.SetActive(isSilenced);
        ScopeTransform.gameObject.SetActive(isScoped);
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
        if(WeaponType == WeaponType.Sniper && isScoped) if(sniperFocus) sniperFocus.SetFocus(aiming);
    }

    void Mark()
    {
        if (!aim)
        {
            lastMarkingTime = Time.time;
            markingTarget = null;
            return;
        }
        Collider[] targets = Physics.OverlapSphere(transform.position, markDistance, markLayer);
        Debug.Log("------------");
        Debug.Log("Targets: " + targets.Length);
        Transform nearesTarget = null;
        float nearestAngle = Mathf.Infinity;
        foreach (Collider target in targets)
        {
            Debug.Log("Target Name: " + target.transform.parent.name);
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            if (angle < markAngle / 2)
            {
                if (angle < nearestAngle)
                {
                    nearestAngle = angle;
                    nearesTarget = target.transform;
                }
            }
        }
        if (nearesTarget)
        {
            if (markingTarget == nearesTarget)
            {
                if (Time.time - lastMarkingTime >= markingTime)
                {
                    Debug.Log("Near");
                    float dstToTarget = Vector3.Distance(camera.transform.position, nearesTarget.transform.position);

                    if (!Physics.Raycast(camera.transform.position, nearesTarget.transform.position - camera.transform.position, dstToTarget, obstacleMask))
                    {

                        if (nearesTarget.TryGetComponent(out BotAuthor author))
                        {
                            if (author.marked) return;
                            if (author.setup.health.died) return;
                            if (author.setup.TryGetComponent(out Outline outline))
                            {
                                outline.enabled = true;
                                author.SetMark(camera);
                                audio.PlayOneShot(markSound);
                            }
                        }
                    }
                }
            }
            else
            {
                markingTarget = nearesTarget;
                lastMarkingTime = Time.time;
            }
        }
        else
        {
            lastMarkingTime = Time.time;
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
                    if (Time.time >= nextTimeToFire)
                    {
                        nextTimeToFire = Time.time + 1f / fireRate;
                        OneShoot();
                    }
                }
            }
            else
            {
                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f / fireRate;
                    OneShoot();
                }
            }
        }
    }
    void AimZoom()
    {
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.IsName("ReloadEmpty")
            || currentState.IsName("Reload")
            || currentState.IsName("ForceDraw")
            || currentState.IsName("Draw"))
        {
            float changedFOV = Mathf.Lerp(camera.fieldOfView, FOV, zoomSpeed * Time.deltaTime);
            float changedStackFOV = Mathf.Lerp(stackCamera.fieldOfView, stackFOV, zoomSpeed * Time.deltaTime);
            camera.fieldOfView = changedFOV;
            stackCamera.fieldOfView = changedStackFOV;
        }
        else
        {
            if (aim)
            {
                float changedFOV = Mathf.Lerp(camera.fieldOfView,isScoped? aimFOV: defaultAimFOV, zoomSpeed * Time.deltaTime);
                float changedStackFOV = Mathf.Lerp(stackCamera.fieldOfView,isScoped? stackAimFOV: defaultStackAimFOV, zoomSpeed * Time.deltaTime);
                camera.fieldOfView = changedFOV;
                stackCamera.fieldOfView = changedStackFOV;
            }
            else
            {
                float changedFOV = Mathf.Lerp(camera.fieldOfView, FOV, zoomSpeed * Time.deltaTime);
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
        ParabolicBullet parabolicBullet = bullet.GetComponent<ParabolicBullet>();
        parabolicBullet.Initialize(firePoint, bulletSpeed, bulletGravity, damage, holster.player);
    }

    void CallToEnemy()
    {
        if(isSilenced) return;
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
            if(isSilenced) audio.PlayOneShot(suppressedFireSounds[0], shootVolume);
            else audio.PlayOneShot(fireSounds[0], shootVolume);
            beforeShooting = false;
        }
        else
        {
            if(isSilenced) audio.PlayOneShot(suppressedFireSounds[1], shootVolume);
            else audio.PlayOneShot(fireSounds[1], shootVolume);
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
        if(WeaponType == WeaponType.Sniper && isScoped) if(sniperFocus) sniperFocus.SetFocus(false);

    }

    void WeaponPositioning()
    {
        if (aim)
        {
            if(isScoped) transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, positioningSpeed * Time.deltaTime);
            else transform.localPosition = Vector3.Lerp(transform.localPosition, defaultAimPosition, positioningSpeed * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, restPosition, positioningSpeed * Time.deltaTime);
        }
    }

    public int GetAllAmmo()
    {
        WeaponType type = WeaponTypeConverter(WeaponEnum);
        if(type == WeaponType.Handgun) return data.PlayerData.BulletBag.PistolSize;
        if(type == WeaponType.Shotgun) return data.PlayerData.BulletBag.ShotgunSize;
        if(type == WeaponType.SMG) return data.PlayerData.BulletBag.SMGSize;
        if(type == WeaponType.Rifle) return data.PlayerData.BulletBag.RifleSize;
        if(type == WeaponType.Sniper) return data.PlayerData.BulletBag.SniperSize;
        if(type == WeaponType.Machinegun) return data.PlayerData.BulletBag.MashineGunSize;
        else return 0;
    }

    public void SetAllAmmo(int value)
    {
        WeaponType type = WeaponTypeConverter(WeaponEnum);
        if(type == WeaponType.Handgun) data.PlayerData.BulletBag.PistolSize = value;
        if(type == WeaponType.Shotgun) data.PlayerData.BulletBag.ShotgunSize = value;
        if(type == WeaponType.SMG) data.PlayerData.BulletBag.SMGSize = value;
        if(type == WeaponType.Rifle) data.PlayerData.BulletBag.RifleSize = value;
        if(type == WeaponType.Sniper) data.PlayerData.BulletBag.SniperSize = value;
        if(type == WeaponType.Machinegun) data.PlayerData.BulletBag.MashineGunSize = value;
    }

    WeaponType WeaponTypeConverter(WeaponEnum weaponEnum)
    {
        foreach (WeaponBasicModel weaponModel in sealedData.WeaponBasics)
        {
            if(weaponModel.WeaponEnum == weaponEnum)
            {
                return weaponModel.WeaponType;
            }
        }
        return WeaponType.Special;
    }
}
