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
    
    private DataManager dataManager;
    private SealedData sealedData;
    private WeaponHolster weaponHolster;
    private Player player;
    private Animator animator;

    private int CurrentWeaponID;
    private bool holterEnabled = false;

    [System.Obsolete]
    public void Start()
    {
        dataManager = FindFirstObjectByType<DataManager>();
        sealedData = FindFirstObjectByType<SealedData>();
        weaponHolster = FindFirstObjectByType<WeaponHolster>();
        player = FindFirstObjectByType<Player>();
        if (HolsterScreen.TryGetComponent(out Animator animator)) this.animator = animator;
        RebuildFastHolster(dataManager.playerModel.Holster);
        RebuildWheelHolster(dataManager.playerModel.Holster);
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas.TryGetComponent(out RectTransform rect)) screenSize = rect.rect.size;
        
        Debug.Log(screenSize);
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
        int SelectedWeaponIndex = dataManager.playerModel.SelectedWeaponIndex;
        foreach (HolsterModel slot in slots)
        {
            //if (index > 1) return;
            index++;
            WeaponBasicModel weaponBasic = null;
            
            if (slot.EquipedWeapon.weaponName == WeaponName.NONE)
            {
                weaponBasic = new WeaponBasicModel();
            }
            else
            {
                weaponBasic = sealedData.WeaponBasics[(int)slot.EquipedWeapon.weaponName -1];
            }
            
            Debug.Log($"WeaponBasic: {weaponBasic.Name}     Type: {slot.EquipedWeapon.weaponName}");
            
            bool noWeapon = slot.EquipedWeapon.weaponName == WeaponName.NONE;
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
            if(index == dataManager.playerModel.SelectedWeaponIndex) selectionColor = SelectedColor;
            else selectionColor = UnselectedColor;
            wheelSlots[index].Rebuild(slot, GetWeaponBasic(sealedData.WeaponBasics, slot.EquipedWeapon.weaponName), this, selectionColor);
            index++;
        }
        RebuildHolsterCursor();
    }

    public void RebuildHolsterCursor()
    {
        int SelectedIndex = dataManager.playerModel.SelectedWeaponIndex;
        if(SelectedIndex == 0) HolsterCursor.transform.eulerAngles = new (0,0,0);
        else if(SelectedIndex == 1) HolsterCursor.transform.eulerAngles = new (0,0,-90);
        else if(SelectedIndex == 2) HolsterCursor.transform.eulerAngles = new (0,0,180);
        if(SelectedIndex == 3) HolsterCursor.transform.eulerAngles = new (0,0,90);
    }
    public void Selector(int index)
    {
        SaveCurrentWeaponParams();
        HolsterModel slot = dataManager.playerModel.Holster[index];
        bool noWeapon = slot.EquipedWeapon.weaponName == WeaponName.NONE;
        if (noWeapon) return;
        dataManager.playerModel.SelectedWeaponIndex = index;
        RebuildFastHolster(dataManager.playerModel.Holster);
        RebuildWheelHolster(dataManager.playerModel.Holster);
        CloseHolster();
    }

    public void SaveCurrentWeaponParams()
    {
        if (weaponHolster.currentWeapon)
        {
            EquipedWeaponModel equipedWeapon = dataManager.playerModel.Holster[dataManager.playerModel.SelectedWeaponIndex].EquipedWeapon;
            equipedWeapon.MagazineBulletCount = weaponHolster.currentWeapon.currentAmmo;
            equipedWeapon.Sight = weaponHolster.currentWeapon.sightModel;
            equipedWeapon.Suppressor = weaponHolster.currentWeapon.suppressorModel;
        }
    }

    public void WeaponThrow(HolsterModel holster, int CurrentAmmoSize)
    {
        WeaponBasicModel weaponBasic = GetWeaponBasic(sealedData.WeaponBasics, holster.EquipedWeapon.weaponName);

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
            dragable.SuppressorModel = holster.EquipedWeapon.Suppressor;
            dragable.SightModel = holster.EquipedWeapon.Sight;
        }

        /*Reset Holster Data*/
        holster.IsOccupied = false;
        holster.EquipedWeapon.ID = 0;
        holster.EquipedWeapon.weaponName = WeaponName.NONE;
        holster.EquipedWeapon.MagazineBulletCount = 0;
        holster.EquipedWeapon.Suppressor = new SuppressorModel();
        holster.EquipedWeapon.Sight = new SightModel();
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

    WeaponBasicModel GetWeaponBasic(List<WeaponBasicModel> WeaponBasics, WeaponName weaponName)
    {
        foreach (WeaponBasicModel weaponBasic in WeaponBasics)
        {
            if (weaponBasic.weaponName == weaponName) return weaponBasic;
        }
        return null;
    }
}
