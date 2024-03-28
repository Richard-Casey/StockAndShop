using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class DailySummaryManager : MonoBehaviour
{
    [SerializeField] private GameObject summaryPrefab;
    [SerializeField] private Transform summaryParent;
    private GameObject instantiatedSummaryPrefab;
    private SummaryPrefabScript summaryScript;


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

    // Day Tracking
    private int currentDay = 1; // Start from day 1

    // Dictionary to track sales of each item
    //private Dictionary<string, int> itemSales = new Dictionary<string, int>();



    void Start()
    {
        customerSpawner = FindObjectOfType<CustomerSpawner>();
        InstantiatedSummaryPrefab();
    }

    private void InstantiatedSummaryPrefab()
    {
        instantiatedSummaryPrefab = Instantiate(summaryPrefab, summaryParent);

        instantiatedSummaryPrefab.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        summaryScript = instantiatedSummaryPrefab.GetComponent<SummaryPrefabScript>();
        if (summaryScript == null)
        {
            Debug.LogError("SummaryPrefabScript component not found on the instantiated summary prefab.");
        }
    }

    public void UpdateSummaryInfo()
    {
        if (summaryScript != null) // Fix variable name
        {
            summaryScript.UpdateData( // Fix variable name
                currentDay,
                numberOfCustomers,
                GetMostPopularItem(),
                GetLeastPopularItem(),
                highestTransactionValue,
                mostProfitableCustomer,
                highestTransactionValue,
                mostProfitableTransactionProfit,
                dailyProfit,
                customerSpawner.Reputation,
                customersNotSatisfied,
                itemsNotSatisfied,
                dailyRevenue,
                dailyExpenses
            );
        }
    }


    public void RegisterCustomerEntry()
    {
        numberOfCustomers++;
        summaryScript.UpdateNumberOfCustomers(numberOfCustomers);
    }

    public void RegisterTransaction(Customer customer, Dictionary<string, int> purchasedItems, float transactionValue, float transactionProfit)
    {
        // Update daily revenue and profit
        dailyRevenue += transactionValue;
        dailyProfit += transactionProfit;

        // Update item sales count
        foreach (var item in purchasedItems)
        {
            // Update item sales dictionary
            if (itemSales.ContainsKey(item.Key))
            {
                itemSales[item.Key] += item.Value;
            }
            else
            {
                itemSales[item.Key] = item.Value;
            }
        }

        // Update most popular item
        UpdateMostPopularItem(); // Call the method to update the most popular item in the UI
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
        //Debug.Log("EndOfDaySummary is being called");
        GameObject newSummary = Instantiate(summaryPrefab, summaryParent);
        // Get the SummaryPrefabScript component from the instantiated prefab
        SummaryPrefabScript summaryScript = newSummary.GetComponent<SummaryPrefabScript>();
        if (summaryScript != null)
        {
            // Call UpdateData on the script with the day's summary data
            summaryScript.UpdateData(
                // Assuming you have a way to track the current day
                day: currentDay,
                numberOfCustomers: numberOfCustomers,
                mostPopular: GetMostPopularItem(),
                leastPopular: GetLeastPopularItem(),
                highestTransaction: highestTransactionValue,
                profitableCustomer: mostProfitableCustomer,
                profitableAmount: highestTransactionValue, // Assuming this is the amount for the most profitable transaction
                profitableProfit: mostProfitableTransactionProfit,
                profit: dailyProfit,
                satisfaction: customerSpawner.Reputation, // Directly using the reputation as customer satisfaction
                shortagesCustomers: customersNotSatisfied,
                shortagesItems: itemsNotSatisfied,
                revenue: dailyRevenue,
                expenses: dailyExpenses
            );
        }
        else
        {
            Debug.LogError("SummaryPrefabScript component not found on the instantiated summary prefab.");
        }

        ResetDailyStats();
        IncrementDay();
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
        // Filter out items that haven't been sold
        var soldItems = itemSales.Where(kv => kv.Value > 0);

        // Find the item with the lowest quantity sold
        var leastPopular = soldItems.OrderBy(kv => kv.Value).FirstOrDefault().Key;

        // Return the name of the least popular item
        return leastPopular ?? "N/A";
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

    private void IncrementDay()
    {
        currentDay++;
    }

    private void UpdateMostPopularItem()
    {
        if (itemSales.Count > 0)
        {
            // Find the item with the highest quantity sold
            string mostPopular = itemSales.OrderByDescending(kv => kv.Value).First().Key;

            // Update the UI text field with the most popular item
            summaryScript.UpdateMostPopularItem(mostPopular);
        }
    }
}
