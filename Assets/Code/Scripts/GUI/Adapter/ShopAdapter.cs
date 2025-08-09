using Data;
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
    public RectTransform weaponItemParent;
    public GameObject itemViewPrefab;

    void Start()
    {
        //WeaponAdapter(0);
        // You can call WeaponAdapter with different weapon types as needed
        // For example: WeaponAdapter((int)WeaponType.Sniper);  
    }
    /// <summary>
    /// Initializes the shop adapter with the specified weapon type.
    /// This method clears the existing items in the weapon item parent and populates it with new items based on the weapon type.
    /// </summary>
    public void WeaponAdapter(int weaponType)
    {
        Clear(weaponItemParent);

        foreach (WeaponBasicModel model in sealedData.WeaponBasics)
        {
            // Check if the model's type matches the specified weapon type
            if ((int)model.WeaponType == weaponType)
            {
                //show 
                GameObject itemView = Instantiate(itemViewPrefab, weaponItemParent);
                if (itemView.transform.GetChild(0).TryGetComponent(out Image image))
                {
                    image.sprite = model.SpriteReference;
                }
            }
        }
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
            if(counter > 100 || parent.childCount == 0)
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
