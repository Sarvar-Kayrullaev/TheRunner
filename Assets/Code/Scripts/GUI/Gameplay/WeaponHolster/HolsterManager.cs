using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data;
using PlayerRoot;
using Shader;

public class HolsterManager : MonoBehaviour
{
    [Header("Params")] 
    [SerializeField] Color SelectedColor;
    [SerializeField] Color UnselectedColor;
    [Header("Fast Slot 1")]
    [SerializeField] GameObject Slot_1;
    [SerializeField] Image Slot_1_Image;
    [SerializeField] TMP_Text Slot_1_Text;
    [Header("Fast Slot 2")]
    [SerializeField] GameObject Slot_2;
    [SerializeField] Image Slot_2_Image;
    [SerializeField] TMP_Text Slot_2_Text;
    [Space]
    [SerializeField] Color Slot_SelectedColor;
    [SerializeField] Color Slot_UnselectedColor;
    [Space]
    [Header("Holster")]
    [SerializeField] GameObject SlotsParent;
    [SerializeField] GameObject HolsterScreen;
    [SerializeField] GameObject HolsterCursor;
    [HideInInspector] public GameObject draggingObject;
    [HideInInspector] public Vector2 screenSize;

    [Space]
    [Header("Other Objects")]
    [SerializeField] Transform WeaponThrowPoint;
    [SerializeField] GameObject PlayerCrosshair;

    private StartData data;
    private SealedData sealedData;
    private WeaponHolster weaponHolster;
    private Player player;
    private Animator animator;

    private int CurrentWeaponID;
    private bool holterEnabled = false;

    [System.Obsolete]
    public void Start()
    {
        data = FindObjectOfType<StartData>();
        sealedData = FindObjectOfType<SealedData>();
        weaponHolster = FindObjectOfType<WeaponHolster>();
        player = FindFirstObjectByType<Player>();
        if (HolsterScreen.TryGetComponent(out Animator animator)) this.animator = animator;
        RebuildFastHolster(data.PlayerData.Holster);
        RebuildWheelHolster(data.PlayerData.Holster);
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas.TryGetComponent(out RectTransform rect)) screenSize = rect.rect.size;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (HolsterScreen.activeInHierarchy)
            {
                animator.Play("HolsterClose");
                Invoke(nameof(Disable), 0.2f);
            }
            else
            {
                OpenHolster();
            }
        }
    }

    void Disable()
    {
        HolsterScreen.SetActive(false);
        holterEnabled = false;
        if(weaponHolster.currentWeapon)
        {
            PlayerCrosshair.SetActive(true);
        }else
        {
            PlayerCrosshair.SetActive(false);
        }
    }

    public void CloseHolster()
    {
        if (HolsterScreen.activeInHierarchy)
        {
            animator.Play("HolsterClose");
            Invoke(nameof(Disable), 0.2f);
        }
    }

    public void OpenHolster()
    {
        SaveCurrentWeaponParams();
        holterEnabled = true;
        HolsterScreen.SetActive(true);
        animator.Play("HolsterOpen");
        PlayerCrosshair.SetActive(false);
    }

    public void RebuildFastHolster(List<HolsterModel> slots)
    {
        int index = 0;
        int SelectedWeaponIndex = data.PlayerData.SelectedWeaponIndex;
        foreach (HolsterModel slot in slots)
        {
            //if (index > 1) return;
            index++;
            WeaponBasicModel weaponBasic = GetWeaponBasic(sealedData.WeaponBasics, slot.EquipedWeapon.WeaponEnum);
            bool noWeapon = slot.EquipedWeapon.WeaponEnum == WeaponEnum.NONE;
            bool isSelected = SelectedWeaponIndex == index - 1;
            if (isSelected & !noWeapon) weaponHolster.DrawWeapon(weaponBasic.WeaponPrefab);
            else if(isSelected & noWeapon) weaponHolster.DrawHand();

            if(isSelected & !noWeapon & !holterEnabled) PlayerCrosshair.SetActive(true);
            else if(isSelected & noWeapon) PlayerCrosshair.SetActive(false);

            if (index == 1)
            {
                if (Slot_1.TryGetComponent(out SetBlurUI setBlur)) setBlur.SetColor(isSelected & !noWeapon ? Slot_SelectedColor : Slot_UnselectedColor);
                if (noWeapon)
                {
                    Slot_1_Image.gameObject.SetActive(false);
                    Slot_1_Text.gameObject.SetActive(false);
                }
                else
                {
                    Slot_1_Image.gameObject.SetActive(true);
                    Slot_1_Text.gameObject.SetActive(true);

                    Slot_1_Image.sprite = weaponBasic.SpriteReference;
                    Slot_1_Text.text = weaponBasic.Name;
                }
            }
            else if (index == 2)
            {
                if (Slot_2.TryGetComponent(out SetBlurUI setBlur)) setBlur.SetColor(isSelected & !noWeapon ? Slot_SelectedColor : Slot_UnselectedColor);
                if (noWeapon)
                {
                    Slot_2_Image.gameObject.SetActive(false);
                    Slot_2_Text.gameObject.SetActive(false);
                }
                else
                {
                    Slot_2_Image.gameObject.SetActive(true);
                    Slot_2_Text.gameObject.SetActive(true);

                    Slot_2_Image.sprite = weaponBasic.SpriteReference;
                    Slot_2_Text.text = weaponBasic.Name;
                }
            }
        }
    }

    public void RebuildWheelHolster(List<HolsterModel> slots)
    {
        List<InvertorySlot> wheelSlots = WeaponWheelSlots();

        int index = 0;
        Color selectionColor;

        foreach (HolsterModel slot in slots)
        {
            if(index == data.PlayerData.SelectedWeaponIndex) selectionColor = SelectedColor;
            else selectionColor = UnselectedColor;
            wheelSlots[index].Rebuild(slot, GetWeaponBasic(sealedData.WeaponBasics, slot.EquipedWeapon.WeaponEnum), this, selectionColor);
            index++;
        }
        RebuildHolsterCursor();
    }

    public void RebuildHolsterCursor()
    {
        int SelectedIndex = data.PlayerData.SelectedWeaponIndex;
        if(SelectedIndex == 0) HolsterCursor.transform.eulerAngles = new (0,0,0);
        else if(SelectedIndex == 1) HolsterCursor.transform.eulerAngles = new (0,0,-90);
        else if(SelectedIndex == 2) HolsterCursor.transform.eulerAngles = new (0,0,180);
        if(SelectedIndex == 3) HolsterCursor.transform.eulerAngles = new (0,0,90);
    }
    public void Selector(int index)
    {
        SaveCurrentWeaponParams();
        HolsterModel slot = data.PlayerData.Holster[index];
        bool noWeapon = slot.EquipedWeapon.WeaponEnum == WeaponEnum.NONE;
        if (noWeapon) return;
        data.PlayerData.SelectedWeaponIndex = index;
        RebuildFastHolster(data.PlayerData.Holster);
        RebuildWheelHolster(data.PlayerData.Holster);
        CloseHolster();
    }

    public void SaveCurrentWeaponParams()
    {
        if (weaponHolster.currentWeapon)
        {
            EquipedWeaponModel equipedWeapon = data.PlayerData.Holster[data.PlayerData.SelectedWeaponIndex].EquipedWeapon;
            equipedWeapon.MagazineBulletCount = weaponHolster.currentWeapon.currentAmmo;
            equipedWeapon.Scoped = weaponHolster.currentWeapon.isScoped;
            equipedWeapon.Suppressed = weaponHolster.currentWeapon.isSilenced;
        }
    }

    public void WeaponThrow(HolsterModel holster, int CurrentAmmoSize)
    {
        WeaponBasicModel weaponBasic = GetWeaponBasic(sealedData.WeaponBasics, holster.EquipedWeapon.WeaponEnum);

        Quaternion initialRotation = WeaponThrowPoint.rotation;
        Quaternion randomRotation = Random.rotation;
        randomRotation = Quaternion.Euler(randomRotation.eulerAngles * 0.1f);
        Quaternion randomizedRotation = initialRotation * randomRotation;

        GameObject throwed = Instantiate(weaponBasic.DroppedWeaponPrefab, WeaponThrowPoint.position, randomizedRotation);
        if(throwed.TryGetComponent(out Rigidbody rigidbody)) rigidbody.AddForce(WeaponThrowPoint.forward*700);
        if (throwed.TryGetComponent(out Dragable dragable))
        {
            /*Set Dragable Attributes*/
            dragable.DragableType = DragableType.Weapon;
            dragable.Prefab = weaponBasic.WeaponPrefab;
            dragable.ID = holster.EquipedWeapon.ID;
            dragable.CurrentAmmoSize = CurrentAmmoSize;
            dragable.Bullets = 0;
            dragable.IsThrowed = true;
            dragable.Suppressed = holster.EquipedWeapon.Suppressed;
            dragable.Scoped = holster.EquipedWeapon.Scoped;
        }

        /*Reset Holster Data*/
        holster.IsOccupied = false;
        holster.EquipedWeapon.ID = 0;
        holster.EquipedWeapon.WeaponEnum = WeaponEnum.NONE;
        holster.EquipedWeapon.MagazineBulletCount = 0;
        holster.EquipedWeapon.Suppressed = false;
        holster.EquipedWeapon.Scoped = false;
    }

    List<InvertorySlot> WeaponWheelSlots()
    {
        List<InvertorySlot> list = new();
        foreach (Transform item in SlotsParent.transform)
        {
            if (item.TryGetComponent(out InvertorySlot component)) list.Add(component);
        }
        return list;
    }

    WeaponBasicModel GetWeaponBasic(List<WeaponBasicModel> WeaponBasics, WeaponEnum WeaponEnum)
    {
        foreach (WeaponBasicModel weaponBasic in WeaponBasics)
        {
            if (weaponBasic.WeaponEnum == WeaponEnum) return weaponBasic;
        }
        return null;
    }
}
