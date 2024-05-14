using UnityEngine;
using TMPro;
using System.Collections.Generic; // Make sure to include this for using Lists

/// <summary>
/// Generates a receipt for a customer's purchases.
/// </summary>
public class ReceiptGenerator : MonoBehaviour
{
    /// <summary>
    /// UI text component for displaying the customer's name.
    /// </summary>
    public TextMeshProUGUI customerNameText;

    /// <summary>
    /// UI text component for displaying the individual costs of items.
    /// </summary>
    public TextMeshProUGUI individualCostsText;

    /// <summary>
    /// UI text component for displaying the list of items.
    /// </summary>
    public TextMeshProUGUI itemsListText;

    /// <summary>
    /// UI text component for displaying the total cost.
    /// </summary>
    public TextMeshProUGUI totalCostText;

    /// <summary>
    /// UI text component for displaying the total profit.
    /// </summary>
    public TextMeshProUGUI totalProfitText;

    /// <summary>
    /// Generates a receipt for the specified customer.
    /// </summary>
    /// <param name="customer">The customer for whom the receipt is generated.</param>
    public void GenerateReceipt(Customer customer)
    {
        // Reset previous receipt data
        itemsListText.text = "";
        individualCostsText.text = "";
        float totalCost = 0;
        float totalProfit = 0;

        // Loop through each purchased item to populate the receipt
        foreach (var item in customer.GetPurchasedItems())
        {
            // Assuming item has properties: itemName, quantity, and price
            itemsListText.text += $"{item.itemName} x{item.quantity}\n";
            individualCostsText.text += $"£{item.price:F2}\n";

            totalCost += item.price * item.quantity;

            // Placeholder for profit calculation; adjust as needed
            // float itemCost = 0; // Example placeholder value
            // totalProfit += (item.price - itemCost) * item.quantity;
        }

        customerNameText.text = customer.customerName;
        totalCostText.text = $"£{totalCost:F2}";
        totalProfitText.text = $"£{totalProfit:F2}";
    }

}
