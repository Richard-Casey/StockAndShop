using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

/// <summary>
/// 
/// </summary>
public class WholesaleManager : MonoBehaviour
{
    /// <summary>
    /// The content grid layout
    /// </summary>
    public Transform contentGridLayout;
    /// <summary>
    /// The dynamic content sizer
    /// </summary>
    public DynamicContentSizeForTwoColumns dynamicContentSizer;

    /// <summary>
    /// The inventory manager
    /// </summary>
    [Header("Manager References")]
    public InventoryManager inventoryManager;

    /// <summary>
    /// The wholesale item prefab
    /// </summary>
    [Header("UI Components")]
    public GameObject wholesaleItemPrefab;
    /// <summary>
    /// The wholesale items
    /// </summary>
    [Header("Wholesale Items")]
    public List<InventoryItem> wholesaleItems;


    /// <summary>
    /// Populates the wholesale UI.
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
    /// Starts this instance.
    /// </summary>
    void Start()
    {
        PopulateWholesaleUI();
    }



    // When moving items to the inventory from wholesale
    /// <summary>
    /// Moves the items to inventory.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    /// <param name="quantity">The quantity.</param>
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
