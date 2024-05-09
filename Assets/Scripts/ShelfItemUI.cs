using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class ShelfItemUI : MonoBehaviour
{
    /// <summary>
    /// The dynamic content size script
    /// </summary>
    private DynamicContentSizeForOneColumn dynamicContentSizeScript;

    /// <summary>
    /// The inventory manager
    /// </summary>
    [Header("Manager References")]
    private InventoryManager inventoryManager;
    /// <summary>
    /// The shelf manager
    /// </summary>
    private ShelfManager shelfManager;
    /// <summary>
    /// The bought for cost text
    /// </summary>
    public TextMeshProUGUI boughtForCostText;
    /// <summary>
    /// The demand bar
    /// </summary>
    public Image demandBar;
    /// <summary>
    /// The inventory item
    /// </summary>
    public InventoryItem inventoryItem;

    /// <summary>
    /// The inventory item UI
    /// </summary>
    [Header("Misc")]
    public InventoryItemUI inventoryItemUI;
    /// <summary>
    /// The item image
    /// </summary>
    [Header("UI Components")]
    public Image itemImage;
    /// <summary>
    /// The item name
    /// </summary>
    public string itemName;
    /// <summary>
    /// The item name text
    /// </summary>
    public TextMeshProUGUI itemNameText;
    /// <summary>
    /// The minus button
    /// </summary>
    public Button minusButton;
    /// <summary>
    /// The plus button
    /// </summary>
    public Button plusButton;
    /// <summary>
    /// The profit per item
    /// </summary>
    public float profitPerItem;
    /// <summary>
    /// The profit per item text
    /// </summary>
    public TextMeshProUGUI profitPerItemText;
    /// <summary>
    /// The quantity on shelf
    /// </summary>
    public int quantityOnShelf;
    /// <summary>
    /// The quantity on shelf text
    /// </summary>
    public TextMeshProUGUI quantityOnShelfText;
    /// <summary>
    /// The quantity to remove text
    /// </summary>
    public TextMeshProUGUI quantityToRemoveText;
    /// <summary>
    /// The remove button
    /// </summary>
    public Button removeButton;

    /// <summary>
    /// The selling price
    /// </summary>
    [Header("Item Settings")]
    public float sellingPrice;
    /// <summary>
    /// The selling price text
    /// </summary>
    public TextMeshProUGUI sellingPriceText;
    /// <summary>
    /// The total profit
    /// </summary>
    public float totalProfit;
    /// <summary>
    /// The total profit text
    /// </summary>
    public TextMeshProUGUI totalProfitText;


    /// <summary>
    /// Awakes this instance.
    /// </summary>
    private void Awake()
    {
        // Add listeners to buttons
        plusButton.onClick.AddListener(OnPlusButtonClicked);
        minusButton.onClick.AddListener(OnMinusButtonClicked);
        removeButton.onClick.AddListener(OnRemoveButtonClicked);

        // Find InventoryManager in the scene
        inventoryManager = FindObjectOfType<InventoryManager>();
        shelfManager = FindObjectOfType<ShelfManager>();
    }

    /// <summary>
    /// Updates the size of the content.
    /// </summary>
    private void UpdateContentSize()
    {
        if (shelfManager != null)
        {
            // Call the UpdateContentSize function of the ShelfManager
            shelfManager.UpdateContentSize();
        }
    }

    /// <summary>
    /// Updates the demand bar.
    /// </summary>
    /// <param name="color">The color.</param>
    private void UpdateDemandBar(Color color)
    {
        if (demandBar != null)
        {
            demandBar.color = color;
        }
        else
        {
            Debug.LogError("Demand bar is not assigned or is not an Image component.");
        }
    }

    /// <summary>
    /// Called when [minus button clicked].
    /// </summary>
    public void OnMinusButtonClicked()
    {
        // Decrease the quantity to remove, ensuring it doesn't go below zero
        int quantityToRemove = int.Parse(quantityToRemoveText.text);
        if (quantityToRemove > 0)
        {
            quantityToRemove--;
            quantityToRemoveText.text = quantityToRemove.ToString();
        }
    }

    /// <summary>
    /// Called when [plus button clicked].
    /// </summary>
    public void OnPlusButtonClicked()
    {
        // Increase the quantity to remove, up to the maximum quantity on the shelf
        int quantityToRemove = int.Parse(quantityToRemoveText.text);
        if (quantityToRemove < quantityOnShelf)
        {
            quantityToRemove++;
            quantityToRemoveText.text = quantityToRemove.ToString();
        }
    }

    /// <summary>
    /// Called when [remove button clicked].
    /// </summary>
    public void OnRemoveButtonClicked()
    {
        int quantityToRemove = int.Parse(quantityToRemoveText.text);
        if (quantityToRemove > 0)
        {
            quantityOnShelf -= quantityToRemove;
            quantityOnShelfText.text = quantityOnShelf.ToString();

            totalProfit = profitPerItem * quantityOnShelf;
            totalProfitText.text = "£" + totalProfit.ToString("F2");

            if (inventoryManager != null)
            {
                inventoryManager.HandleRemovedShelfItem(itemName, boughtForCostText.text, sellingPriceText.text, quantityToRemove, itemImage.sprite);
                InformationBar.Instance.DisplayMessage($"{quantityToRemove}x {itemName} removed from shelf");
            }

            quantityToRemoveText.text = "0";
        }

        if (quantityOnShelf <= 0)
        {
            shelfManager.RemoveShelfItem(this.gameObject);
        }
    }

    /// <summary>
    /// Sets the dynamic content size script.
    /// </summary>
    /// <param name="script">The script.</param>
    public void SetDynamicContentSizeScript(DynamicContentSizeForOneColumn script)
    {
        dynamicContentSizeScript = script;
    }

    /// <summary>
    /// Sets the item data.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="sellQuantity">The sell quantity.</param>
    public void SetItemData(InventoryItem item, int sellQuantity)
    {
        Debug.Log("Setting item data for " + item.itemName);
        Debug.Log("Sell Quantity: " + sellQuantity);
        Debug.Log("Item Cost: " + item.cost);
        Debug.Log("Selling Price: " + item.sellingPrice);

        // Set the information for the shelvesPanelPrefab
        itemNameText.text = item.itemName;
        boughtForCostText.text = "£" + item.cost.ToString("F2");

        // Set the item image
        if (itemImage != null && item.itemImage != null)
        {
            itemImage.sprite = item.itemImage;
        }
        else
        {
            Debug.LogError("Item image component or sprite is null.");
        }

        // Assign the selling price
        sellingPrice = item.sellingPrice;
        sellingPriceText.text = "£" + sellingPrice.ToString("F2");

        // Calculate profit per item
        profitPerItem = sellingPrice - item.cost;
        profitPerItemText.text = "£" + profitPerItem.ToString("F2");

        // Calculate total profit
        totalProfit = profitPerItem * sellQuantity;
        Debug.Log("Calculated Total Profit: " + totalProfit);
        totalProfitText.text = "£" + totalProfit.ToString("F2");

        // Set quantity on shelf
        quantityOnShelf = sellQuantity;
        quantityOnShelfText.text = quantityOnShelf.ToString();

        Color demandColor = InventoryManager.CalculateDemandBarColor(sellingPrice, item.cost);
        UpdateDemandBar(demandColor);

        itemName = item.itemName;

        UpdateContentSize();
    }

    /// <summary>
    /// Updates the UI.
    /// </summary>
    public void UpdateUI()
    {
        quantityOnShelfText.text = quantityOnShelf.ToString();
        totalProfitText.text = $"£{profitPerItem * quantityOnShelf:F2}";

        // Remove the item from display if no more are left
        if (quantityOnShelf <= 0)
        {
            Debug.Log($"{itemName} is out of stock, removing from shelf.");
            InformationBar.Instance.DisplayMessage($"{itemName} is now out of stock.");
            shelfManager.RemoveShelfItem(gameObject);
        }
    }



}
