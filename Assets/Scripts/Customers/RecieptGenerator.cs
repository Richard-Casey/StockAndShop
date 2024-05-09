using UnityEngine;
using TMPro;
using System.Collections.Generic; // Make sure to include this for using Lists

/// <summary>
/// 
/// </summary>
public class ReceiptGenerator : MonoBehaviour
{
    /// <summary>
    /// The customer name text
    /// </summary>
    public TextMeshProUGUI customerNameText;
    /// <summary>
    /// The individual costs text
    /// </summary>
    public TextMeshProUGUI individualCostsText;
    /// <summary>
    /// The items list text
    /// </summary>
    public TextMeshProUGUI itemsListText;
    /// <summary>
    /// The total cost text
    /// </summary>
    public TextMeshProUGUI totalCostText;
    /// <summary>
    /// The total profit text
    /// </summary>
    public TextMeshProUGUI totalProfitText;

    // This single method is responsible for generating the receipt
    /// <summary>
    /// Generates the receipt.
    /// </summary>
    /// <param name="customer">The customer.</param>
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
