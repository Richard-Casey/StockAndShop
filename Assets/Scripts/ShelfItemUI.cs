using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShelfItemUI : MonoBehaviour
{
    private DynamicContentSizeForOneColumn dynamicContentSizeScript;

    [Header("Manager References")]
    private InventoryManager inventoryManager;
    private ShelfManager shelfManager;
    public TextMeshProUGUI boughtForCostText;
    public Image demandBar;
    public InventoryItem inventoryItem;

    [Header("Misc")]
    public InventoryItemUI inventoryItemUI;
    [Header("UI Components")]
    public Image itemImage;
    public string itemName;
    public TextMeshProUGUI itemNameText;
    public Button minusButton;
    public Button plusButton;
    public float profitPerItem;
    public TextMeshProUGUI profitPerItemText;
    public int quantityOnShelf;
    public TextMeshProUGUI quantityOnShelfText;
    public TextMeshProUGUI quantityToRemoveText;
    public Button removeButton;

    [Header("Item Settings")]
    public float sellingPrice;
    public TextMeshProUGUI sellingPriceText;
    public float totalProfit;
    public TextMeshProUGUI totalProfitText;


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

    private void UpdateContentSize()
    {
        if (shelfManager != null)
        {
            // Call the UpdateContentSize function of the ShelfManager
            shelfManager.UpdateContentSize();
        }
    }

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

    public void SetDynamicContentSizeScript(DynamicContentSizeForOneColumn script)
    {
        dynamicContentSizeScript = script;
    }

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
