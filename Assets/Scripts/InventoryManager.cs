using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Transform contentGridLayout;

    [Header("Dynamic Content")]
    public DynamicContentSizeForTwoColumns dynamicContentSizeScript;

    [Header("UI Components")]
    public GameObject inventoryItemPrefab;
    [Header("Inventory Settings")]
    public List<InventoryItem> inventoryItems = new List<InventoryItem>();


    // Create a list to store InventoryItemUI elements
    public List<InventoryItemUI> inventoryItemUIList = new List<InventoryItemUI>();
    public Transform inventoryPanel;
    public ScrollRect scrollRect;
    public ShopFloorManager shopFloorManager;// This can be categorized elsewhere if more appropriate

    [Header("Misc")]
    public Scrollbar verticalScrollbar;

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

    // Method to move items from Inventory to Shop Floor
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

    public void RemoveItem(string itemName)
    {
        InventoryItem item = inventoryItems.Find(i => i.itemName == itemName);
        if (item != null)
        {
            inventoryItems.Remove(item);
            UpdateInventoryUI();
        }
    }

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

    public InventoryItem FindItemByName(string name)
    {
        return inventoryItems.FirstOrDefault(item => item.itemName == name);
    }

    public List<InventoryItem> GetAvailableItemsForSale()
    {
        // This method returns all items available in the inventory
        return inventoryItems.Where(item => item.quantity > 0).ToList();
    }



}
