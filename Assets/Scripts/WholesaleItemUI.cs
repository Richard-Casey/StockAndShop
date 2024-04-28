using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WholesaleItemUI : MonoBehaviour
{
    public BuyItemHandler buyItemHandler;
    public CashDisplay cashDisplay;
    public DailySummaryManager dailySummaryManager;

    [Header("Manager References")]
    public InventoryItem inventoryItem;
    public InventoryManager inventoryManager;
    public float itemCost;

    [Header("Item Settings")]
    public string itemName;
    public Button minusButton;
    public Button plusButton;
    public int quantity;

    [Header("UI Components")]
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI totalCostText;
    public WholesaleManager wholesaleManager;

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

    private void UpdateUI()
    {
        quantityText.text = quantity.ToString();
        float totalCost = inventoryItem.cost * quantity;
        totalCostText.text = $"£{totalCost:F2}";
    }

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

    public void IncrementQuantity()
    {
        quantity++;
        UpdateUI();
    }
}

