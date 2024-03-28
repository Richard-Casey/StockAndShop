using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WholesaleItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI totalCostText;
    public Button plusButton;
    public Button minusButton;

    [Header("Item Settings")]
    public string itemName;
    public float itemCost;
    public int quantity;

    [Header("Manager References")]
    public InventoryItem inventoryItem;
    public CashDisplay cashDisplay;
    public WholesaleManager wholesaleManager;
    public BuyItemHandler buyItemHandler;
    public InventoryManager inventoryManager;
    public DailySummaryManager dailySummaryManager;

    private bool listenersAdded = false;
    
    private void Awake()
    {
        // Find the Buy button within the prefab
        Button buyButton = GetComponentInChildren<Button>();

        if (buyButton != null)
        {
            // Add a listener to the button's click event and specify the method to call
            buyButton.onClick.AddListener(BuyItem);
        }
        else
        {
            Debug.LogError("Buy button not found in the prefab.");
        }

        // Find the GameManager GameObject in the scene
        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
        {
            // Get the necessary components from the GameManager GameObject
            wholesaleManager = gameManagerObject.GetComponent<WholesaleManager>();
            buyItemHandler = gameManagerObject.GetComponent<BuyItemHandler>();
            inventoryManager = gameManagerObject.GetComponent<InventoryManager>();
        }
        else
        {
            Debug.LogError("GameManager GameObject not found in the scene.");
        }

        // Remove the BuyItem listener from the minus button
        if (minusButton != null)
        {
            minusButton.onClick.RemoveListener(BuyItem);
        }
    }


    private void Start()
    {
        cashDisplay = FindObjectOfType<CashDisplay>();
        UpdateUI();
    }

    private void UpdateUI()
    {
        quantityText.text = quantity.ToString();
        float totalCost = inventoryItem.cost * quantity;
        totalCostText.text = $"£{totalCost.ToString("F2")}";
    }

    public void IncrementQuantity()
    {
        quantity++;
        UpdateUI();
    }

    public void DecrementQuantity()
    {
        if (quantity > 0)
        {
            quantity--;
            UpdateUI();
            Debug.Log("Quantity decremented to: " + quantity); // Add this line
        }
        else
        {
            Debug.Log("Quantity is already at zero.");
        }
    }


    public void BuyItem()
    {
        float totalCost = itemCost * quantity;

        if (cashDisplay != null && cashDisplay.cashOnHand >= totalCost && wholesaleManager != null)
        {
            // Deduct the total cost from cash on hand
            float newCashAmount = cashDisplay.cashOnHand - totalCost;
            cashDisplay.SetCash(newCashAmount); // Update the cash amount in CashDisplay

            //Calculate expenses for the current purchase
            float expenses = totalCost;

            // Think of this as informing the DailySummaryManager about thew Expense
            //dailySummaryManager.RegisterDailyExpenses(expenses); 
/* ABOVE HERE
 * ABOVE HERE
 * ABOVE HEREABOVE HEREABOVE HEREABOVE HERE
 * ABOVE HERE
 * ABOVE HERE
 * ABOVE HERE
 * ABOVE HERE
 * ABOVE HERE
 * ABOVE HERE
 * ABOVE HEREABOVE HERE
 * ABOVE HEREABOVE HEREABOVE HERE
 * ABOVE HEREABOVE HEREABOVE HEREABOVE HEREABOVE HERE
 * ABOVE HEREABOVE HEREABOVE HERE
 * ABOVE HEREABOVE HERE
 * ABOVE HEREABOVE HERE
 * ABOVE HERE
 * ABOVE HERE
 * ABOVE HERE
 * ABOVE HERE
 * 

// Move the purchased item to the inventory through the WholesaleManager
wholesaleManager.MoveItemsToInventory(itemName, quantity);

// Debug log statements for tracking
Debug.Log($"Bought {quantity} {itemName}(s).");
Debug.Log($"Remaining cash: £{newCashAmount}"); // Use the newCashAmount here

// Reset the quantity to 0
quantity = 0;
UpdateUI();

// Update the Inventory UI
if (inventoryManager != null)
{
    inventoryManager.UpdateInventoryUI();
}
}
else
{
Debug.LogError("Insufficient funds or missing WholesaleManager.");
}
}




}
