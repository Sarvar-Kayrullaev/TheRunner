using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Widget;
using Button = UnityEngine.UI.Button;
using Random = System.Random;

/// <summary>
/// ShopAdapter is responsible for managing the shop interface, displaying items based on their type.
/// It uses a SealedData instance to access weapon data and dynamically creates item views in the UI.
/// It clears the existing items in the weapon item parent before adding new ones.
/// </summary>
public class ShopAdapter : MonoBehaviour
{
    public SealedData sealedData;
    public DataManager dataManager;
    
    [Header("Fund Setup")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text goldText;

    [Header("Weapon Content Setup")]
    [SerializeField] private RectTransform weaponContentParent;
    [SerializeField] private GameObject contentPrefab;

    [Header("Weapon Stats Setup")]
    [SerializeField] private RectTransform weaponTitleTextParent;
    [SerializeField] private RectTransform weaponStatsBar1;
    [SerializeField] private RectTransform weaponStatsBar2;
    [SerializeField] private RectTransform weaponStatsBar3;
    [SerializeField] private RectTransform weaponStatsBar4;
    [SerializeField] private RectTransform weaponStatsBar5;
    [Space(10)]
    [SerializeField] private RectTransform weaponPriceTextParent;
    [Space(10)]
    [SerializeField] private TMP_Text weaponTitleText;
    [SerializeField]private TMP_Text weaponPriceText;
    [Space(10)]
    [SerializeField] private TMP_Text weaponStatsValueText1;
    [SerializeField] private TMP_Text weaponStatsValueText2;
    [SerializeField] private TMP_Text weaponStatsValueText3;
    [SerializeField] private TMP_Text weaponStatsValueText4;
    [SerializeField] private TMP_Text weaponStatsValueText5;
    [Space(10)]
    [SerializeField] private Slider weaponStatsSlider1;
    [SerializeField] private Slider weaponStatsSlider2;
    [SerializeField] private Slider weaponStatsSlider3;
    [SerializeField] private Slider weaponStatsSlider4;
    [SerializeField] private Slider weaponStatsSlider5;
    [Space(10)]
    [SerializeField] private ItemButton weaponBuyButtonComponent;
    [SerializeField] private ItemButton weaponEquipButtonComponent;
    [SerializeField] private ItemButton weaponCustomizeButtonComponent;
    [Space(10)]
    [Header("Equipment Setup")]
    [SerializeField] private RectTransform equipmentScreen;
    [SerializeField] private List<GUI.Button> slotsButton;
    [SerializeField] private List<Image> slotsImage;
    
    
     [Space(10)]
    [Header("Customization Setup")]
    [SerializeField] private RectTransform customizationScreen;
    [Space(10)]
    [Header("Buy Setup")]
    [SerializeField] private RectTransform buyScreen;
    [SerializeField] private RectTransform notEnoughMoneyScreen;
    [SerializeField] private Button buyButton;

    private void Awake()
    {
        StartCoroutine(Initialize());
    }
    
    private IEnumerator Initialize()
    {
        if (!sealedData)
        {
            sealedData = FindFirstObjectByType<SealedData>();
        }

        if (!dataManager)
        {
            dataManager = FindFirstObjectByType<DataManager>();
        }

        while (!sealedData || !dataManager || !dataManager.LoadingCompleted)
        {
            yield return null;
        }

        // how to wait until dataManager not null or loadData?
        
        BuildFundAdapter();
    }

    public void BuildWeaponAdapter(int weaponType)
    {
        Clear(weaponContentParent);
        var playerModel = dataManager.playerModel;
        
        var defaultSelectedFirst = false;
        var index = 0;

        var sortedWeapon = sealedData.WeaponBasics.OrderBy<WeaponBasicModel, object>(w => w.WeaponPrice).ToList();
        
        foreach (WeaponBasicModel model in sortedWeapon)
        {
            // Check if the model's type matches the specified weapon type
            
            if ((int)model.WeaponType == weaponType)
            {
                //show 
                var x = index;
                var itemView = Instantiate(contentPrefab, weaponContentParent);
                
                var shopWeapon = playerModel.ShopWeapons[(int) model.weaponName-1];
                itemView.transform.GetChild(2).gameObject.SetActive(shopWeapon is { IsUnlocked: true, IsPurchased: true });

                if (itemView.transform.GetChild(0).TryGetComponent(out Image image))
                {
                    image.sprite = model.SpriteReference;
                }
                
                if (itemView.TryGetComponent(out Widget.Button button))
                {
                    button.OnClick.RemoveAllListeners();
                    button.OnClick.AddListener((() => BuildWeaponStatsAdapter((int) model.weaponName, itemView.transform)));
                    button.OnClick.AddListener((() => SetSelectionVisualize(x)));
                    if (!defaultSelectedFirst)
                    {
                        button.OnClick.Invoke();
                        defaultSelectedFirst = true;
                    }
                }

                index++;
            }
        }
    }

    public void SetSelectionVisualize(int index)
    {
        int currentIndex = 0;
        foreach (RectTransform content in weaponContentParent)
        {
            if (currentIndex == index)
            {
                content.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                content.GetChild(1).gameObject.SetActive(false);
            }

            currentIndex++;
        }
    }

    private void BuildFundAdapter()
    {
        moneyText.text = dataManager.playerModel.Funds.Money.ToString();
        goldText.text = dataManager.playerModel.Funds.Gold.ToString();
    }
    
    public void BuildWeaponStatsAdapter(int weaponName, Transform content)
    {
        var maxDamage = 0;
        var maxAccuracy = 0.0f;
        var maxFireRate = 0.0f;
        var maxReloadTime = 0.0f;
        var maxMobility = 0.0f;
        var maxReloadSpeed = Mathf.Infinity;

        foreach (var weapon in sealedData.WeaponBasics)
        {
            if (weapon.WeaponAttribute.Damage > maxDamage) maxDamage = weapon.WeaponAttribute.Damage;
            if(weapon.WeaponAttribute.Accuracy > maxAccuracy) maxAccuracy = weapon.WeaponAttribute.Accuracy;
            if (weapon.WeaponAttribute.FireRate > maxFireRate) maxFireRate = weapon.WeaponAttribute.FireRate;
            if (weapon.WeaponAttribute.ReloadTime > maxReloadTime) maxReloadTime = weapon.WeaponAttribute.ReloadTime;
            if (weapon.WeaponAttribute.Mobility > maxMobility) maxMobility = weapon.WeaponAttribute.Mobility;
            if(maxReloadSpeed > maxReloadTime) maxReloadSpeed = maxReloadTime;
        }
        
        weaponBuyButtonComponent.OnClick.RemoveAllListeners();
        weaponCustomizeButtonComponent.OnClick.RemoveAllListeners();
        weaponEquipButtonComponent.OnClick.RemoveAllListeners();
        
        foreach (var weapon in sealedData.WeaponBasics)
        {
            var currentWeaponName = (int) weapon.weaponName;
            if (currentWeaponName == weaponName)
            {
                weaponTitleText.text = weapon.Name;
                weaponPriceText.text = weapon.WeaponPrice.ToString();

                var damage = weapon.WeaponAttribute.Damage;
                var accuracy = weapon.WeaponAttribute.Accuracy;
                var fireRate = weapon.WeaponAttribute.FireRate;
                var reloadTime = weapon.WeaponAttribute.ReloadTime;
                var mobility = weapon.WeaponAttribute.Mobility;

                weaponStatsValueText1.text = $"{damage}";
                weaponStatsValueText2.text = $"%{accuracy}";
                weaponStatsValueText3.text = $"{fireRate}/s";
                weaponStatsValueText4.text = $"{reloadTime}s";
                weaponStatsValueText5.text = $"{mobility}m/s";
                
                var damageValue = (float)damage / (float)maxDamage;
                var accuracyValue = accuracy / maxAccuracy;
                var fireRateValue = fireRate / maxFireRate;
                var reloadSpeedValue = maxReloadSpeed / reloadTime;
                var mobilityValue = mobility / maxMobility;
                
                weaponStatsSlider1.value = damageValue;
                weaponStatsSlider2.value = accuracyValue;
                weaponStatsSlider3.value = fireRateValue;
                weaponStatsSlider4.value = reloadSpeedValue;
                weaponStatsSlider5.value = mobilityValue;
                
                var playerModel = dataManager.playerModel;
                var shopWeaponModel = playerModel.ShopWeapons[currentWeaponName-1];
                
                if (shopWeaponModel.IsUnlocked)
                {
                    if (shopWeaponModel.IsPurchased)
                    {
                        weaponPriceTextParent.gameObject.SetActive(false);
                        weaponBuyButtonComponent.gameObject.SetActive(false);
                        weaponEquipButtonComponent.gameObject.SetActive(true);
                        weaponCustomizeButtonComponent.gameObject.SetActive(true);
                        
                        weaponEquipButtonComponent.OnClick.AddListener(() => BuildEquipmentScreen(weapon, true, new()));
                        weaponCustomizeButtonComponent.OnClick.AddListener(() => BuildCustomizeScreen(weapon));
                        
                        content.GetChild(2).gameObject.SetActive(true);
                        
                        
                    }
                    else
                    {
                        weaponPriceTextParent.gameObject.SetActive(true);
                        weaponBuyButtonComponent.gameObject.SetActive(true);
                        weaponEquipButtonComponent.gameObject.SetActive(false);
                        weaponCustomizeButtonComponent.gameObject.SetActive(false);

                        if (weapon.WeaponPrice <= playerModel.Funds.Money)
                        {
                            weaponBuyButtonComponent.OnClick.AddListener(() => BuildBuyScreen(weapon, content));
                        }
                        else
                        {
                            weaponBuyButtonComponent.OnClick.AddListener((() => notEnoughMoneyScreen.gameObject.SetActive(true)));
                        }
                    }
                }
                else
                {
                    weaponPriceTextParent.gameObject.SetActive(false);
                    weaponBuyButtonComponent.gameObject.SetActive(false);
                    weaponEquipButtonComponent.gameObject.SetActive(false);
                    weaponCustomizeButtonComponent.gameObject.SetActive(false);
                }
            }
        }
    }

    private void BuildEquipmentScreen(WeaponBasicModel basicModel, bool changeable, List<HolsterModel> fakeHolsterModels)
    {
        equipmentScreen.gameObject.SetActive(true);

        List<HolsterModel> holsterModels = new();
        if(changeable) holsterModels = dataManager.playerModel.Holster;
        else holsterModels = fakeHolsterModels;
        
        var applyButton = equipmentScreen.GetChild(1).GetChild(1).GetComponent<Button>();
        if(changeable) applyButton.onClick.RemoveAllListeners();

        int slotIndex = 0;
        foreach (var holsterModel in holsterModels)
        {
            if (holsterModel.IsLocked)
            {
                slotsButton[slotIndex].onClick.RemoveAllListeners();
                slotsButton[slotIndex].transform.GetChild(0).gameObject.SetActive(false);
                slotsButton[slotIndex].transform.GetChild(1).gameObject.SetActive(false);
                slotsButton[slotIndex].transform.GetChild(2).gameObject.SetActive(true);
            }
            else if (holsterModel.IsOccupied)
            {
                slotsButton[slotIndex].onClick.RemoveAllListeners();
                slotsButton[slotIndex].transform.GetChild(0).gameObject.SetActive(false);
                slotsButton[slotIndex].transform.GetChild(1).gameObject.SetActive(true);
                slotsButton[slotIndex].transform.GetChild(2).gameObject.SetActive(false);

                var image = slotsButton[slotIndex].transform.GetChild(1).GetChild(0).GetComponent<Image>();
                var currentBasicWeapon = sealedData.WeaponBasics[(int)holsterModel.EquipedWeapon.weaponName - 1];
                image.sprite = currentBasicWeapon.SpriteReference;

                if (changeable)
                {
                    var index = slotIndex;
                    slotsButton[slotIndex].onClick.AddListener(() => Add(index, basicModel));
                    slotsButton[slotIndex].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    slotsButton[slotIndex].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    slotsButton[slotIndex].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    slotsButton[slotIndex].transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
                }
            }
            else
            {
                slotsButton[slotIndex].onClick.RemoveAllListeners();
                slotsButton[slotIndex].transform.GetChild(0).gameObject.SetActive(true);
                slotsButton[slotIndex].transform.GetChild(1).gameObject.SetActive(false);
                slotsButton[slotIndex].transform.GetChild(2).gameObject.SetActive(false);

                if (changeable)
                {
                    var index = slotIndex;
                    slotsButton[slotIndex].onClick.AddListener(() => Add(index, basicModel));
                    slotsButton[slotIndex].transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    slotsButton[slotIndex].transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    slotsButton[slotIndex].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    slotsButton[slotIndex].transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
                }
            }

            slotIndex++;
        }
        
        return;

        void Add(int slotIndex, WeaponBasicModel  basicModel)
        {
            
            PlayerModel playerModel = dataManager.playerModel;
            EquipedWeaponModel equipedWeapon = new();
            HolsterModel slot = new();
            ShopWeaponModel shopWeapon =  playerModel.ShopWeapons[(int) basicModel.weaponName - 1];
            
            equipedWeapon.weaponName = basicModel.weaponName;
            equipedWeapon.Suppressor = shopWeapon.Suppressor;
            equipedWeapon.Sight = shopWeapon.Sight;
            equipedWeapon.MagazineBulletCount = basicModel.WeaponAttribute.MagazineSize;
            equipedWeapon.ID = UnityEngine.Random.Range(100000, 999999);

            slot.EquipedWeapon = equipedWeapon;
            slot.IsOccupied = true;
            
            applyButton.onClick.AddListener((() => Apply(slot, slotIndex)));
            
            List<HolsterModel> newHolster = dataManager.playerModel.Holster.ToList();
            newHolster[slotIndex] = slot;
            BuildEquipmentScreen(basicModel, false, newHolster);
        }

        void Apply(HolsterModel slot, int slotIndex)
        {
            PlayerModel playerModel = dataManager.playerModel;
            playerModel.Holster[slotIndex] = slot;
            DataProvider.SavePlayerData(playerModel);
        }
    }

    private void BuildCustomizeScreen(WeaponBasicModel basicModel)
    {
        
    }

    private void BuildBuyScreen(WeaponBasicModel basicModel, Transform content)
    {
        buyScreen.gameObject.SetActive(true);
        
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(Buy);
        return;

        void Buy()
        {
            var playerModel = dataManager.playerModel;
            foreach (var shopWeapon in playerModel.ShopWeapons)
            {
                if (shopWeapon.weaponName == basicModel.weaponName)
                {
                    var leftoverMoney = playerModel.Funds.Money - basicModel.WeaponPrice;
                    if (leftoverMoney >= 0)
                    {
                        playerModel.Funds.Money -= basicModel.WeaponPrice;
                        shopWeapon.IsPurchased = true;
                        Debug.Log($"Purchased: {shopWeapon.weaponName}    BasicModel: {basicModel.weaponName}");
                    }
                }
            }
            
            dataManager.UpdatePlayerModel(playerModel);
            BuildWeaponStatsAdapter((int)basicModel.weaponName, content);
            BuildFundAdapter();
        }
    }
    
    
    private static void Clear(Transform parent)
    {
        if (parent == null)
        {
            Debug.LogWarning("Parent RectTransform is null. Cannot clear items.");
            return;
        }
        var isEmpty = parent.childCount == 0;
        if (isEmpty)
        {
            return;
        }
        var counter = 0;
        var isComplete = false;
        
        while (isComplete == false)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
            counter++;
            if (counter > 100 || parent.childCount == 0)
            {
                isComplete = true;
            }
        }
        if (parent.childCount > 0)
        {
            Debug.LogWarning($"Clear loop stopped after 100 iterations. Some children may remain. Looped count: {counter}");
        }
    }


}
