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
    private DataManager dataManager;
    WeaponBasicModel weaponBasic;
    [HideInInspector] public HolsterModel holsterModel;
    WeaponHolster weaponHolster;
    DragableWeapon dragableWeapon;
    Dragable dragable;

    void Start()
    {
        dataManager = FindFirstObjectByType<DataManager>();
        weaponHolster = FindFirstObjectByType<WeaponHolster>();
    }

    public void Rebuild(HolsterModel holster, WeaponBasicModel weaponBasicModel, HolsterManager manager, DragableWeapon dragableWeapon, Dragable dragable)
    {
        if(isDragging) return;
        if(holster.equipedWeapon.weaponName != WeaponName.NONE) this.Image.sprite = weaponBasicModel.spriteReference;
        if(holster.equipedWeapon.weaponName != WeaponName.NONE) this.Name.text = weaponBasicModel.name;
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
        float SelectedIndex = dataManager.playerModel.SelectedWeaponIndex;
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
                    otherInventory.holsterModel.isOccupied = true;
                    otherInventory.holsterModel.equipedWeapon.id = holsterModel.equipedWeapon.id;
                    otherInventory.holsterModel.equipedWeapon.weaponName = holsterModel.equipedWeapon.weaponName;
                    otherInventory.holsterModel.equipedWeapon.magazineBulletCount = holsterModel.equipedWeapon.magazineBulletCount;
                    otherInventory.holsterModel.equipedWeapon.suppressor = holsterModel.equipedWeapon.suppressor;
                    otherInventory.holsterModel.equipedWeapon.sight = holsterModel.equipedWeapon.sight;
                    dragableWeapon.PickSoundEffect();
                }
                else
                {
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

                    otherInventory.holsterModel.isOccupied = true;
                    otherInventory.holsterModel.equipedWeapon.id = holsterModel.equipedWeapon.id;
                    otherInventory.holsterModel.equipedWeapon.weaponName = holsterModel.equipedWeapon.weaponName;
                    otherInventory.holsterModel.equipedWeapon.magazineBulletCount = holsterModel.equipedWeapon.magazineBulletCount;
                    otherInventory.holsterModel.equipedWeapon.suppressor = holsterModel.equipedWeapon.suppressor;
                    otherInventory.holsterModel.equipedWeapon.sight = holsterModel.equipedWeapon.sight;
                    dragableWeapon.PickSoundEffect();
                    if (dataManager.playerModel.SelectedWeaponIndex == otherClone.index && weaponHolster.currentWeapon) Manager.WeaponThrow(otherClone, weaponHolster.currentWeapon.currentAmmo);
                    else Manager.WeaponThrow(otherClone, otherClone.equipedWeapon.magazineBulletCount);
                }
                Manager.RebuildFastHolster(dataManager.playerModel.Holster);
                Manager.RebuildWheelHolster(dataManager.playerModel.Holster);
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
        Manager.RebuildFastHolster(dataManager.playerModel.Holster);
        Manager.RebuildWheelHolster(dataManager.playerModel.Holster);
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
