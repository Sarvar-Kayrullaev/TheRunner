using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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



    private TMP_Text weaponTitleText;
    private TMP_Text weaponPriceText;

    private TMP_Text weaponStatsTitleText1;
    private TMP_Text weaponStatsValueText1;
    private TMP_Text weaponStatsTitleText2;
    private TMP_Text weaponStatsValueText2;
    private TMP_Text weaponStatsTitleText3;
    private TMP_Text weaponStatsValueText3;
    private TMP_Text weaponStatsTitleText4;
    private TMP_Text weaponStatsValueText4;
    private TMP_Text weaponStatsTitleText5;
    private TMP_Text weaponStatsValueText5;

    private Button weaponBuyButtonComponent;
    private Button weaponEquipButtonComponent;
    private Button weaponCustomizeButtonComponent;

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
        if (weaponContentParent == null || contentPrefab == null)
        {
            Debug.LogError("Weapon content parent or content prefab is not assigned in the ShopAdapter.");
            return;
        }
        if (weaponTitleTextParent == null || weaponStatsBar1 == null || weaponStatsBar2 == null ||
            weaponStatsBar3 == null || weaponStatsBar4 == null || weaponStatsBar5 == null ||
            weaponPriceTextParent == null || weaponBuyButton == null || weaponEquipButton == null || weaponCustomizeButton == null)
        {
            Debug.LogError("One or more UI elements are not assigned in the ShopAdapter.");
            return;
        }
        // Initialize the shop adapter with default values or settings if needed

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
        // This method can be used to display weapon stats based on the weapon type.
        // Implementation can be added as needed.


    }
    /// <summary>
    /// Clears all child items from the specified parent transform.
    /// This is useful for resetting the UI before adding new items.
    /// The method used by While
    /// </summary>

    void Clear(Transform parent)
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
        int counter = 0;
        bool isComplete = false;
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
