using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Widget;
using Button = UnityEngine.UI.Button;

/// <summary>
/// ShopAdapter is responsible for managing the shop interface, displaying items based on their type.
/// It uses a SealedData instance to access weapon data and dynamically creates item views in the UI.
/// It clears the existing items in the weapon item parent before adding new ones.
/// </summary>
public class ShopAdapter : MonoBehaviour
{
    public SealedData sealedData;

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
    [Space(1)]
    [SerializeField] private RectTransform weaponPriceTextParent;
    [SerializeField] private RectTransform weaponBuyButton;
    [SerializeField] private RectTransform weaponEquipButton;
    [SerializeField] private RectTransform weaponCustomizeButton;

    
    [Space(1)]
    [SerializeField] private TMP_Text weaponTitleText;
    [SerializeField]private TMP_Text weaponPriceText;
    [Space(1)]
    [SerializeField] private TMP_Text weaponStatsTitleText1;
    [SerializeField] private TMP_Text weaponStatsValueText1;
    [SerializeField] private TMP_Text weaponStatsTitleText2;
    [SerializeField] private TMP_Text weaponStatsValueText2;
    [SerializeField] private TMP_Text weaponStatsTitleText3;
    [SerializeField] private TMP_Text weaponStatsValueText3;
    [SerializeField] private TMP_Text weaponStatsTitleText4;
    [SerializeField] private TMP_Text weaponStatsValueText4;
    [SerializeField] private TMP_Text weaponStatsTitleText5;
    [SerializeField] private TMP_Text weaponStatsValueText5;
    [Space(1)]
    [SerializeField] private Slider weaponStatsSlider1;
    [SerializeField] private Slider weaponStatsSlider2;
    [SerializeField] private Slider weaponStatsSlider3;
    [SerializeField] private Slider weaponStatsSlider4;
    [SerializeField] private Slider weaponStatsSlider5;
    [Space(1)]
    [SerializeField] private Button weaponBuyButtonComponent;
    [SerializeField] private Button weaponEquipButtonComponent;
    [SerializeField] private Button weaponCustomizeButtonComponent;
    
    private void Initialize()
    {
        if (sealedData == null)
        {
            sealedData = FindFirstObjectByType<SealedData>();
            if (sealedData == null)
            {
                Debug.LogError("SealedData instance not found in the scene. Please ensure it is present.");
                return;
            }
        }
    }
    void Awake()
    {
        Initialize();
    }
    
    public void BuildWeaponAdapter(int weaponType)
    {
        Clear(weaponContentParent);

        var defaultSelectedFirst = false;
        var index = 0;
        foreach (WeaponBasicModel model in sealedData.WeaponBasics)
        {
            // Check if the model's type matches the specified weapon type
            if ((int)model.WeaponType == weaponType)
            {
                //show 
                var x = index;
                GameObject itemView = Instantiate(contentPrefab, weaponContentParent);
                if (itemView.transform.GetChild(0).TryGetComponent(out Image image))
                {
                    image.sprite = model.SpriteReference;
                }
                
                if (itemView.TryGetComponent(out Widget.Button button))
                {
                    button.OnClick.AddListener((() => BuildWeaponStatsAdapter((int) model.WeaponEnum)));
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
    
    public void BuildWeaponStatsAdapter(int weaponType)
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
        foreach (var weapon in sealedData.WeaponBasics)
        {
            var type = (int) weapon.WeaponEnum;
            if (type == weaponType)
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
                
            }
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
