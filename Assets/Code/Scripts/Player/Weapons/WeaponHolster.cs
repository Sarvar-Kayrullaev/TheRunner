using System;
using System.Collections;
using System.Collections.Generic;
using BotRoot;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayerRoot;
using Data;
using Shader;

public class WeaponHolster : MonoBehaviour
{
    public Player player;
    public AmmoBag ammoBag;
    public TMP_Text ammoBagText;
    public Transform rockThrowPefab;
    public Transform climbHand;
    public Crosshair crosshair;
    public int currentWeaponIndex = 0;
    public int CurrentWeaponID;
    public bool Mobile;

    [Space]
    public Transform recoilPosition;
    public Transform recoilRotation;
    public Transform forward;
    public Camera stackCamera;
    //public SniperFocus sniperFocus;
    [HideInInspector] public Weapon currentWeapon = null;

    private SealedData sealedData;
    private HolsterManager holsterManager;
    private DataManager dataManager;
    private BotGlobal botGlobal;

    private void Awake()
    {
        sealedData = FindFirstObjectByType<SealedData>();
        holsterManager = FindFirstObjectByType<HolsterManager>();
        dataManager = FindFirstObjectByType<DataManager>();
        botGlobal = FindFirstObjectByType<BotGlobal>();
        //sniperFocus = FindFirstObjectByType<SniperFocus>();
        //ResetHolster();
    }

    private void Start()
    {
        BuildWeaponHolster();
    }

    void BuildWeaponHolster()
    {
        foreach (Transform child in transform)
        {
            
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            RockThrow();
        }
        if (currentWeapon)
        {
            //
        }
    }

    public void RockThrow()
    {
        holsterManager.SaveCurrentWeaponParams();
        if (transform.childCount > 0)
        {
            if (transform.GetChild(transform.childCount - 1).GetComponent<RockThrowWeapon>()) return;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        Transform rockThrow = Instantiate(rockThrowPefab, transform);
        rockThrow.GetComponent<RockThrowWeapon>().holster = this;
    }

    public void Climb()
    {
        holsterManager.SaveCurrentWeaponParams();
        if (transform.childCount > 0)
        {
            if (transform.GetChild(transform.childCount - 1).TryGetComponent(out HandActionController handActionControllerComponent))
            {
                return;
            }
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        var hand = Instantiate(climbHand, transform);

        if (hand.TryGetComponent(out HandActionController handActionController))
        {
            handActionController.holster = this;
        }
    }

    public void DrawWeapon(GameObject Prefab)
    {
        int selectedWeapon = dataManager.playerModel.SelectedWeaponIndex;
        EquipedWeaponModel equipedWeapon = dataManager.playerModel.Holster[selectedWeapon].equipedWeapon;
        if (CurrentWeaponID == equipedWeapon.id) return;

        if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
        
        GameObject insWeapon = Instantiate(Prefab, transform);
        if (insWeapon.TryGetComponent(out Weapon weapon))
        {
            weapon.released = CurrentWeaponID == equipedWeapon.id;
            CurrentWeaponID = equipedWeapon.id;
            currentWeapon = weapon;
            weapon.holster = this;
            weapon.crosshair = crosshair;
            TryGetComponent(out WeaponSway weaponSwayComponent);
            weapon.sway = weaponSwayComponent;
            weapon.stackCamera = stackCamera;
            weapon.botGlobal = botGlobal;
            weapon.forward = forward;
            weapon.currentAmmo = equipedWeapon.magazineBulletCount;
            weapon.equipedModel = equipedWeapon;
            weapon.equipedSlotIndex = selectedWeapon;
            //weapon.sniperFocus = sniperFocus;
            weapon.weaponType = WeaponTypeConverter(equipedWeapon.weaponName);
            weapon.SetSight(equipedWeapon.sight);
            weapon.SetSuppressor(equipedWeapon.suppressor);
            crosshair.weapon = weapon;
            crosshair.restingSize = weapon.restingAccuracy;
            crosshair.shootSize = weapon.shootAccuracy;
            crosshair.walkSize = weapon.walkAccuracy;
            crosshair.runSize = weapon.runAccuracy;
            crosshair.aimAccuracyRate = weapon.aimAccuracyRate;
            
            weapon.TryGetComponent(out Recoil recoilComponent);
            recoilComponent.RecoilPositionTranform = recoilPosition;
            recoilComponent.RecoilRotationTranform = recoilRotation;
            weapon.forceDraw = true;

            RebuildBulletText(equipedWeapon.weaponName);

            RebuildBullet(weapon.currentAmmo, weapon.magazineSize);
        }
    }

    public void RedrawWeapon()
    {
        int selectedWeapon = dataManager.playerModel.SelectedWeaponIndex;
        EquipedWeaponModel equipedWeapon = dataManager.playerModel.Holster[selectedWeapon].equipedWeapon;
        
        if(equipedWeapon.weaponName == WeaponName.NONE) return;
        if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
        GameObject insWeapon = Instantiate(sealedData.GetWeaponBasicModelByName(equipedWeapon.weaponName).weaponPrefab, transform);
        Weapon weapon = insWeapon.GetComponent<Weapon>();
        Animator insWeaponAnimator = insWeapon.GetComponent<Animator>();
        insWeaponAnimator.Play("ForceDraw");

        weapon.released = CurrentWeaponID == equipedWeapon.id;
        CurrentWeaponID = equipedWeapon.id;
        currentWeapon = weapon;
        weapon.holster = this;
        weapon.crosshair = crosshair;
        weapon.sway = GetComponent<WeaponSway>();
        weapon.stackCamera = stackCamera;
        weapon.botGlobal = botGlobal;
        weapon.forward = forward;
        weapon.currentAmmo = equipedWeapon.magazineBulletCount;
        //weapon.sniperFocus = sniperFocus;
        weapon.weaponType = WeaponTypeConverter(equipedWeapon.weaponName);
        weapon.SetSight(equipedWeapon.sight);
        weapon.SetSuppressor(equipedWeapon.suppressor);
        crosshair.weapon = weapon;
        crosshair.restingSize = weapon.restingAccuracy;
        crosshair.shootSize = weapon.shootAccuracy;
        crosshair.walkSize = weapon.walkAccuracy;
        crosshair.runSize = weapon.runAccuracy;
        crosshair.aimAccuracyRate = weapon.aimAccuracyRate;

        Recoil recoil = weapon.GetComponent<Recoil>();
        recoil.RecoilPositionTranform = recoilPosition;
        recoil.RecoilRotationTranform = recoilRotation;
        weapon.forceDraw = true;

        RebuildBulletText(equipedWeapon.weaponName);

        RebuildBullet(weapon.currentAmmo, weapon.magazineSize);
        weapon.released = true;
    }

    public void DrawHand()
    {
        if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
        CurrentWeaponID = 0;

        RebuildBulletText(WeaponName.NONE);

        RebuildBullet(0, 0);
    }

    public void TakeBullet(int currentAmmo)
    {
        ammoBagText.text = "" + currentWeapon.GetAllAmmo();
        ammoBag.TakeBullet(currentAmmo);
    }

    public void RebuildBullet(int currentAmmo, int magazineSize)
    {
        ammoBag.Build(currentAmmo, magazineSize);
    }

    public void ResetBullets(int currentAmmo, int  magazineSize)
    {
        ammoBag.ResetBullets(currentAmmo, magazineSize);
    }

    public void RebuildBulletText(WeaponName weaponName)
    {
        WeaponType type = WeaponTypeConverter(weaponName);
        if (type == WeaponType.Handgun) ammoBagText.text = "" + dataManager.playerModel.BulletBag.PistolSize;
        else if (type == WeaponType.Shotgun) ammoBagText.text = "" + dataManager.playerModel.BulletBag.ShotgunSize;
        else if (type == WeaponType.SMG) ammoBagText.text = "" + dataManager.playerModel.BulletBag.SMGSize;
        else if (type == WeaponType.Rifle) ammoBagText.text = "" + dataManager.playerModel.BulletBag.RifleSize;
        else if (type == WeaponType.Sniper) ammoBagText.text = "" + dataManager.playerModel.BulletBag.SniperSize;
        else if (type == WeaponType.Machinegun) ammoBagText.text = "" + dataManager.playerModel.BulletBag.MashineGunSize;
        else ammoBagText.text = "0";
    }

    WeaponType WeaponTypeConverter(WeaponName weaponName)
    {
        foreach (WeaponBasicModel weaponModel in sealedData.WeaponBasics)
        {
            if (weaponModel.weaponName == weaponName)
            {
                return weaponModel.weaponType;
            }
        }
        return WeaponType.Special;
    }
}
