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
        SlotIndex = holster.index;
        if(holster.equipedWeapon.weaponName != WeaponName.NONE) this.Image.sprite = weaponBasicModel.spriteReference;
        if(holster.equipedWeapon.weaponName != WeaponName.NONE) this.Name.text = weaponBasicModel.name;
        this.IsOccupied = holster.isOccupied;
        this.IsLocked = holster.isLocked;
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
            else Manager.WeaponThrow(holsterModel, holsterModel.equipedWeapon.magazineBulletCount);
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
                    otherClone.index = otherInventory.holsterModel.index;
                    otherClone.isOccupied = otherInventory.holsterModel.isOccupied;
                    otherClone.isLocked = otherInventory.holsterModel.isLocked;

                    otherClone.equipedWeapon = new();
                    otherClone.equipedWeapon.id = otherInventory.holsterModel.equipedWeapon.id;
                    otherClone.equipedWeapon.weaponName = otherInventory.holsterModel.equipedWeapon.weaponName;
                    otherClone.equipedWeapon.magazineBulletCount = otherInventory.holsterModel.equipedWeapon.magazineBulletCount;
                    otherClone.equipedWeapon.suppressor = otherInventory.holsterModel.equipedWeapon.suppressor;
                    otherClone.equipedWeapon.sight = otherInventory.holsterModel.equipedWeapon.sight;

                    otherInventory.holsterModel.index = otherInventory.holsterModel.index;
                    otherInventory.holsterModel.isOccupied = holsterModel.isOccupied;
                    otherInventory.holsterModel.isLocked = holsterModel.isLocked;
                    otherInventory.holsterModel.equipedWeapon.id = holsterModel.equipedWeapon.id;
                    otherInventory.holsterModel.equipedWeapon.weaponName = holsterModel.equipedWeapon.weaponName;
                    otherInventory.holsterModel.equipedWeapon.magazineBulletCount = holsterModel.equipedWeapon.magazineBulletCount;
                    otherInventory.holsterModel.equipedWeapon.suppressor = holsterModel.equipedWeapon.suppressor;
                    otherInventory.holsterModel.equipedWeapon.sight = holsterModel.equipedWeapon.sight;
                    
                    holsterModel.index = holsterModel.index;
                    holsterModel.isOccupied = false;
                    holsterModel.isLocked = holsterModel.isLocked;
                    holsterModel.equipedWeapon.id = 0;
                    holsterModel.equipedWeapon.weaponName = WeaponName.NONE;
                    holsterModel.equipedWeapon.magazineBulletCount = 0;
                    holsterModel.equipedWeapon.suppressor = new SuppressorModel();
                    holsterModel.equipedWeapon.sight = new SightModel();
                }
                else
                {
                    ///change 2
                    HolsterModel otherClone = new();
                    otherClone.index = otherInventory.holsterModel.index;
                    otherClone.isOccupied = otherInventory.holsterModel.isOccupied;
                    otherClone.isLocked = otherInventory.holsterModel.isLocked;
                    otherClone.equipedWeapon = new();
                    otherClone.equipedWeapon.id = otherInventory.holsterModel.equipedWeapon.id;
                    otherClone.equipedWeapon.weaponName = otherInventory.holsterModel.equipedWeapon.weaponName;
                    otherClone.equipedWeapon.magazineBulletCount = otherInventory.holsterModel.equipedWeapon.magazineBulletCount;
                    otherClone.equipedWeapon.suppressor = otherInventory.holsterModel.equipedWeapon.suppressor;
                    otherClone.equipedWeapon.sight = otherInventory.holsterModel.equipedWeapon.sight;

                    otherInventory.holsterModel.index = otherInventory.holsterModel.index;
                    otherInventory.holsterModel.isOccupied = holsterModel.isOccupied;
                    otherInventory.holsterModel.isLocked = holsterModel.isLocked;
                    otherInventory.holsterModel.equipedWeapon.id = holsterModel.equipedWeapon.id;
                    otherInventory.holsterModel.equipedWeapon.weaponName = holsterModel.equipedWeapon.weaponName;
                    otherInventory.holsterModel.equipedWeapon.magazineBulletCount = holsterModel.equipedWeapon.magazineBulletCount;
                    otherInventory.holsterModel.equipedWeapon.suppressor = holsterModel.equipedWeapon.suppressor;
                    otherInventory.holsterModel.equipedWeapon.sight = holsterModel.equipedWeapon.sight;
                    
                    holsterModel.index = holsterModel.index;
                    holsterModel.isOccupied = true;
                    holsterModel.isLocked = holsterModel.isLocked;
                    holsterModel.equipedWeapon.id = otherClone.equipedWeapon.id;
                    holsterModel.equipedWeapon.weaponName = otherClone.equipedWeapon.weaponName;
                    holsterModel.equipedWeapon.magazineBulletCount = otherClone.equipedWeapon.magazineBulletCount;
                    holsterModel.equipedWeapon.suppressor = otherClone.equipedWeapon.suppressor;
                    holsterModel.equipedWeapon.sight = otherClone.equipedWeapon.sight;
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
            else Manager.WeaponThrow(holsterModel, holsterModel.equipedWeapon.magazineBulletCount);
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
