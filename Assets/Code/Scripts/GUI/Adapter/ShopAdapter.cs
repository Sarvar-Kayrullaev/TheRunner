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
    /// <summary>
    /// Initializes the shop adapter with the specified weapon type.
    /// This method clears the existing items in the weapon item parent and populates it with new items based on the weapon type.
    /// </summary>
    public void BuildWeaponAdapter(int weaponType)
    {
        Clear(weaponContentParent);

        foreach (WeaponBasicModel model in sealedData.WeaponBasics)
        {
            // Check if the model's type matches the specified weapon type
            if ((int)model.WeaponType == weaponType)
            {
                //show 
                GameObject itemView = Instantiate(contentPrefab, weaponContentParent);
                if (itemView.transform.GetChild(0).TryGetComponent(out Image image))
                {
                    image.sprite = model.SpriteReference;
                }

                if (itemView.TryGetComponent(out Widget.Button button))
                {
                    button.OnClick.AddListener((() => BuildWeaponStatsAdapter((int) model.WeaponEnum)));
                }
            }
        }
    }

    /// <summary>
    /// Displays weapon stats based on the specified weapon type.
    /// This method can be used to show detailed information about a specific weapon.
    /// </summary>
    /// <param name="weaponType">The type of weapon to display stats for.</param>
    public void BuildWeaponStatsAdapter(int weaponType)
    {
        foreach (var weapon in sealedData.WeaponBasics)
        {
            var type = (int) weapon.WeaponEnum;
            if (type == weaponType)
            {
                weaponTitleText.text = weapon.Name;
                weaponPriceText.text = weapon.WeaponPrice.ToString();

                weaponStatsValueText1.text = weapon.WeaponAttribute.Damage.ToString();
                weaponStatsValueText2.text = weapon.WeaponAttribute.Accuracy.ToString();
                weaponStatsValueText3.text = weapon.WeaponAttribute.FireRate.ToString();
                weaponStatsValueText4.text = weapon.WeaponAttribute.ReloadTime.ToString();
                weaponStatsValueText5.text = weapon.WeaponAttribute.Mobility.ToString();
                
            }
        }
    }
    /// <summary>
    /// Clears all child items from the specified parent transform.
    /// This is useful for resetting the UI before adding new items.
    /// The method used by While
    /// </summary>
    private void Clear(Transform parent)
    {

        // Make the while loop not forever
        if (parent == null)
        {
            Debug.LogWarning("Parent RectTransform is null. Cannot clear items.");
            return;
        }
        bool isEmpty = parent.childCount == 0;
        if (isEmpty)
        {
            return; // No need to clear if there are no children
        }
        // Make sure to avoid infinite loops
        // Limit the number of iterations to prevent potential infinite loops
        var counter = 0;
        var isComplete = false;
        // Use a while loop to clear all children
        // This will ensure that all children are removed, even if there are many
        while (isComplete == false)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
            counter++;
            if (counter > 100 || parent.childCount == 0)
            {
                isComplete = true; // Stop if we have cleared all children or reached the limit
            }
        }
        if (parent.childCount > 0)
        {
            Debug.LogWarning($"Clear loop stopped after 100 iterations. Some children may remain. Looped count: {counter}");
        }
    }


}
