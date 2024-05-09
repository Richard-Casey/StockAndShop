using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 
/// </summary>
public class ShelfManager : MonoBehaviour
{
    /// <summary>
    /// The content grid layout
    /// </summary>
    public Transform contentGridLayout;
    /// <summary>
    /// The dynamic content size script
    /// </summary>
    public DynamicContentSizeForOneColumn dynamicContentSizeScript;

    /// <summary>
    /// The inventory manager
    /// </summary>
    [Header("Manager References")]
    public InventoryManager inventoryManager;
    /// <summary>
    /// The shelf container
    /// </summary>
    public Transform shelfContainer;
    /// <summary>
    /// The shelf item prefab
    /// </summary>
    [Header("Prefab and Container")]
    public GameObject shelfItemPrefab;

    /// <summary>
    /// The shelf items
    /// </summary>
    [Header("Misc")]
    public List<GameObject> shelfItems;

    /// <summary>
    /// Initializes the shelf items.
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
    /// Starts this instance.
    /// </summary>
    void Start()
    {
        InitializeShelfItems();
    }

    /// <summary>
    /// Adds to shelf items.
    /// </summary>
    /// <param name="itemToAdd">The item to add.</param>
    public void AddToShelfItems(InventoryItem itemToAdd)
    {
        // Check if an item with the same name and selling price already exists on the shelf
        ShelfItemUI existingItem = FindExistingShelfItem(itemToAdd.itemName, itemToAdd.sellingPrice);

        if (existingItem != null)
        {
            // Item exists, update quantity and possibly other relevant data
            existingItem.quantityOnShelf += itemToAdd.quantity;
            existingItem.inventoryItem = itemToAdd; // Ensure the inventoryItem reference is updated
            existingItem.UpdateUI(); // Make sure you have an UpdateUI method in ShelfItemUI that updates the quantity display
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
                shelfItemUI.SetItemData(itemToAdd, itemToAdd.quantity); // Adjust this method if needed to use the inventoryItem field
                shelfItemUI.inventoryItem = itemToAdd; // Set the inventoryItem reference

                // Additional setup as needed...
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
    /// Finds the existing shelf item.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <param name="sellingPrice">The selling price.</param>
    /// <returns></returns>
    public ShelfItemUI FindExistingShelfItem(string itemName, float sellingPrice)
    {
        // Iterate through your existing shelf items and find a match
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
    /// Gets the item cost.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <returns></returns>
    public float GetItemCost(string itemName)
    {
        foreach (var itemGO in shelfItems)
        {
            var shelfItemUI = itemGO.GetComponent<ShelfItemUI>();
            if (shelfItemUI != null && shelfItemUI.itemName == itemName)
            {
                return shelfItemUI.inventoryItem.cost; // Assuming inventoryItem has a cost field
            }
        }
        return -1; // Item not found
    }

    /// <summary>
    /// Gets the item price.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <returns></returns>
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
    /// Gets the lowest price on shelf.
    /// </summary>
    /// <returns></returns>
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
    /// Removes the shelf item.
    /// </summary>
    /// <param name="shelfItem">The shelf item.</param>
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
    /// Updates the size of the content.
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

    // Method to update the shelf items
    /// <summary>
    /// Updates the shelf items.
    /// </summary>
    public void UpdateShelfItems()
    {
        InitializeShelfItems();
    }

    /// <summary>
    /// Gets the shelf item quantities.
    /// </summary>
    /// <returns></returns>
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
