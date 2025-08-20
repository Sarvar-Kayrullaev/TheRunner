using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        sealedData = FindFirstObjectByType<SealedData>();
        holsterManager = FindFirstObjectByType<HolsterManager>();
        dataManager = FindFirstObjectByType<DataManager>();
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
            if (transform.GetChild(transform.childCount - 1).GetComponent<HandActionController>()) return;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        Transform hand = Instantiate(climbHand, transform);
        hand.GetComponent<HandActionController>().holster = this;
    }

    public void DrawWeapon(GameObject Prefab)
    {
        int selectedWeapon = dataManager.playerModel.SelectedWeaponIndex;
        EquipedWeaponModel equipedWeapon = dataManager.playerModel.Holster[selectedWeapon].EquipedWeapon;
        Debug.Log($"CurrentID: {CurrentWeaponID} == EquipedID: {equipedWeapon.ID}");
        if (CurrentWeaponID == equipedWeapon.ID) return;

        if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
        
        GameObject insWeapon = Instantiate(Prefab, transform);
        Weapon weapon = insWeapon.GetComponent<Weapon>();

        weapon.released = CurrentWeaponID == equipedWeapon.ID;
        CurrentWeaponID = equipedWeapon.ID;
        currentWeapon = weapon;
        weapon.holster = this;
        weapon.crosshair = crosshair;
        weapon.sway = GetComponent<WeaponSway>();
        weapon.stackCamera = stackCamera;
        weapon.forward = forward;
        weapon.currentAmmo = equipedWeapon.MagazineBulletCount;
        //weapon.sniperFocus = sniperFocus;
        weapon.WeaponType = WeaponTypeConverter(equipedWeapon.weaponName);
        weapon.SetSight(equipedWeapon.Sight);
        weapon.SetSuppressor(equipedWeapon.Suppressor);
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
    }

    public void RedrawWeapon()
    {
        int selectedWeapon = dataManager.playerModel.SelectedWeaponIndex;
        EquipedWeaponModel equipedWeapon = dataManager.playerModel.Holster[selectedWeapon].EquipedWeapon;
        
        if(equipedWeapon.weaponName == WeaponName.NONE) return;
        if (transform.childCount > 0) Destroy(transform.GetChild(0).gameObject);
        GameObject insWeapon = Instantiate(sealedData.GetWeaponBasicModelByName(equipedWeapon.weaponName).WeaponPrefab, transform);
        Weapon weapon = insWeapon.GetComponent<Weapon>();
        Animator insWeaponAnimator = insWeapon.GetComponent<Animator>();
        insWeaponAnimator.Play("ForceDraw");

        weapon.released = CurrentWeaponID == equipedWeapon.ID;
        CurrentWeaponID = equipedWeapon.ID;
        currentWeapon = weapon;
        weapon.holster = this;
        weapon.crosshair = crosshair;
        weapon.sway = GetComponent<WeaponSway>();
        weapon.stackCamera = stackCamera;
        weapon.forward = forward;
        weapon.currentAmmo = equipedWeapon.MagazineBulletCount;
        //weapon.sniperFocus = sniperFocus;
        weapon.WeaponType = WeaponTypeConverter(equipedWeapon.weaponName);
        weapon.SetSight(equipedWeapon.Sight);
        weapon.SetSuppressor(equipedWeapon.Suppressor);
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
                return weaponModel.WeaponType;
            }
        }
        return WeaponType.Special;
    }
}
