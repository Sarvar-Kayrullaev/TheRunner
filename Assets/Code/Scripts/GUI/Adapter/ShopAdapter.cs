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

    [Header("Fund Setup")] [SerializeField]
    private TMP_Text moneyText;

    [SerializeField] private TMP_Text goldText;
    [Space] [SerializeField] private TMP_Text moneyTextCustomization;

    [Header("Weapon Content Setup")] [SerializeField]
    private RectTransform weaponContentParent;

    [SerializeField] private GameObject contentPrefab;

    [Header("Weapon Stats Setup")] [SerializeField]
    private RectTransform weaponTitleTextParent;

    [SerializeField] private RectTransform weaponDescriptionParent;

    [SerializeField] private RectTransform weaponStatsBar1;
    [SerializeField] private RectTransform weaponStatsBar2;
    [SerializeField] private RectTransform weaponStatsBar3;
    [SerializeField] private RectTransform weaponStatsBar4;
    [SerializeField] private RectTransform weaponStatsBar5;
    [Space(10)] [SerializeField] private RectTransform weaponPriceTextParent;
    [Space(10)] [SerializeField] private TMP_Text weaponTitleText;
    [SerializeField] private TMP_Text weaponDescriptionText;
    [SerializeField] private TMP_Text weaponPriceText;
    [Space(10)] [SerializeField] private TMP_Text weaponStatsValueText1;
    [SerializeField] private TMP_Text weaponStatsValueText2;
    [SerializeField] private TMP_Text weaponStatsValueText3;
    [SerializeField] private TMP_Text weaponStatsValueText4;
    [SerializeField] private TMP_Text weaponStatsValueText5;
    [Space(10)] [SerializeField] private Slider weaponStatsSlider1;
    [SerializeField] private Slider weaponStatsSlider2;
    [SerializeField] private Slider weaponStatsSlider3;
    [SerializeField] private Slider weaponStatsSlider4;
    [SerializeField] private Slider weaponStatsSlider5;
    [Space(10)] [SerializeField] private ItemButton weaponBuyButtonComponent;
    [SerializeField] private ItemButton weaponEquipButtonComponent;
    [SerializeField] private ItemButton weaponCustomizeButtonComponent;

    [Space(10)] [Header("Equipment Setup")] [SerializeField]
    private RectTransform equipmentScreen;

    [SerializeField] private List<GUI.Button> slotsButton;
    [SerializeField] private List<Image> slotsImage;


    [Space(10)] [Header("Customization Setup")] [SerializeField]
    private RectTransform customizationScreen;

    [SerializeField] private RectTransform customizationSightContentParent;
    [SerializeField] private RectTransform customizationSuppressorContentParent;
    [SerializeField] private RectTransform customizationMagazineContentParent;
    [SerializeField] private RectTransform customizationSkinContentParent;
    [SerializeField] private Button customizationBuyButton;
    [SerializeField] private Button customizationApplyButton;
    [SerializeField] private Button customizationAttachButton;
    [SerializeField] private Button customizationCancelButton;
    [SerializeField] private TMP_Text customizationDescriptionText;
    [SerializeField] private TMP_Text customizationPriceText;
    [SerializeField] private GameObject customizationContentPrefab;
    [SerializeField] private NavigationBar customizationNavigationBar;
    [SerializeField] private Sprite customizationPurchasedContentSprite;
    [SerializeField] private Sprite customizationNotPurchasedContentSprite;
    [SerializeField] private Sprite customizationAttachedContentSprite;
    [SerializeField] private Color customizationSelectedContentColor;
    [SerializeField] private Color customizationPurchasedContentColor;
    [SerializeField] private Color customizationNotPurchasedContentColor;
    [SerializeField] private Color customizationAttachedContentColor;

    [Space(10)] [Header("Buy Setup")] [SerializeField]
    private RectTransform buyScreen;

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

        var sortedWeapon = sealedData.WeaponBasics.OrderBy<WeaponBasicModel, object>(w => w.weaponPrice).ToList();

        foreach (WeaponBasicModel model in sortedWeapon)
        {
            // Check if the model's type matches the specified weapon type

            if ((int)model.weaponType == weaponType)
            {
                //show 
                var x = index;
                var itemView = Instantiate(contentPrefab, weaponContentParent);

                var shopWeapon = playerModel.ShopWeapons[(int)model.weaponName - 1];
                //itemView.transform.GetChild(2).gameObject.SetActive(shopWeapon is { IsUnlocked: true, IsPurchased: true });

                if (itemView.transform.GetChild(1).TryGetComponent(out Image image))
                {
                    image.sprite = model.spriteReference;
                }

                if (itemView.TryGetComponent(out Widget.Button button))
                {
                    button.OnClick.RemoveAllListeners();
                    button.OnClick.AddListener(
                        (() => BuildWeaponStatsAdapter((int)model.weaponName, itemView.transform)));
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
                content.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                content.GetChild(0).gameObject.SetActive(false);
            }

            currentIndex++;
        }
    }

    private void BuildFundAdapter()
    {
        moneyText.text = dataManager.playerModel.Funds.money.ToString();
        goldText.text = dataManager.playerModel.Funds.gold.ToString();
        moneyTextCustomization.text = dataManager.playerModel.Funds.money.ToString();
        SetPreferredSizeWidth(moneyTextCustomization);
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
            if (weapon.weaponAttribute.Damage > maxDamage) maxDamage = weapon.weaponAttribute.Damage;
            if (weapon.weaponAttribute.Accuracy > maxAccuracy) maxAccuracy = weapon.weaponAttribute.Accuracy;
            if (weapon.weaponAttribute.FireRate > maxFireRate) maxFireRate = weapon.weaponAttribute.FireRate;
            if (weapon.weaponAttribute.ReloadTime > maxReloadTime) maxReloadTime = weapon.weaponAttribute.ReloadTime;
            if (weapon.weaponAttribute.Mobility > maxMobility) maxMobility = weapon.weaponAttribute.Mobility;
            if (maxReloadSpeed > maxReloadTime) maxReloadSpeed = maxReloadTime;
        }

        weaponBuyButtonComponent.OnClick.RemoveAllListeners();
        weaponCustomizeButtonComponent.OnClick.RemoveAllListeners();
        weaponEquipButtonComponent.OnClick.RemoveAllListeners();

        foreach (var weapon in sealedData.WeaponBasics)
        {
            var currentWeaponName = (int)weapon.weaponName;
            if (currentWeaponName == weaponName)
            {
                weaponTitleText.text = weapon.name;
                weaponDescriptionText.text = weapon.description;
                if (weaponDescriptionText.TryGetComponent(out TextAdapter textAdapter))
                {
                    textAdapter.FixParentAspect(weaponDescriptionText);
                }

                weaponPriceText.text = weapon.weaponPrice.ToString();

                var damage = weapon.weaponAttribute.Damage;
                var accuracy = weapon.weaponAttribute.Accuracy;
                var fireRate = weapon.weaponAttribute.FireRate;
                var reloadTime = weapon.weaponAttribute.ReloadTime;
                var mobility = weapon.weaponAttribute.Mobility;

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
                var shopWeaponModel = playerModel.ShopWeapons[currentWeaponName - 1];

                if (shopWeaponModel.isUnlocked)
                {
                    if (shopWeaponModel.isPurchased)
                    {
                        weaponPriceTextParent.gameObject.SetActive(false);
                        weaponBuyButtonComponent.gameObject.SetActive(false);
                        weaponEquipButtonComponent.gameObject.SetActive(true);
                        weaponCustomizeButtonComponent.gameObject.SetActive(true);

                        weaponEquipButtonComponent.OnClick.AddListener(() => BuildEquipmentScreen(weapon, true, new()));
                        weaponCustomizeButtonComponent.OnClick.AddListener(() =>
                            BuildCustomizeScreen(weapon));

                        //content.GetChild(2).gameObject.SetActive(true);
                    }
                    else
                    {
                        weaponPriceTextParent.gameObject.SetActive(true);
                        weaponBuyButtonComponent.gameObject.SetActive(true);
                        weaponEquipButtonComponent.gameObject.SetActive(false);
                        weaponCustomizeButtonComponent.gameObject.SetActive(false);

                        if (weapon.weaponPrice <= playerModel.Funds.money)
                        {
                            weaponBuyButtonComponent.OnClick.AddListener(() => BuildBuyScreen(weapon, content));
                        }
                        else
                        {
                            weaponBuyButtonComponent.OnClick.AddListener((() =>
                                notEnoughMoneyScreen.gameObject.SetActive(true)));
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

    private void BuildEquipmentScreen(WeaponBasicModel basicModel, bool changeable,
        List<HolsterModel> fakeHolsterModels)
    {
        equipmentScreen.gameObject.SetActive(true);

        List<HolsterModel> holsterModels = new();
        if (changeable) holsterModels = dataManager.playerModel.Holster;
        else holsterModels = fakeHolsterModels;

        var applyButton = equipmentScreen.GetChild(1).GetChild(1).GetComponent<Button>();
        if (changeable) applyButton.onClick.RemoveAllListeners();

        int slotIndex = 0;
        foreach (var holsterModel in holsterModels)
        {
            if (holsterModel.isLocked)
            {
                slotsButton[slotIndex].onClick.RemoveAllListeners();
                slotsButton[slotIndex].transform.GetChild(0).gameObject.SetActive(false);
                slotsButton[slotIndex].transform.GetChild(1).gameObject.SetActive(false);
                slotsButton[slotIndex].transform.GetChild(2).gameObject.SetActive(true);
            }
            else if (holsterModel.isOccupied)
            {
                slotsButton[slotIndex].onClick.RemoveAllListeners();
                slotsButton[slotIndex].transform.GetChild(0).gameObject.SetActive(false);
                slotsButton[slotIndex].transform.GetChild(1).gameObject.SetActive(true);
                slotsButton[slotIndex].transform.GetChild(2).gameObject.SetActive(false);

                var image = slotsButton[slotIndex].transform.GetChild(1).GetChild(0).GetComponent<Image>();
                var currentBasicWeapon = sealedData.WeaponBasics[(int)holsterModel.equipedWeapon.weaponName - 1];
                image.sprite = currentBasicWeapon.spriteReference;

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

        void Add(int slotIndex, WeaponBasicModel basicModel)
        {
            PlayerModel playerModel = dataManager.playerModel;
            EquipedWeaponModel equipedWeapon = new();
            HolsterModel slot = new();
            ShopWeaponModel shopWeapon = playerModel.ShopWeapons[(int)basicModel.weaponName - 1];

            equipedWeapon.weaponName = basicModel.weaponName;
            equipedWeapon.suppressor = shopWeapon.suppressor;
            equipedWeapon.sight = shopWeapon.sight;
            equipedWeapon.magazineBulletCount = basicModel.weaponAttribute.MagazineSize;
            equipedWeapon.id = UnityEngine.Random.Range(100000, 999999);

            slot.equipedWeapon = equipedWeapon;
            slot.isOccupied = true;

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

    private void BuildCustomizeScreen(WeaponBasicModel sealedWeaponModel)
    {
        BuildFundAdapter();
        var deliverPlayerModel = new PlayerModel(dataManager.playerModel);
        var deliverShopWeapon = deliverPlayerModel.ShopWeapons[(int)sealedWeaponModel.weaponName - 1];
        var selectorShopWeapon = new ShopWeaponModel(dataManager.playerModel.ShopWeapons[(int)sealedWeaponModel.weaponName - 1]);
        var originalShopWeapon = dataManager.playerModel.ShopWeapons[(int)sealedWeaponModel.weaponName - 1];
        customizationScreen.gameObject.SetActive(true);

        var sightContentsModel = new List<SealedSightModel>();
        var suppressorContentsModel = new List<SealedSuppressorModel>();
        
        customizationApplyButton.onClick.RemoveAllListeners();
        customizationApplyButton.onClick.AddListener(Apply);

        BuildSightContents();
        BuildSuppressorContents();
        Debug.Log("CUSTOMIZATION SCREEN");
        
        customizationNavigationBar.Initialize();
        foreach (var itemButton in customizationNavigationBar.barButtons)
        {
            switch (itemButton.transform.GetSiblingIndex())
            {
                case 0:
                {
                    Debug.Log("ADDED BAR BUTTON 0");
                    itemButton.OnClick.AddListener((() => SelectorSight(sealedData.Sights[(int) deliverShopWeapon.sight.name])));
                    break;
                }
                case 1:
                {
                    Debug.Log("ADDED BAR BUTTON 1");
                    itemButton.OnClick.AddListener((() => SelectorSuppressor(sealedData.Suppressors[(int) deliverShopWeapon.suppressor.name])));
                    break;
                }
            }
        }
        customizationNavigationBar.barButtons[0].OnClick.Invoke();
        
        void BuildSightContents()
        {
            Clear(customizationSightContentParent);
            sightContentsModel.Clear();
            foreach (var sightModel in sealedData.Sights)
            {
                foreach (var weaponName in sightModel.compatibleWeaponsName)
                {
                    if (weaponName != sealedWeaponModel.weaponName) continue;
                    sightContentsModel.Add(sightModel);
                    var sightContent = Instantiate(customizationContentPrefab, customizationSightContentParent);
                    if (sightContent.transform.GetChild(0).TryGetComponent(out Image image))
                        image.sprite = sightModel.referenceImage;
                    if (sightContent.TryGetComponent(out Button button))
                        button.onClick.AddListener((() => SelectorSight(sightModel)));
                }
            }
            SelectorSight(sealedData.Sights[(int) deliverShopWeapon.sight.name]);
        }
        void BuildSuppressorContents()
        {
            Clear(customizationSuppressorContentParent);
            suppressorContentsModel.Clear();
            foreach (var suppressorModel in sealedData.Suppressors)
            {
                foreach (var weaponName in suppressorModel.compatibleWeaponsName)
                {
                    if (weaponName != sealedWeaponModel.weaponName) continue;
                    suppressorContentsModel.Add(suppressorModel);
                    var suppressorContent = Instantiate(customizationContentPrefab, customizationSuppressorContentParent);
                    if (suppressorContent.transform.GetChild(0).TryGetComponent(out Image image))
                        image.sprite = suppressorModel.referenceImage;
                    if (suppressorContent.TryGetComponent(out Button button))
                        button.onClick.AddListener((() => SelectorSuppressor(suppressorModel)));
                }
            }
            SelectorSuppressor(sealedData.Suppressors[(int) deliverShopWeapon.suppressor.name]);
        }
        void SelectorSight(SealedSightModel selectedSightModel)
        {
            Debug.Log("SELECTED SIGHT");
            customizationDescriptionText.text = selectedSightModel.description;
            customizationPriceText.text = selectedSightModel.price.ToString();
            SetPreferredSizeWidth(customizationPriceText);
            
            var originalSelectionIsPurchased = originalShopWeapon.purchasedSights.Any(purchasedSight => purchasedSight.name == selectedSightModel.name);

            customizationPriceText.transform.parent.gameObject.SetActive(!originalSelectionIsPurchased);
            customizationBuyButton.gameObject.SetActive(!originalSelectionIsPurchased);
            customizationApplyButton.gameObject.SetActive(originalSelectionIsPurchased);
            customizationAttachButton.gameObject.SetActive(originalSelectionIsPurchased);
            
            selectorShopWeapon.sight = new SightModel() { name = selectedSightModel.name };
            
            if (!originalSelectionIsPurchased)
            {
                customizationBuyButton.onClick.RemoveAllListeners();
                if (dataManager.playerModel.Funds.money >= selectedSightModel.price)
                {
                    customizationBuyButton.interactable = true;
                    customizationBuyButton.onClick.AddListener((() => BuySight(selectedSightModel)));
                }
                else customizationBuyButton.interactable = false;
            }
            else
            {
                var selectionIsAttached = selectedSightModel.name == deliverShopWeapon.sight.name;
                if (selectionIsAttached)
                {
                    customizationAttachButton.interactable = false;
                }
                else
                {
                    customizationAttachButton.onClick.RemoveAllListeners();
                    customizationAttachButton.interactable = true;
                    customizationAttachButton.onClick.AddListener((() => AttachSight(selectedSightModel)));
                }
            }

            var contentIndex = 0;
            foreach (RectTransform contentTransform in customizationSightContentParent)
            {
                var isSelected = sightContentsModel[contentIndex].name == selectorShopWeapon.sight.name;
                var isAttached = sightContentsModel[contentIndex].name == deliverShopWeapon.sight.name;
                var isPurchased = IsPurchasedSight(sightContentsModel[contentIndex]);
                if (contentTransform.TryGetComponent(out Image backgroundImage))
                {
                    if (isSelected)
                    {
                        backgroundImage.sprite = isAttached ? customizationAttachedContentSprite : customizationNotPurchasedContentSprite;
                        backgroundImage.color = customizationSelectedContentColor;
                    }
                    else if(isAttached)
                    {
                        backgroundImage.sprite = customizationAttachedContentSprite;
                        backgroundImage.color = customizationAttachedContentColor;
                    }
                    else if(isPurchased)
                    {
                        backgroundImage.sprite = customizationPurchasedContentSprite;
                        backgroundImage.color = customizationPurchasedContentColor;
                    }
                    else
                    {
                        backgroundImage.sprite = customizationNotPurchasedContentSprite;
                        backgroundImage.color = customizationNotPurchasedContentColor;
                    }
                }

                contentIndex++;
            }
        }
        void SelectorSuppressor(SealedSuppressorModel sealedSuppressorModel)
        {
            Debug.Log("SELECTED SUPPRESSOR");
            customizationDescriptionText.text = sealedSuppressorModel.description;
            customizationPriceText.text = sealedSuppressorModel.price.ToString();
            SetPreferredSizeWidth(customizationPriceText);
            
            var originalSelectionIsPurchased = originalShopWeapon.purchasedSuppressors.Any(purchasedSuppressor => purchasedSuppressor.name == sealedSuppressorModel.name);

            customizationPriceText.transform.parent.gameObject.SetActive(!originalSelectionIsPurchased);
            customizationBuyButton.gameObject.SetActive(!originalSelectionIsPurchased);
            customizationApplyButton.gameObject.SetActive(originalSelectionIsPurchased);
            customizationAttachButton.gameObject.SetActive(originalSelectionIsPurchased);
            
            selectorShopWeapon.suppressor = new SuppressorModel() { name = sealedSuppressorModel.name };
            
            if (!originalSelectionIsPurchased)
            {
                customizationBuyButton.onClick.RemoveAllListeners();
                if (dataManager.playerModel.Funds.money >= sealedSuppressorModel.price)
                {
                    customizationBuyButton.interactable = true;
                    customizationBuyButton.onClick.AddListener((() => BuySuppressor(sealedSuppressorModel)));
                }
                else customizationBuyButton.interactable = false;
            }
            else
            {
                var selectionIsAttached = sealedSuppressorModel.name == deliverShopWeapon.suppressor.name;
                if (selectionIsAttached)
                {
                    customizationAttachButton.interactable = false;
                }
                else
                {
                    customizationAttachButton.onClick.RemoveAllListeners();
                    customizationAttachButton.interactable = true;
                    customizationAttachButton.onClick.AddListener((() => AttachSuppressor(sealedSuppressorModel)));
                }
            }

            var contentIndex = 0;
            foreach (RectTransform contentTransform in customizationSuppressorContentParent)
            {
                var isSelected = suppressorContentsModel[contentIndex].name == selectorShopWeapon.suppressor.name;
                var isAttached = suppressorContentsModel[contentIndex].name == deliverShopWeapon.suppressor.name;
                var isPurchased = IsPurchasedSuppressor(suppressorContentsModel[contentIndex]);
                if (contentTransform.TryGetComponent(out Image backgroundImage))
                {
                    if (isSelected)
                    {
                        backgroundImage.sprite = isAttached ? customizationAttachedContentSprite : customizationNotPurchasedContentSprite;
                        backgroundImage.color = customizationSelectedContentColor;
                    }
                    else if(isAttached)
                    {
                        backgroundImage.sprite = customizationAttachedContentSprite;
                        backgroundImage.color = customizationAttachedContentColor;
                    }
                    else if(isPurchased)
                    {
                        backgroundImage.sprite = customizationPurchasedContentSprite;
                        backgroundImage.color = customizationPurchasedContentColor;
                    }
                    else
                    {
                        backgroundImage.sprite = customizationNotPurchasedContentSprite;
                        backgroundImage.color = customizationNotPurchasedContentColor;
                    }
                }

                contentIndex++;
            }
        }
        void BuySight(SealedSightModel sightModel)
        {
            var sight = new SightModel
            {
                name = sightModel.name,
                equipped = false
            };
            var playerModel = dataManager.playerModel;
            dataManager.playerModel.ShopWeapons[(int)sealedWeaponModel.weaponName - 1].purchasedSights.Add(sight);
            deliverPlayerModel.ShopWeapons[(int)sealedWeaponModel.weaponName -1].purchasedSights.Add(sight);
            playerModel.Funds.money -= sightModel.price;
            deliverPlayerModel.Funds.money -= sightModel.price;
            dataManager.UpdatePlayerModel(playerModel);
            SelectorSight((sightModel));
            BuildFundAdapter();
        }
        void BuySuppressor(SealedSuppressorModel suppressorModel)
        {
            var suppressor = new SuppressorModel()
            {
                name = suppressorModel.name,
                equipped = false
            };
            var playerModel = dataManager.playerModel;
            dataManager.playerModel.ShopWeapons[(int)sealedWeaponModel.weaponName - 1].purchasedSuppressors.Add(suppressor);
            deliverPlayerModel.ShopWeapons[(int)sealedWeaponModel.weaponName -1].purchasedSuppressors.Add(suppressor);
            playerModel.Funds.money -= suppressorModel.price;
            deliverPlayerModel.Funds.money -= suppressorModel.price;
            dataManager.UpdatePlayerModel(playerModel);
            SelectorSuppressor((suppressorModel));
            BuildFundAdapter();
        }
        void AttachSight(SealedSightModel attachableSightModel)
        {
            deliverShopWeapon.sight = new SightModel() { name = attachableSightModel.name, equipped = true };
            SelectorSight((attachableSightModel));
        }
        
        void AttachSuppressor(SealedSuppressorModel attachableSuppressorModel)
        {
            deliverShopWeapon.suppressor = new SuppressorModel() { name = attachableSuppressorModel.name, equipped = true };
            SelectorSuppressor((attachableSuppressorModel));
        }

        bool IsPurchasedSight(SealedSightModel sightModel)
        {
            foreach (var purchasedSight in dataManager.playerModel.ShopWeapons[(int)sealedWeaponModel.weaponName - 1]
                         .purchasedSights)
            {
                if (sightModel.name == purchasedSight.name)
                {
                    return true;
                }
            }

            return false;
        }
        bool IsPurchasedSuppressor(SealedSuppressorModel suppressorModel)
        {
            foreach (var purchasedSuppressor in dataManager.playerModel.ShopWeapons[(int)sealedWeaponModel.weaponName - 1]
                         .purchasedSuppressors)
            {
                if (suppressorModel.name == purchasedSuppressor.name)
                {
                    return true;
                }
            }

            return false;
        }
        void Apply()
        {
            dataManager.UpdatePlayerModel(deliverPlayerModel);
            customizationScreen.gameObject.SetActive(false);
        }
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
                    var leftoverMoney = playerModel.Funds.money - basicModel.weaponPrice;
                    if (leftoverMoney >= 0)
                    {
                        playerModel.Funds.money -= basicModel.weaponPrice;
                        shopWeapon.isPurchased = true;
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
            Debug.LogWarning(
                $"Clear loop stopped after 100 iterations. Some children may remain. Looped count: {counter}");
        }
    }

    public void SetPreferredSizeWidth(TMP_Text text)
    {
        var preferredSize = text.GetPreferredValues();
        var preferredWidth = preferredSize.x;
        text.rectTransform.sizeDelta = new Vector2(preferredWidth, text.rectTransform.sizeDelta.y);
    }
}