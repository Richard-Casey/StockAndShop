using UnityEngine;
using TMPro;
using System.Collections.Generic; // Make sure to include this for using Lists

public class ReceiptGenerator : MonoBehaviour
{
    public TextMeshProUGUI customerNameText;
    public TextMeshProUGUI individualCostsText;
    public TextMeshProUGUI itemsListText;
    public TextMeshProUGUI totalCostText;
    public TextMeshProUGUI totalProfitText;

    // This single method is responsible for generating the receipt
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
