using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class WholesaleItemUI : MonoBehaviour
{
    /// <summary>
    /// The buy item handler
    /// </summary>
    public BuyItemHandler buyItemHandler;
    /// <summary>
    /// The cash display
    /// </summary>
    public CashDisplay cashDisplay;
    /// <summary>
    /// The daily summary manager
    /// </summary>
    public DailySummaryManager dailySummaryManager;

    /// <summary>
    /// The inventory item
    /// </summary>
    [Header("Manager References")]
    public InventoryItem inventoryItem;
    /// <summary>
    /// The inventory manager
    /// </summary>
    public InventoryManager inventoryManager;
    /// <summary>
    /// The item cost
    /// </summary>
    public float itemCost;

    /// <summary>
    /// The item name
    /// </summary>
    [Header("Item Settings")]
    public string itemName;
    /// <summary>
    /// The minus button
    /// </summary>
    public Button minusButton;
    /// <summary>
    /// The plus button
    /// </summary>
    public Button plusButton;
    /// <summary>
    /// The quantity
    /// </summary>
    public int quantity;

    /// <summary>
    /// The quantity text
    /// </summary>
    [Header("UI Components")]
    public TextMeshProUGUI quantityText;
    /// <summary>
    /// The total cost text
    /// </summary>
    public TextMeshProUGUI totalCostText;
    /// <summary>
    /// The wholesale manager
    /// </summary>
    public WholesaleManager wholesaleManager;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    private void Awake()
    {
        Button buyButton = GetComponentInChildren<Button>();
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuyItem);
        }
        else
        {
            Debug.LogError("Buy button not found in the prefab.");
        }

        if (minusButton != null)
        {
            minusButton.onClick.RemoveListener(BuyItem);
        }
    }

    /// <summary>
    /// Starts this instance.
    /// </summary>
    private void Start()
    {
        cashDisplay = FindObjectOfType<CashDisplay>();
        if (cashDisplay == null)
        {
            Debug.LogError("CashDisplay component not found.");
        }

        wholesaleManager = FindObjectOfType <WholesaleManager>();
        if (wholesaleManager == null)
        {
            Debug.LogError("WholesaleManager component not found.");
        }

        dailySummaryManager = FindObjectOfType<DailySummaryManager>();
        if (dailySummaryManager == null)
        {
            Debug.LogError("DailySummaryManager component not found.");
        }

        UpdateUI();
    }

    /// <summary>
    /// Updates the UI.
    /// </summary>
    private void UpdateUI()
    {
        quantityText.text = quantity.ToString();
        float totalCost = inventoryItem.cost * quantity;
        totalCostText.text = $"£{totalCost:F2}";
    }

    /// <summary>
    /// Buys the item.
    /// </summary>
    public void BuyItem()
    {
        Debug.Log("Attempting to buy item");
        if (cashDisplay == null || wholesaleManager == null || dailySummaryManager == null)
        {
            Debug.LogError("One or more required components are not set");
            return;
        }

        float totalCost = itemCost * quantity;

        if (cashDisplay.cashOnHand >= totalCost)
        {
            cashDisplay.SetCash(cashDisplay.cashOnHand - totalCost);
            dailySummaryManager.RegisterDailyExpenses(totalCost);
            wholesaleManager.MoveItemsToInventory(itemName, quantity);

            Debug.Log($"Bought {quantity} {itemName}(s). Remaining cash: £{cashDisplay.cashOnHand}");

            quantity = 0; // Reset quantity
            UpdateUI(); // Update the UI to reflect this change
        }
        else
        {
            Debug.LogError("Insufficient funds or missing WholesaleManager.");
        }
    }



    /// <summary>
    /// Decrements the quantity.
    /// </summary>
    public void DecrementQuantity()
    {
        if (quantity > 0)
        {
            quantity--;
            UpdateUI();
            Debug.Log("Quantity decremented to: " + quantity);
        }
        else
        {
            Debug.Log("Quantity is already at zero.");
        }
    }

    /// <summary>
    /// Increments the quantity.
    /// </summary>
    public void IncrementQuantity()
    {
        quantity++;
        UpdateUI();
    }
}

