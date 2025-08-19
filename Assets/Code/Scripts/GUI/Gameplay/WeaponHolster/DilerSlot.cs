using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Data;

public class DilerSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler, IPointerClickHandler
{
    [Range(0.1f, 1f)] public float holdDuration = 0.3f;
    [HideInInspector] public HolsterManager Manager;

    public Image Image;
    public TMP_Text Name;

    public RectTransform StateOccupiedTransform;
    public RectTransform StateUnoccupiedTransform;

    private bool isPressing = false;
    public bool isDragging = false;
    StartData data;
    WeaponBasicModel weaponBasic;
    [HideInInspector] public HolsterModel holsterModel;
    WeaponHolster weaponHolster;
    DragableWeapon dragableWeapon;
    Dragable dragable;

    void Start()
    {
        data = FindFirstObjectByType<StartData>();
        weaponHolster = FindFirstObjectByType<WeaponHolster>();
    }

    public void Rebuild(HolsterModel holster, WeaponBasicModel weaponBasicModel, HolsterManager manager, DragableWeapon dragableWeapon, Dragable dragable)
    {
        if(isDragging) return;
        if(holster.EquipedWeapon.weaponName != WeaponName.NONE) this.Image.sprite = weaponBasicModel.SpriteReference;
        if(holster.EquipedWeapon.weaponName != WeaponName.NONE) this.Name.text = weaponBasicModel.Name;
        this.Manager = manager;
        this.holsterModel = holster;
        this.weaponBasic = weaponBasicModel;
        this.dragableWeapon = dragableWeapon;
        this.dragable = dragable;
        StateOccupiedTransform.gameObject.SetActive(true);
        StateUnoccupiedTransform.gameObject.SetActive(false);
    }

    void Drag()
    {
        if (true)
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
            StateOccupiedTransform.gameObject.SetActive(false);
            StateUnoccupiedTransform.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (!isDragging) return;
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
            Debug.Log("No Picked");
            if (Manager.draggingObject) Destroy(Manager.draggingObject);
            StateOccupiedTransform.gameObject.SetActive(true);
            StateOccupiedTransform.gameObject.SetActive(false);
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
                    otherInventory.holsterModel.IsOccupied = true;
                    otherInventory.holsterModel.EquipedWeapon.ID = holsterModel.EquipedWeapon.ID;
                    otherInventory.holsterModel.EquipedWeapon.weaponName = holsterModel.EquipedWeapon.weaponName;
                    otherInventory.holsterModel.EquipedWeapon.MagazineBulletCount = holsterModel.EquipedWeapon.MagazineBulletCount;
                    otherInventory.holsterModel.EquipedWeapon.Suppressor = holsterModel.EquipedWeapon.Suppressor;
                    otherInventory.holsterModel.EquipedWeapon.Sight = holsterModel.EquipedWeapon.Sight;
                    dragableWeapon.PickSoundEffect();
                }
                else
                {
                    HolsterModel otherClone = new();
                    otherClone.Index = otherInventory.holsterModel.Index;
                    otherClone.IsOccupied = otherInventory.holsterModel.IsOccupied;
                    otherClone.IsLocked = otherInventory.holsterModel.IsLocked;

                    otherClone.EquipedWeapon = new();
                    otherClone.EquipedWeapon.ID = otherInventory.holsterModel.EquipedWeapon.ID;
                    otherClone.EquipedWeapon.weaponName = otherInventory.holsterModel.EquipedWeapon.weaponName;
                    otherClone.EquipedWeapon.MagazineBulletCount = otherInventory.holsterModel.EquipedWeapon.MagazineBulletCount;
                    otherClone.EquipedWeapon.Suppressor = otherInventory.holsterModel.EquipedWeapon.Suppressor;
                    otherClone.EquipedWeapon.Sight = otherInventory.holsterModel.EquipedWeapon.Sight;

                    otherInventory.holsterModel.IsOccupied = true;
                    otherInventory.holsterModel.EquipedWeapon.ID = holsterModel.EquipedWeapon.ID;
                    otherInventory.holsterModel.EquipedWeapon.weaponName = holsterModel.EquipedWeapon.weaponName;
                    otherInventory.holsterModel.EquipedWeapon.MagazineBulletCount = holsterModel.EquipedWeapon.MagazineBulletCount;
                    otherInventory.holsterModel.EquipedWeapon.Suppressor = holsterModel.EquipedWeapon.Suppressor;
                    otherInventory.holsterModel.EquipedWeapon.Sight = holsterModel.EquipedWeapon.Sight;
                    dragableWeapon.PickSoundEffect();
                    if (data.PlayerData.SelectedWeaponIndex == otherClone.Index && weaponHolster.currentWeapon) Manager.WeaponThrow(otherClone, weaponHolster.currentWeapon.currentAmmo);
                    else Manager.WeaponThrow(otherClone, otherClone.EquipedWeapon.MagazineBulletCount);
                }
                Manager.RebuildFastHolster(data.PlayerData.Holster);
                Manager.RebuildWheelHolster(data.PlayerData.Holster);
                Destroy(dragable.gameObject);
                dragableWeapon.Close();
                Debug.Log("Dropped: " + hitObject.name);
            }
            else
            {
                //Back
                Debug.Log("No Picked");
                if (Manager.draggingObject) Destroy(Manager.draggingObject);
                StateOccupiedTransform.gameObject.SetActive(true);
                StateUnoccupiedTransform.gameObject.SetActive(false);
            }
        }
        else
        {
            if (Manager.draggingObject) Destroy(Manager.draggingObject);
            Debug.Log("No Picked");
            StateOccupiedTransform.gameObject.SetActive(true);
            StateUnoccupiedTransform.gameObject.SetActive(false);
            return;
        }

        if (Manager.draggingObject) Destroy(Manager.draggingObject);
        Manager.RebuildFastHolster(data.PlayerData.Holster);
        Manager.RebuildWheelHolster(data.PlayerData.Holster);
    }

















    bool onClick = true;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(onClick)
        {
            dragableWeapon.PickToEmty();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        onClick = true;
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
        onClick = false;
        if (isPressing)
        {
            isDragging = true;
            Drag();
        }
    }
}
