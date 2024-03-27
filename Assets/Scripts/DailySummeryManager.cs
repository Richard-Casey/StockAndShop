using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class DailySummaryManager : MonoBehaviour
{
    [SerializeField] private GameObject summaryPrefab;
    [SerializeField] private Transform summaryParent;

    // Track daily statistics
    private int numberOfCustomers = 0;
    private Dictionary<string, int> itemSales = new Dictionary<string, int>();
    private float highestTransactionValue = 0f;
    private float mostProfitableTransactionProfit = 0f;
    private string mostProfitableCustomer = "";
    private float dailyProfit = 0f;
    private int customersNotSatisfied = 0;
    private int itemsNotSatisfied = 0;
    private float dailyRevenue = 0f;
    private float dailyExpenses = 0f;

    // Reference to the CustomerSpawner for reputation data
    private CustomerSpawner customerSpawner;

    void Start()
    {
        customerSpawner = FindObjectOfType<CustomerSpawner>();
    }

    public void RegisterCustomerEntry()
    {
        numberOfCustomers++;
    }

    public void RegisterTransaction(Customer customer, float transactionValue, float transactionProfit)
    {
        // Update daily revenue and profit
        dailyRevenue += transactionValue;
        dailyProfit += transactionProfit;

        // Check if this transaction is the highest or most profitable
        if (transactionValue > highestTransactionValue)
        {
            highestTransactionValue = transactionValue;
            mostProfitableCustomer = customer.customerName;  // Assuming highest value is also most profitable
        }

        if (transactionProfit > mostProfitableTransactionProfit)
        {
            mostProfitableTransactionProfit = transactionProfit;
        }

        // Update item sales count
        foreach (var item in customer.GetPurchasedItems())
        {
            if (itemSales.ContainsKey(item.itemName))
            {
                itemSales[item.itemName] += item.quantity;
            }
            else
            {
                itemSales[item.itemName] = item.quantity;
            }
        }
    }

    public void RegisterCustomerDissatisfaction(int itemsNotFound)
    {
        if (itemsNotFound > 0)
        {
            customersNotSatisfied++;
            itemsNotSatisfied += itemsNotFound;
        }
    }

    public void RegisterDailyExpenses(float amount)
    {
        dailyExpenses += amount;
    }

    public void EndOfDaySummary()
    {
        GameObject newSummary = Instantiate(summaryPrefab, summaryParent);
        UpdateSummary(newSummary);
        ResetDailyStats();
    }

    private void UpdateSummary(GameObject summary)
    {
        summary.transform.Find("ShopInfo/NumOfCustomersNum").GetComponent<TextMeshProUGUI>().text = numberOfCustomers.ToString();
        summary.transform.Find("ShopInfo/MostPopularItem").GetComponent<TextMeshProUGUI>().text = GetMostPopularItem();
        summary.transform.Find("ShopInfo/LeastPopularItem").GetComponent<TextMeshProUGUI>().text = GetLeastPopularItem();
        summary.transform.Find("ShopInfo/HighestValue").GetComponent<TextMeshProUGUI>().text = $"£{highestTransactionValue:F2}";
        summary.transform.Find("ShopInfo/MostProfitableTransactionText/CustomerName").GetComponent<TextMeshProUGUI>().text = mostProfitableCustomer;
        summary.transform.Find("ShopInfo/MostProfitableTransactionText/Amount/Value").GetComponent<TextMeshProUGUI>().text = $"£{highestTransactionValue:F2}";
        summary.transform.Find("ShopInfo/MostProfitableTransactionText/Profit/Value").GetComponent<TextMeshProUGUI>().text = $"£{mostProfitableTransactionProfit:F2}";
        summary.transform.Find("ShopInfo/DailyProfit/Amount").GetComponent<TextMeshProUGUI>().text = $"£{dailyProfit:F2}";
        summary.transform.Find("ShopInfo/CustomerSatisfaction").GetComponent<TextMeshProUGUI>().text = $"{customerSpawner.Reputation:F0}%";
        summary.transform.Find("ShopInfo/StockShortages/Amount").GetComponent<TextMeshProUGUI>().text = $"{customersNotSatisfied} Customers";
        summary.transform.Find("ShopInfo/StockShortages/Items").GetComponent<TextMeshProUGUI>().text = $"{itemsNotSatisfied} Items";
        summary.transform.Find("ShopInfo/RevenueValue").GetComponent<TextMeshProUGUI>().text = $"£{dailyRevenue:F2}";
        summary.transform.Find("ShopInfo/ExpensesValue").GetComponent<TextMeshProUGUI>().text = $"£{dailyExpenses:F2}";
    }

    private string GetMostPopularItem()
    {
        return itemSales.OrderByDescending(i => i.Value).FirstOrDefault().Key ?? "N/A";
    }

    private string GetLeastPopularItem()
    {
        return itemSales.OrderBy(i => i.Value).Where(i => i.Value > 0).FirstOrDefault().Key ?? "N/A";
    }

    private void ResetDailyStats()
    {
        numberOfCustomers = 0;
        itemSales.Clear();
        highestTransactionValue = 0f;
        mostProfitableTransactionProfit = 0f;
        mostProfitableCustomer = "";
        dailyProfit = 0f;
        customersNotSatisfied = 0;
        itemsNotSatisfied = 0;
        dailyRevenue = 0f;
        dailyExpenses = 0f;
    }
}
