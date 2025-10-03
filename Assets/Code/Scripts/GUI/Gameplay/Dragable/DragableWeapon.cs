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
    DataManager dataManager;
    WeaponHolster weaponHolster;
    WeaponName _weaponName;
    PlayerAudio playerAudio;
    DilerSlot diler;
    Weapon weaponComponent;

    [Space]
    [Header("Setup")]
    [SerializeField] Image image;
    [SerializeField] new TMP_Text name;

    void Awake()
    {
        dataManager = FindFirstObjectByType<DataManager>();
        sealedData = FindFirstObjectByType<SealedData>();
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
        if (dragable.prefab.TryGetComponent(out Weapon _weaponComponent)) weaponComponent = _weaponComponent;
        
        _weaponName = weaponComponent.weaponName;
        enabled = true;
        this.dragable = dragable;
        DragableWeaponGUI.SetActive(true);
        SecondDragableWeaponGUI.SetActive(true);
        CancelInvoke(nameof(Close));
        Invoke(nameof(Close), 0.4f);
        WeaponBasicModel weaponModel = WeaponModel(sealedData.WeaponBasics);
        image.sprite = weaponModel.spriteReference;
        name.text = weaponModel.name;

        HolsterModel newHolster = new();
        newHolster.equipedWeapon = new();
        newHolster.equipedWeapon.id = dragable.id;
        newHolster.equipedWeapon.weaponName = _weaponName;
        newHolster.equipedWeapon.magazineBulletCount = dragable.currentAmmoSize;
        newHolster.equipedWeapon.suppressor = dragable.suppressorModel;
        newHolster.equipedWeapon.sight = dragable.sightModel;
        diler.Rebuild(newHolster, weaponModel, holsterManager,this, dragable);
    }

    public void Pick(int slotIndex)
    {
        HolsterModel slot = dataManager.playerModel.Holster[slotIndex];
        if (slot.isLocked) return;

        Debug.Log("Pick");

        if (slot.isOccupied)
        {
            if (dataManager.playerModel.SelectedWeaponIndex == slotIndex) holsterManager.WeaponThrow(slot, weaponHolster.currentWeapon.currentAmmo);
            else holsterManager.WeaponThrow(slot, slot.equipedWeapon.magazineBulletCount);
        }
        PickSoundEffect();
        Weapon weapon = dragable.prefab.GetComponent<Weapon>();
        slot.equipedWeapon.weaponName = weapon.weaponName;
        slot.equipedWeapon.suppressor = dragable.suppressorModel;
        slot.equipedWeapon.sight = dragable.sightModel;
        slot.equipedWeapon.magazineBulletCount = dragable.currentAmmoSize;
        slot.equipedWeapon.id = dragable.id;
        slot.isOccupied = true;

        weapon.SetSight(slot.equipedWeapon.sight);
        weapon.SetSuppressor(slot.equipedWeapon.suppressor);

        holsterManager.RebuildFastHolster(dataManager.playerModel.Holster);
        holsterManager.RebuildWheelHolster(dataManager.playerModel.Holster);
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
        foreach (HolsterModel slot in dataManager.playerModel.Holster)
        {
            index++;
            if(!slot.isLocked && !slot.isOccupied)
            {
                Debug.Log("Pick");
                playerAudio.audio.PlayOneShot(playerAudio.CLIP_PICK_WEAPON, playerAudio.Volume);
                Weapon weapon = dragable.prefab.GetComponent<Weapon>();
                slot.equipedWeapon.weaponName = weapon.weaponName;
                slot.equipedWeapon.suppressor = dragable.suppressorModel;
                slot.equipedWeapon.sight = dragable.sightModel;
                slot.equipedWeapon.magazineBulletCount = dragable.currentAmmoSize;
                slot.equipedWeapon.id = dragable.id;
                slot.isOccupied = true;
                dataManager.playerModel.Holster[index-1] = slot;
                
                dataManager.UpdatePlayerModel(dataManager.playerModel); // Data Saved
                
                weapon.SetSight(slot.equipedWeapon.sight);
                weapon.SetSuppressor(slot.equipedWeapon.suppressor);

                holsterManager.RebuildFastHolster(dataManager.playerModel.Holster);
                holsterManager.RebuildWheelHolster(dataManager.playerModel.Holster);
                Destroy(dragable.gameObject);
                Close();
                break;
            }
            else
            {
                if(index == dataManager.playerModel.Holster.Count)
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
