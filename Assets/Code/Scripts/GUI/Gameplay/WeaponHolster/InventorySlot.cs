using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Data;

public class InvertorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerClickHandler
{
    [Range(0.1f, 1f)] public float holdDuration = 0.3f;
    public int SlotIndex;

    [Space]
    [Header("States")]
    public GameObject StateUnoccupied;
    public GameObject StateOccupied;
    public GameObject StateLocked;

    [Space]
    [Header("Attribute")]
    public Image Image;
    public TMP_Text Name;

    [HideInInspector] public bool IsOccupied;
    [HideInInspector] public bool IsLocked;
    [HideInInspector] public HolsterManager Manager;

    private bool isPressing = false;
    private bool isDragging = false;
    StartData data;
    WeaponBasicModel weaponBasic;
    [HideInInspector] public HolsterModel holsterModel;
    WeaponHolster weaponHolster;

    void Start()
    {
        data = FindFirstObjectByType<StartData>();
        weaponHolster = FindFirstObjectByType<WeaponHolster>();
    }

    public void Rebuild(HolsterModel holster, WeaponBasicModel weaponBasicModel, HolsterManager manager, Color SelectionColor)
    {
        SlotIndex = holster.Index;
        if(holster.EquipedWeapon.WeaponEnum != WeaponEnum.NONE) this.Image.sprite = weaponBasicModel.SpriteReference;
        if(holster.EquipedWeapon.WeaponEnum != WeaponEnum.NONE) this.Name.text = weaponBasicModel.Name;
        this.IsOccupied = holster.IsOccupied;
        this.IsLocked = holster.IsLocked;
        this.Manager = manager;
        this.holsterModel = holster;
        this.weaponBasic = weaponBasicModel;
        StateUnoccupied.SetActive(!IsOccupied && !IsLocked);
        StateOccupied.SetActive(IsOccupied && !IsLocked);
        StateLocked.SetActive(IsLocked);
        Image.color = SelectionColor;
        Name.color = SelectionColor;
    }

    void Drag()
    {
        if (IsOccupied && !IsLocked)
        {
            //Generate Object
            Manager.draggingObject = new GameObject("DraggingItem");
            RectTransform dragRect = Manager.draggingObject.AddComponent<RectTransform>();

            dragRect.SetParent(Manager.transform);

            dragRect.anchoredPosition = new(0, 0);
            dragRect.sizeDelta = new(275, 80);
            dragRect.anchorMin = new(0.5f, 0.5f);
            dragRect.anchorMax = new(0.5f, 0.5f);
            dragRect.localScale = new(0, 0, 0);

            Image image = Manager.draggingObject.AddComponent<Image>();
            image.color = Color.white;
            image.sprite = this.Image.sprite;
            image.raycastTarget = false;
            image.preserveAspect = true;

            //Disable Occupied
            StateOccupied.SetActive(false);
            StateUnoccupied.SetActive(false);
        }
    }

    void Update()
    {
        if (!isDragging || !IsOccupied) return;
        if (Manager.draggingObject.TryGetComponent(out RectTransform rect))
        {
            float canvasWidth = Manager.screenSize.x;
            float canvasHeight = Manager.screenSize.y;

            Vector2 mouseNormalized = new(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height);
            Vector2 generatedPosition = new(canvasWidth * mouseNormalized.x, canvasHeight * mouseNormalized.y);

            rect.anchoredPosition = new(generatedPosition.x - (canvasWidth / 2), generatedPosition.y - (canvasHeight / 2));
            rect.localScale = new(1, 1, 1);
        }
    }

    void Drop(PointerEventData eventData)
    {
        if(!isDragging) return;
        GameObject hitObject = eventData.pointerCurrentRaycast.gameObject;
        float SelectedIndex = data.PlayerData.SelectedWeaponIndex;
        if(hitObject == null)
        {
            Debug.Log("Throwed!");
            StateOccupied.SetActive(false);
            StateUnoccupied.SetActive(true);
            if (Manager.draggingObject) Destroy(Manager.draggingObject);

            if (data.PlayerData.SelectedWeaponIndex == SlotIndex) Manager.WeaponThrow(holsterModel, weaponHolster.currentWeapon.currentAmmo);
            else Manager.WeaponThrow(holsterModel, holsterModel.EquipedWeapon.MagazineBulletCount);
            Manager.RebuildFastHolster(data.PlayerData.Holster);
            Manager.RebuildWheelHolster(data.PlayerData.Holster);
            return;
        }

        if (hitObject.TryGetComponent(out InvertorySlot otherInventory))
        {
            if (!otherInventory.IsLocked)
            {
                //Drop

                if(otherInventory.IsOccupied == false)
                {
                    ///change
                    HolsterModel otherClone = new();
                    otherClone.Index = otherInventory.holsterModel.Index;
                    otherClone.IsOccupied = otherInventory.holsterModel.IsOccupied;
                    otherClone.IsLocked = otherInventory.holsterModel.IsLocked;

                    otherClone.EquipedWeapon = new();
                    otherClone.EquipedWeapon.ID = otherInventory.holsterModel.EquipedWeapon.ID;
                    otherClone.EquipedWeapon.WeaponEnum = otherInventory.holsterModel.EquipedWeapon.WeaponEnum;
                    otherClone.EquipedWeapon.MagazineBulletCount = otherInventory.holsterModel.EquipedWeapon.MagazineBulletCount;
                    otherClone.EquipedWeapon.Suppressed = otherInventory.holsterModel.EquipedWeapon.Suppressed;
                    otherClone.EquipedWeapon.Scoped = otherInventory.holsterModel.EquipedWeapon.Scoped;

                    otherInventory.holsterModel.Index = otherInventory.holsterModel.Index;
                    otherInventory.holsterModel.IsOccupied = holsterModel.IsOccupied;
                    otherInventory.holsterModel.IsLocked = holsterModel.IsLocked;
                    otherInventory.holsterModel.EquipedWeapon.ID = holsterModel.EquipedWeapon.ID;
                    otherInventory.holsterModel.EquipedWeapon.WeaponEnum = holsterModel.EquipedWeapon.WeaponEnum;
                    otherInventory.holsterModel.EquipedWeapon.MagazineBulletCount = holsterModel.EquipedWeapon.MagazineBulletCount;
                    otherInventory.holsterModel.EquipedWeapon.Suppressed = holsterModel.EquipedWeapon.Suppressed;
                    otherInventory.holsterModel.EquipedWeapon.Scoped = holsterModel.EquipedWeapon.Scoped;
                    
                    holsterModel.Index = holsterModel.Index;
                    holsterModel.IsOccupied = false;
                    holsterModel.IsLocked = holsterModel.IsLocked;
                    holsterModel.EquipedWeapon.ID = 0;
                    holsterModel.EquipedWeapon.WeaponEnum = WeaponEnum.NONE;
                    holsterModel.EquipedWeapon.MagazineBulletCount = 0;
                    holsterModel.EquipedWeapon.Suppressed = false;
                    holsterModel.EquipedWeapon.Scoped = false;
                }
                else
                {
                    ///change 2
                    HolsterModel otherClone = new();
                    otherClone.Index = otherInventory.holsterModel.Index;
                    otherClone.IsOccupied = otherInventory.holsterModel.IsOccupied;
                    otherClone.IsLocked = otherInventory.holsterModel.IsLocked;
                    otherClone.EquipedWeapon = new();
                    otherClone.EquipedWeapon.ID = otherInventory.holsterModel.EquipedWeapon.ID;
                    otherClone.EquipedWeapon.WeaponEnum = otherInventory.holsterModel.EquipedWeapon.WeaponEnum;
                    otherClone.EquipedWeapon.MagazineBulletCount = otherInventory.holsterModel.EquipedWeapon.MagazineBulletCount;
                    otherClone.EquipedWeapon.Suppressed = otherInventory.holsterModel.EquipedWeapon.Suppressed;
                    otherClone.EquipedWeapon.Scoped = otherInventory.holsterModel.EquipedWeapon.Scoped;

                    otherInventory.holsterModel.Index = otherInventory.holsterModel.Index;
                    otherInventory.holsterModel.IsOccupied = holsterModel.IsOccupied;
                    otherInventory.holsterModel.IsLocked = holsterModel.IsLocked;
                    otherInventory.holsterModel.EquipedWeapon.ID = holsterModel.EquipedWeapon.ID;
                    otherInventory.holsterModel.EquipedWeapon.WeaponEnum = holsterModel.EquipedWeapon.WeaponEnum;
                    otherInventory.holsterModel.EquipedWeapon.MagazineBulletCount = holsterModel.EquipedWeapon.MagazineBulletCount;
                    otherInventory.holsterModel.EquipedWeapon.Suppressed = holsterModel.EquipedWeapon.Suppressed;
                    otherInventory.holsterModel.EquipedWeapon.Scoped = holsterModel.EquipedWeapon.Scoped;
                    
                    holsterModel.Index = holsterModel.Index;
                    holsterModel.IsOccupied = true;
                    holsterModel.IsLocked = holsterModel.IsLocked;
                    holsterModel.EquipedWeapon.ID = otherClone.EquipedWeapon.ID;
                    holsterModel.EquipedWeapon.WeaponEnum = otherClone.EquipedWeapon.WeaponEnum;
                    holsterModel.EquipedWeapon.MagazineBulletCount = otherClone.EquipedWeapon.MagazineBulletCount;
                    holsterModel.EquipedWeapon.Suppressed = otherClone.EquipedWeapon.Suppressed;
                    holsterModel.EquipedWeapon.Scoped = otherClone.EquipedWeapon.Scoped;
                }
                Manager.RebuildFastHolster(data.PlayerData.Holster);
                Manager.RebuildWheelHolster(data.PlayerData.Holster);
                Debug.Log("Dropped: " + hitObject.name);
            }
            else
            {
                //Back
                Debug.Log("Undropped!");
                StateOccupied.SetActive(IsOccupied && !IsLocked);
            }
        }
        else
        {
            Debug.Log("Throwed!");
            StateOccupied.SetActive(false);
            StateUnoccupied.SetActive(true);
            if (Manager.draggingObject) Destroy(Manager.draggingObject);

            if (data.PlayerData.SelectedWeaponIndex == SlotIndex) Manager.WeaponThrow(holsterModel, weaponHolster.currentWeapon.currentAmmo);
            else Manager.WeaponThrow(holsterModel, holsterModel.EquipedWeapon.MagazineBulletCount);
            Manager.RebuildFastHolster(data.PlayerData.Holster);
            Manager.RebuildWheelHolster(data.PlayerData.Holster);
            return;
        }

        if (Manager.draggingObject) Destroy(Manager.draggingObject);
        Manager.RebuildFastHolster(data.PlayerData.Holster);
        Manager.RebuildWheelHolster(data.PlayerData.Holster);
    }




















    public void OnPointerClick(PointerEventData eventData)
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressing = true;
        Invoke(nameof(OnLongPress), holdDuration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelInvoke(nameof(OnLongPress));
        isPressing = false;
        Drop(eventData);
        isDragging = false;
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        float pointerDistance = Vector2.Distance(eventData.delta, Vector2.zero);
        if (pointerDistance >= 10)
        {
            isPressing = false;
        }
    }

    void OnLongPress()
    {
        if (isPressing)
        {
            isDragging = true;
            Drag();
        }
    }
}
