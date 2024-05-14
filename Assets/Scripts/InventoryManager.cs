using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


/// <summary>
/// Manages the inventory of items, including adding, removing, and updating items,
/// as well as updating the UI to reflect inventory changes.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    /// <summary>
    /// The parent transform for the content grid layout.
    /// </summary>
    public Transform contentGridLayout;

    [Header("Dynamic Content")]

    /// <summary>
    /// Script for dynamically adjusting the content size for two columns.
    /// </summary>
    public DynamicContentSizeForTwoColumns dynamicContentSizeScript;

    [Header("UI Components")]

    /// <summary>
    /// The prefab for inventory item UI elements.
    /// </summary>
    public GameObject inventoryItemPrefab;

    
    [Header("Inventory Settings")]

    /// <summary>
    /// The list of inventory items.
    /// </summary>
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();

    /// <summary>
    /// The list of InventoryItemUI elements.
    /// </summary>
    public List<InventoryItemUI> inventoryItemUIList = new List<InventoryItemUI>();

    /// <summary>
    /// The parent transform for the inventory panel.
    /// </summary>
    public Transform inventoryPanel;

    /// <summary>
    /// The ScrollRect component for scrolling through the inventory.
    /// </summary>
    public ScrollRect scrollRect;

    /// <summary>
    /// Reference to the ShopFloorManager.
    /// </summary>
    public ShopFloorManager shopFloorManager;// This can be categorized elsewhere if more appropriate

    [Header("Misc")]

    /// <summary>
    /// The vertical scrollbar for the inventory.
    /// </summary>
    public Scrollbar verticalScrollbar;

    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    /// <param name="itemToAdd">The item to add to the inventory.</param>
    public void AddItem(InventoryItem itemToAdd)
    {
        InventoryItem foundItem = inventoryItems.Find(existingItem => existingItem.itemName == itemToAdd.itemName);
        if (foundItem != null)
        {
            foundItem.quantity += itemToAdd.quantity;
            Debug.Log("Updated quantity for existing item: " + foundItem.itemName + " to " + foundItem.quantity);
        }
        else
        {
            inventoryItems.Add(itemToAdd);
            Debug.Log("Added new item: " + itemToAdd.itemName);
        }
        UpdateInventoryUI(); // Call this to refresh the UI each time an item is added
    }

    /// <summary>
    /// Adds quantity to an existing inventory item.
    /// </summary>
    /// <param name="itemName">The name of the item to add quantity to.</param>
    /// <param name="quantityToAdd">The quantity to add.</param>
    public void AddQuantityToInventoryItem(string itemName, int quantityToAdd)
    {
        InventoryItem item = inventoryItems.Find(i => i.itemName == itemName);
        if (item != null)
        {
            item.quantity += quantityToAdd;
        }
        else
        {
            // If the item doesn't exist in the inventory, it could be added as a new item.
            // This depends on how you want to handle such cases.
            Debug.Log("Item not found in inventory: " + itemName);
        }

        // Update the UI to reflect the changes
        UpdateInventoryUI();
    }

    /// <summary>
    /// Calculates the color of the demand bar based on the current price and original cost.
    /// </summary>
    /// <param name="currentPrice">The current price of the item.</param>
    /// <param name="originalCost">The original cost of the item.</param>
    /// <returns>The calculated color for the demand bar.</returns>
    public static Color CalculateDemandBarColor(float currentPrice, float originalCost)
    {
        float lowerBound = originalCost;
        float idealPrice = originalCost * 1.25f;
        float upperBound = originalCost * 1.5f;

        float normalizedValue = Mathf.InverseLerp(lowerBound, upperBound, currentPrice);

        Color demandColor;
        if (currentPrice <= idealPrice)
        {
            demandColor = Color.Lerp(Color.green, Color.yellow, normalizedValue * 2);
        }
        else
        {
            demandColor = Color.Lerp(Color.yellow, Color.red, (normalizedValue - 0.5f) * 2);
        }

        return demandColor;
    }

    /// <summary>
    /// Handles the removal of an item from the shelf and adds it back to the inventory.
    /// </summary>
    /// <param name="itemName">The name of the item to remove.</param>
    /// <param name="costText">The cost text of the item.</param>
    /// <param name="sellingPriceText">The selling price text of the item.</param>
    /// <param name="quantityToAdd">The quantity to add back to the inventory.</param>
    /// <param name="itemImage">The image of the item.</param>
    public void HandleRemovedShelfItem(string itemName, string costText, string sellingPriceText, int quantityToAdd, Sprite itemImage)
    {
        InventoryItem item = inventoryItems.Find(i => i.itemName == itemName);
        if (item != null)
        {
            item.quantity += quantityToAdd;
            if (item.quantity == 0)
            {
                // Logic to remove this item from the inventory list
                inventoryItems.Remove(item);
            }
        }
        else
        {
            // New item logic
            float.TryParse(costText.Replace("£", ""), out float cost);
            float.TryParse(sellingPriceText.Replace("£", ""), out float sellingPrice);

            InventoryItem newItem = new InventoryItem
            {
                itemName = itemName,
                cost = cost,
                sellingPrice = sellingPrice,
                quantity = quantityToAdd,
                itemImage = itemImage // Use the passed image
            };
            inventoryItems.Add(newItem);
        }

        // Update the UI to reflect the changes
        UpdateInventoryUI();
    }


    /// <summary>
    /// Moves items from the inventory to the shop floor.
    /// </summary>
    /// <param name="itemName">The name of the item to move.</param>
    /// <param name="quantity">The quantity to move.</param>
    public void MoveItemsToShopFloor(string itemName, int quantity)
    {
        InventoryItem item = inventoryItems.Find(i => i.itemName == itemName);
        if (item != null && shopFloorManager != null)
        {
            // Remove from Inventory
            inventoryItems.Remove(item);

            // Add to Shop Floor
            shopFloorManager.AddItemToShopFloor(new InventoryItem
            {
                itemName = item.itemName,
                cost = item.cost,
                quantity = quantity,
                itemImage = item.itemImage,
                demand = item.demand,
                baseDemand = item.baseDemand
            });
        }
    }

    /// <summary>
    /// Removes an item from the inventory.
    /// </summary>
    /// <param name="itemName">The name of the item to remove.</param>
    public void RemoveItem(string itemName)
    {
        InventoryItem item = inventoryItems.Find(i => i.itemName == itemName);
        if (item != null)
        {
            inventoryItems.Remove(item);
            UpdateInventoryUI();
        }
    }

    /// <summary>
    /// Updates the quantity of an inventory item.
    /// </summary>
    /// <param name="itemName">The name of the item to update.</param>
    /// <param name="newQuantity">The new quantity of the item.</param>
    public void UpdateInventoryItemQuantity(string itemName, int newQuantity)
    {
        // Find the item in the inventory
        InventoryItem item = inventoryItems.Find(i => i.itemName == itemName);
        if (item != null)
        {
            // Update the quantity
            item.quantity = newQuantity;
        }
        else
        {
            Debug.LogError($"Item not found in inventory: {itemName}");
        }

        UpdateInventoryUI();
    }

    /// <summary>
    /// Updates the inventory UI to reflect the current state of the inventory.
    /// </summary>
    public void UpdateInventoryUI()
    {
        foreach (Transform child in contentGridLayout)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem item in inventoryItems)
        {
            GameObject itemUI = Instantiate(inventoryItemPrefab, contentGridLayout);
            InventoryItemUI itemUIScript = itemUI.GetComponent<InventoryItemUI>();

            if (itemUIScript != null)
            {
                itemUIScript.itemName = item.itemName;
                itemUIScript.itemCost = item.cost;
                itemUIScript.quantity = item.quantity;
                itemUIScript.inventoryManager = this;
                itemUIScript.UpdateUI();

                // Add the InventoryItemUI to the list
                inventoryItemUIList.Add(itemUIScript);
            }

            // Find the corresponding InventoryItemUI for this inventory item
            InventoryItemUI selectedItemUI = inventoryItemUIList.Find(ui => ui.itemName == item.itemName);

            // Only add items that are selected for selling
            if (selectedItemUI != null && selectedItemUI.isSelectedForSelling)
            {
                // Set the isSelectedForSelling property in the InventoryItem
                item.isSelectedForSelling = true;
                item.sellQuantity = selectedItemUI.sellQuantity;
            }
            else
            {
                // If not selected for selling, set isSelectedForSelling to false
                item.isSelectedForSelling = false;
                item.sellQuantity = 0;
            }
        }

        if (dynamicContentSizeScript != null)
        {
            dynamicContentSizeScript.UpdateContentSize(inventoryItems.Count);
        }
        else
        {
            Debug.LogError("DynamicContentSizeForTwoColumns script not assigned in InventoryManager");
        }

        // Update the ScrollRect based on whether the inventory is empty
        if (scrollRect != null)
        {
            bool isInventoryEmpty = inventoryItems.Count == 0;
            scrollRect.verticalScrollbar.gameObject.SetActive(!isInventoryEmpty);
            if (!isInventoryEmpty)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            }
        }
    }

    /// <summary>
    /// Finds an item in the inventory by name.
    /// </summary>
    /// <param name="name">The name of the item to find.</param>
    /// <returns>The found inventory item, or null if not found.</returns>
    public InventoryItem FindItemByName(string name)
    {
        return inventoryItems.FirstOrDefault(item => item.itemName == name);
    }

    /// <summary>
    /// Gets a list of items available for sale.
    /// </summary>
    /// <returns>A list of available items for sale.</returns>
    public List<InventoryItem> GetAvailableItemsForSale()
    {
        // This method returns all items available in the inventory
        return inventoryItems.Where(item => item.quantity > 0).ToList();
    }



}
