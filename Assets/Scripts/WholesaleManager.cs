using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the wholesale operations, including populating the UI with wholesale items and moving items to inventory.
/// </summary>
public class WholesaleManager : MonoBehaviour
{
    /// <summary>
    /// The layout where wholesale item UI elements will be instantiated.
    /// </summary>
    public Transform contentGridLayout;

    /// <summary>
    /// Script for dynamically adjusting the content size based on the number of items.
    /// </summary>
    public DynamicContentSizeForTwoColumns dynamicContentSizer;

    [Header("Manager References")]

    /// <summary>
    /// Reference to the inventory manager.
    /// </summary>
    public InventoryManager inventoryManager;

    [Header("UI Components")]

    /// <summary>
    /// Prefab for the wholesale item UI element.
    /// </summary>
    public GameObject wholesaleItemPrefab;
    [Header("Wholesale Items")]

    /// <summary>
    /// List of items available at the wholesale.
    /// </summary>
    public List<InventoryItem> wholesaleItems;

    /// <summary>
    /// Populates the wholesale UI with items.
    /// </summary>
    void PopulateWholesaleUI()
    {
        foreach (InventoryItem item in wholesaleItems)
        {
            // Instantiate a new UI element from the prefab
            GameObject itemUI = Instantiate(wholesaleItemPrefab, contentGridLayout);

            // Set properties of the instantiated UI element
            WholesaleItemUI itemUIComponent = itemUI.GetComponent<WholesaleItemUI>();
            itemUIComponent.itemName = item.itemName;
            itemUIComponent.itemCost = item.cost;
            itemUIComponent.inventoryItem = item; // Assign the InventoryItem
            itemUIComponent.wholesaleManager = this;

            TextMeshProUGUI itemNameText = itemUI.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            itemNameText.text = item.itemName;

            // Set the price text to the correct price from the InventoryItem
            TextMeshProUGUI priceText = itemUI.transform.Find("PriceText").GetComponent<TextMeshProUGUI>();
            priceText.text = "£" + item.cost.ToString("F2");

            // Access the Image component
            Image itemImage = itemUIComponent.GetComponent<Image>();

            // Check if itemImage is not null before accessing it
            if (itemImage != null)
            {
                itemImage.sprite = item.itemImage;
            }

            // Make sure the itemUI is active in the hierarchy
            itemUI.SetActive(true);

            // Update content size after all items are added
            if (dynamicContentSizer != null)
            {
                dynamicContentSizer.UpdateContentSize(wholesaleItems.Count);
            }
            else
            {
                Debug.LogError("DynamicContentSizer is not assigned in WholesaleManager.");
            }
        }
    }

    /// <summary>
    /// Unity's Start method, called before the first frame update.
    /// </summary>
    void Start()
    {
        PopulateWholesaleUI();
    }



    /// <summary>
    /// Moves items from the wholesale to the inventory.
    /// </summary>
    /// <param name="itemName">Name of the item to move.</param>
    /// <param name="quantity">Quantity of the item to move.</param>
    public void MoveItemsToInventory(string itemName, int quantity)
    {
        InventoryItem item = wholesaleItems.Find(i => i.itemName == itemName);
        if (item != null && inventoryManager != null && quantity > 0)
        {
            inventoryManager.AddItem(new InventoryItem
            {
                itemName = item.itemName,
                cost = item.cost,
                quantity = quantity,
                itemImage = item.itemImage, // Ensure this is correctly assigned
                demand = item.demand,
                baseDemand = item.baseDemand
            });
        }
        else
        {
            Debug.LogError("Failed to find item or invalid setup in wholesale manager.");
        }
    }


}
