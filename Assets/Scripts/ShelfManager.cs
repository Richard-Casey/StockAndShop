using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the items on the shelf, including initialization, updating, and removal.
/// </summary>
public class ShelfManager : MonoBehaviour
{
    /// <summary>
    /// The grid layout where shelf items are displayed.
    /// </summary>
    public Transform contentGridLayout;

    /// <summary>
    /// Reference to the script that dynamically adjusts content size for one column.
    /// </summary>
    public DynamicContentSizeForOneColumn dynamicContentSizeScript;

    [Header("Manager References")]

    /// <summary>
    /// Reference to the InventoryManager for accessing inventory items.
    /// </summary>
    public InventoryManager inventoryManager;

    /// <summary>
    /// The container where shelf items are instantiated.
    /// </summary>
    public Transform shelfContainer;
    [Header("Prefab and Container")]

    /// <summary>
    /// The prefab used for creating shelf items.
    /// </summary>
    public GameObject shelfItemPrefab;

    [Header("Misc")]

    /// <summary>
    /// A list of currently active shelf items.
    /// </summary>
    public List<GameObject> shelfItems;

    /// <summary>
    /// Initializes the shelf items by clearing existing items and adding selected inventory items.
    /// </summary>
    void InitializeShelfItems()
    {
        foreach (var item in shelfItems)
        {
            Destroy(item);
        }
        shelfItems.Clear();

        // Get InventoryItems
        List<InventoryItem> inventoryItems = inventoryManager.inventoryItems;

        // Iterate through the selected items in the inventory
        foreach (var inventoryItem in inventoryItems)
        {
            // Find the corresponding InventoryItemUI for this inventory item
            InventoryItemUI selectedItemUI = inventoryManager.inventoryItemUIList.Find(ui => ui.itemName == inventoryItem.itemName);

            // Check if the item is selected for selling and has a sell quantity greater than 0
            if (selectedItemUI != null && selectedItemUI.isSelectedForSelling && selectedItemUI.sellQuantity > 0)
            {
                GameObject shelfItem = Instantiate(shelfItemPrefab, shelfContainer);
                ShelfItemUI shelfItemUI = shelfItem.GetComponent<ShelfItemUI>();

                if (shelfItemUI != null)
                {
                    // Populate the shelf item with data from the inventory item and sell quantity
                    shelfItemUI.SetItemData(inventoryItem, selectedItemUI.sellQuantity);

                    // Add the shelf item to the list
                    shelfItems.Add(shelfItem);
                }
            }
        }
    }

    /// <summary>
    /// Unity's Start method, called on the frame when a script is enabled just before any of the Update methods are called.
    /// </summary>
    void Start()
    {
        InitializeShelfItems();
    }

    /// <summary>
    /// Adds an item to the shelf items, either updating an existing item or creating a new one.
    /// </summary>
    /// <param name="itemToAdd">The item to add to the shelf.</param>
    public void AddToShelfItems(InventoryItem itemToAdd)
    {
        // Check if an item with the same name and selling price already exists on the shelf
        ShelfItemUI existingItem = FindExistingShelfItem(itemToAdd.itemName, itemToAdd.sellingPrice);

        if (existingItem != null)
        {
            // Item exists, update quantity and possibly other relevant data
            existingItem.quantityOnShelf += itemToAdd.quantity;
            existingItem.inventoryItem = itemToAdd; // Ensure the inventoryItem reference is updated
            existingItem.UpdateUI();
        }
        else
        {
            // If no existing item found, create a new GameObject for the shelf item
            GameObject shelfItem = Instantiate(shelfItemPrefab, shelfContainer);

            // Get the ShelfItemUI component from the instantiated GameObject
            ShelfItemUI shelfItemUI = shelfItem.GetComponent<ShelfItemUI>();

            if (shelfItemUI != null)
            {
                // Populate the shelf item with data from the inventory item and sell quantity
                shelfItemUI.SetItemData(itemToAdd, itemToAdd.quantity); 
                shelfItemUI.inventoryItem = itemToAdd; // Set the inventoryItem reference

                shelfItemUI.SetDynamicContentSizeScript(dynamicContentSizeScript);
                shelfItem.transform.SetParent(contentGridLayout, false);  // Use 'false' to maintain local position and rotation

                // Add the shelf item to the list
                shelfItems.Add(shelfItem);

                // Call the UpdateContentSize function
                UpdateContentSize();
            }
            else
            {
                Debug.LogError("ShelfItemUI component not found on the instantiated shelf item.");
            }
        }
    }

    /// <summary>
    /// Finds an existing shelf item based on the item name and selling price.
    /// </summary>
    /// <param name="itemName">The name of the item.</param>
    /// <param name="sellingPrice">The selling price of the item.</param>
    /// <returns>The found ShelfItemUI component or null if not found.</returns>
    public ShelfItemUI FindExistingShelfItem(string itemName, float sellingPrice)
    {
        // Iterate through existing shelf items and find a match
        foreach (GameObject shelfItemGameObject in shelfItems)
        {
            ShelfItemUI shelfItem = shelfItemGameObject.GetComponent<ShelfItemUI>();
            if (shelfItem != null && shelfItem.itemNameText.text == itemName && shelfItem.sellingPrice == sellingPrice)
            {
                return shelfItem;
            }
        }
        return null; // If no matching shelf item is found
    }

    /// <summary>
    /// Gets the cost of an item based on its name.
    /// </summary>
    /// <param name="itemName">The name of the item.</param>
    /// <returns>The cost of the item or -1 if not found.</returns>
    public float GetItemCost(string itemName)
    {
        foreach (var itemGO in shelfItems)
        {
            var shelfItemUI = itemGO.GetComponent<ShelfItemUI>();
            if (shelfItemUI != null && shelfItemUI.itemName == itemName)
            {
                return shelfItemUI.inventoryItem.cost; 
            }
        }
        return -1; // Item not found
    }

    /// <summary>
    /// Gets the selling price of an item based on its name.
    /// </summary>
    /// <param name="itemName">The name of the item.</param>
    /// <returns>The selling price of the item or -1 if not found.</returns>
    public float GetItemPrice(string itemName)
    {
        foreach (var itemGO in shelfItems)
        {
            var shelfItemUI = itemGO.GetComponent<ShelfItemUI>();
            if (shelfItemUI != null && shelfItemUI.itemName == itemName)
            {
                return shelfItemUI.sellingPrice; // Directly use the sellingPrice field
            }
        }
        return -1; // Item not found
    }

    /// <summary>
    /// Gets the lowest selling price among all items on the shelf.
    /// </summary>
    /// <returns>The lowest selling price or 0 if no items are found.</returns>
    public float GetLowestPriceOnShelf()
    {
        float lowestPrice = float.MaxValue; // Start with the maximum possible float value

        // Loop through each shelf item to find the lowest price
        foreach (var itemGameObject in shelfItems)
        {
            ShelfItemUI shelfItemUI = itemGameObject.GetComponent<ShelfItemUI>();
            if (shelfItemUI != null && shelfItemUI.sellingPrice < lowestPrice)
            {
                lowestPrice = shelfItemUI.sellingPrice;
            }
        }

        // If no items were found, or all items had invalid prices, return 0 or some default minimum price
        return lowestPrice == float.MaxValue ? 0 : lowestPrice;
    }

    /// <summary>
    /// Removes a shelf item.
    /// </summary>
    /// <param name="shelfItem">The shelf item to remove.</param>
    public void RemoveShelfItem(GameObject shelfItem)
    {
        if (shelfItems.Contains(shelfItem))
        {
            shelfItems.Remove(shelfItem);
            Destroy(shelfItem);

            // Call the UpdateContentSize function
            UpdateContentSize();
        }
    }

    /// <summary>
    /// Updates the content size based on the number of shelf items.
    /// </summary>
    public void UpdateContentSize()
    {
        if (inventoryManager != null)
        {
            // Get the number of items in the shelf
            int itemCount = shelfItems.Count;

            // Call the UpdateContentSize function of the DynamicContentSizeForOneColumn script
            inventoryManager.dynamicContentSizeScript.UpdateContentSize(itemCount);
        }
    }

    /// <summary>
    /// Updates the shelf items by reinitializing them.
    /// </summary>
    public void UpdateShelfItems()
    {
        InitializeShelfItems();
    }

    /// <summary>
    /// Gets the quantities of items on the shelf.
    /// </summary>
    /// <returns>A dictionary with item names and their corresponding quantities.</returns>
    public Dictionary<string, int> GetShelfItemQuantities()
    {
        Dictionary<string, int> quantities = new Dictionary<string, int>();
        foreach (var item in shelfItems)
        {
            ShelfItemUI ui = item.GetComponent<ShelfItemUI>();
            if (ui != null && !quantities.ContainsKey(ui.itemName))
            {
                quantities.Add(ui.itemName, ui.quantityOnShelf); // Use initial quantity for calculations
            }
        }
        return quantities;
    }



}
