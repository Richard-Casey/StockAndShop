using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the UI and logic for wholesale items, allowing the player to buy items from the wholesaler.
/// </summary>
public class WholesaleItemUI : MonoBehaviour
{
    /// <summary>
    /// Handler for buying items.
    /// </summary>
    public BuyItemHandler buyItemHandler;

    /// <summary>
    /// Displays the player's current cash.
    /// </summary>
    public CashDisplay cashDisplay;

    /// <summary>
    /// Manages the daily summary and statistics.
    /// </summary>
    public DailySummaryManager dailySummaryManager;

    [Header("Manager References")]

    /// <summary>
    /// Represents the inventory item associated with this UI.
    /// </summary>
    public InventoryItem inventoryItem;

    /// <summary>
    /// Manages the player's inventory.
    /// </summary>
    public InventoryManager inventoryManager;

    /// <summary>
    /// Cost of the item.
    /// </summary>
    public float itemCost;

    [Header("Item Settings")]

    /// <summary>
    /// Name of the item.
    /// </summary>
    public string itemName;

    /// <summary>
    /// Button to decrease the quantity of items to buy.
    /// </summary>
    public Button minusButton;

    /// <summary>
    /// Button to increase the quantity of items to buy.
    /// </summary>
    public Button plusButton;

    /// <summary>
    /// Quantity of items to buy.
    /// </summary>
    public int quantity;

    [Header("UI Components")]

    /// <summary>
    /// Text component displaying the quantity of items.
    /// </summary>
    public TextMeshProUGUI quantityText;

    /// <summary>
    /// Text component displaying the total cost of the items.
    /// </summary>
    public TextMeshProUGUI totalCostText;

    /// <summary>
    /// Manages the wholesale operations.
    /// </summary>
    public WholesaleManager wholesaleManager;

    /// <summary>
    /// Initializes the component. Sets up button listeners and references.
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
    /// Called on the frame when a script is enabled just before any of the Update methods are called the first time.
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
    /// Updates the UI elements to reflect the current quantity and total cost.
    /// </summary>
    private void UpdateUI()
    {
        quantityText.text = quantity.ToString();
        float totalCost = inventoryItem.cost * quantity;
        totalCostText.text = $"£{totalCost:F2}";
    }

    /// <summary>
    /// Handles the purchase of the item. Checks for sufficient funds and updates the relevant components.
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
            UpdateUI(); // Update the UI to reflect change
        }
        else
        {
            Debug.LogError("Insufficient funds or missing WholesaleManager.");
        }
    }

    /// <summary>
    /// Decreases the quantity of items to buy, ensuring it does not go below zero.
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
    /// Increases the quantity of items to buy.
    /// </summary>
    public void IncrementQuantity()
    {
        quantity++;
        UpdateUI();
    }
}

