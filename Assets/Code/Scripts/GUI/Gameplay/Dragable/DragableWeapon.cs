using System;
using System.Collections.Generic;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DragableWeapon : MonoBehaviour
{
    public GameObject DragableWeaponGUI;
    public GameObject SecondDragableWeaponGUI;
    [HideInInspector] new public bool enabled;
    HolsterManager holsterManager;
    Dragable dragable;
    SealedData sealedData;
    WeaponHolster weaponHolster;
    WeaponName _weaponName;
    StartData data;
    PlayerAudio playerAudio;
    DilerSlot diler;
    Weapon weaponComponent;

    [Space]
    [Header("Setup")]
    [SerializeField] Image image;
    [SerializeField] new TMP_Text name;

    void Awake()
    {
        sealedData = FindFirstObjectByType<SealedData>();
        data = FindFirstObjectByType<StartData>();
        holsterManager = FindFirstObjectByType<HolsterManager>();
        weaponHolster = FindFirstObjectByType<WeaponHolster>();
        playerAudio = FindFirstObjectByType<PlayerAudio>();
    }

    void Update()
    {
        if (!enabled) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            Pick(0);
        }
    }

    public void Register(Dragable dragable)
    {
        if(SecondDragableWeaponGUI.TryGetComponent(out DilerSlot diler)) this.diler = diler;
        if (dragable.Prefab.TryGetComponent(out Weapon _weaponComponent)) weaponComponent = _weaponComponent;
        
        _weaponName = weaponComponent.weaponName;
        enabled = true;
        this.dragable = dragable;
        DragableWeaponGUI.SetActive(true);
        SecondDragableWeaponGUI.SetActive(true);
        CancelInvoke(nameof(Close));
        Invoke(nameof(Close), 0.4f);
        WeaponBasicModel weaponModel = WeaponModel(sealedData.WeaponBasics);
        image.sprite = weaponModel.SpriteReference;
        name.text = weaponModel.Name;

        HolsterModel newHolster = new();
        newHolster.EquipedWeapon = new();
        newHolster.EquipedWeapon.ID = dragable.ID;
        newHolster.EquipedWeapon.weaponName = _weaponName;
        newHolster.EquipedWeapon.MagazineBulletCount = dragable.CurrentAmmoSize;
        newHolster.EquipedWeapon.Suppressor = dragable.SuppressorModel;
        newHolster.EquipedWeapon.Sight = dragable.SightModel;
        diler.Rebuild(newHolster, weaponModel, holsterManager,this, dragable);
    }

    public void Pick(int slotIndex)
    {
        HolsterModel slot = data.PlayerData.Holster[slotIndex];
        if (slot.IsLocked) return;

        Debug.Log("Pick");

        if (slot.IsOccupied)
        {
            if (data.PlayerData.SelectedWeaponIndex == slotIndex) holsterManager.WeaponThrow(slot, weaponHolster.currentWeapon.currentAmmo);
            else holsterManager.WeaponThrow(slot, slot.EquipedWeapon.MagazineBulletCount);
        }
        PickSoundEffect();
        Weapon weapon = dragable.Prefab.GetComponent<Weapon>();
        slot.EquipedWeapon.weaponName = weapon.weaponName;
        slot.EquipedWeapon.Suppressor = dragable.SuppressorModel;
        slot.EquipedWeapon.Sight = dragable.SightModel;
        slot.EquipedWeapon.MagazineBulletCount = dragable.CurrentAmmoSize;
        slot.EquipedWeapon.ID = dragable.ID;
        slot.IsOccupied = true;

        weapon.SetSight(slot.EquipedWeapon.Sight);
        weapon.SetSuppressor(slot.EquipedWeapon.Suppressor);

        holsterManager.RebuildFastHolster(data.PlayerData.Holster);
        holsterManager.RebuildWheelHolster(data.PlayerData.Holster);
        Destroy(dragable.gameObject);
        Close();
    }

    public void PickSoundEffect()
    {
        playerAudio.audio.PlayOneShot(playerAudio.CLIP_PICK_WEAPON, playerAudio.Volume);
    }

    public void PickToEmty()
    {
        int index = 0;
        foreach (HolsterModel slot in data.PlayerData.Holster)
        {
            index++;
            if(!slot.IsLocked && !slot.IsOccupied)
            {
                Debug.Log("Pick");
                playerAudio.audio.PlayOneShot(playerAudio.CLIP_PICK_WEAPON, playerAudio.Volume);
                Weapon weapon = dragable.Prefab.GetComponent<Weapon>();
                slot.EquipedWeapon.weaponName = weapon.weaponName;
                slot.EquipedWeapon.Suppressor = dragable.SuppressorModel;
                slot.EquipedWeapon.Sight = dragable.SightModel;
                slot.EquipedWeapon.MagazineBulletCount = dragable.CurrentAmmoSize;
                slot.EquipedWeapon.ID = dragable.ID;
                slot.IsOccupied = true;
                
                weapon.SetSight(slot.EquipedWeapon.Sight);
                weapon.SetSuppressor(slot.EquipedWeapon.Suppressor);

                holsterManager.RebuildFastHolster(data.PlayerData.Holster);
                holsterManager.RebuildWheelHolster(data.PlayerData.Holster);
                Destroy(dragable.gameObject);
                Close();
                break;
            }
            else
            {
                if(index == data.PlayerData.Holster.Count)
                {
                    //The slot is full
                    playerAudio.audio.PlayOneShot(playerAudio.CLIP_NO_SPACE_LEFT, playerAudio.Volume);
                }
            }
        }
    }

    public void Close()
    {
        enabled = false;
        DragableWeaponGUI.SetActive(false);
        SecondDragableWeaponGUI.SetActive(false);
    }

    WeaponBasicModel WeaponModel(List<WeaponBasicModel> list)
    {
        foreach (WeaponBasicModel item in list)
        {
            if (item.weaponName == _weaponName)
            {
                return item;
            }
        }
        return null;
    }
}
