using UnityEngine;
using TMPro;
using UnityEngine.UI;  // This is essential for accessing the Button class
using System.Collections.Generic;
using System.Linq;



public class DailySummaryManager : MonoBehaviour
{

    // Day Tracking
    public int currentDay = 1; // Start from day 1
    private int customersNotSatisfied = 0;
    //private string mostProfitableCustomer = "";

    // Reference to the CustomerSpawner for reputation data
    private CustomerSpawner customerSpawner;
    private float dailyExpenses = 0f;
    private float dailyProfit = 0f;
    private float dailyRevenue = 0f;
    private float highestTransactionValue = 0f;
    private GameObject instantiatedSummaryPrefab;
    private Dictionary<string, int> itemSales = new Dictionary<string, int>();
    private int itemsNotSatisfied = 0;
    private string mostProfitableCustomer = "";
    private float mostProfitableTransactionProfit = 0f;


    // Track daily statistics
    private int numberOfCustomers = 0;
    [SerializeField] private Transform summaryParent;
    [SerializeField] private GameObject summaryPrefab;
    private SummaryPrefabScript summaryScript;
    private List<GameObject> dailySummaries = new List<GameObject>();
    private int currentSummaryIndex = 0;

    [SerializeField] private Button prevDayButton;
    [SerializeField] private Button nextDayButton;
    public bool CurrentDayIsNew() { return dailySummaries.Count < currentDay; }


    private string GetLeastPopularItem()
    {
        // Filter out items that haven't been sold
        var soldItems = itemSales.Where(kv => kv.Value > 0);

        // Find the item with the lowest quantity sold
        var leastPopular = soldItems.OrderBy(kv => kv.Value).FirstOrDefault().Key;

        // Return the name of the least popular item
        return leastPopular ?? "N/A";
    }

    private string GetMostPopularItem()
    {
        return itemSales.OrderByDescending(i => i.Value).FirstOrDefault().Key ?? "N/A";
    }

    private void IncrementDay()
    {
        currentDay++;
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

    private void Start()
    {
        customerSpawner = FindObjectOfType<CustomerSpawner>();
        InstantiatedSummaryPrefab();
        UpdateButtonVisibility(); // Initial button visibility setup
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

    public void EndOfDaySummary()
    {
        GameObject newSummary = Instantiate(summaryPrefab, summaryParent);
        newSummary.SetActive(false); // Initially inactive
        dailySummaries.Add(newSummary);

        if (dailySummaries.Count > 1)
        {
            dailySummaries[currentSummaryIndex].SetActive(false);
        }
        currentSummaryIndex = dailySummaries.Count - 1;

        if (currentSummaryIndex > 0)
        {
            var summaryScript = dailySummaries[currentSummaryIndex - 1].GetComponent<SummaryPrefabScript>();
            if (summaryScript)
            {
                summaryScript.UpdateData(currentDay, numberOfCustomers, GetMostPopularItem(), GetLeastPopularItem(), highestTransactionValue, mostProfitableCustomer, highestTransactionValue, mostProfitableTransactionProfit, dailyProfit, customerSpawner.Reputation, customersNotSatisfied, itemsNotSatisfied, dailyRevenue, dailyExpenses);
            }
        }

        ResetDailyStats();
        IncrementDay();
    }

    public void RegisterCustomerDissatisfaction(int itemsNotFound, int customerCount)
    {
        customersNotSatisfied += customerCount; // Increment for each customer that didn't find items
        itemsNotSatisfied += itemsNotFound; // Add the number of items not found
        Debug.Log($"Customer could not find {itemsNotFound} items.");
        UpdateSummaryInfo(); // Update the UI or other systems immediately
    }

    public void RegisterCustomerEntry()
    {
        numberOfCustomers++;
        summaryScript.UpdateNumberOfCustomers(numberOfCustomers);
    }

    public void RegisterDailyExpenses(float amount)
    {
        dailyExpenses += amount;
        Debug.Log($"Expenses updated: New total expenses = £{dailyExpenses:F2}");
        UpdateSummaryInfo(); // Ensure this method updates the relevant UI or summary display
    }

    public void RegisterTransaction(Customer customer, Dictionary<string, int> purchasedItems, float transactionValue, float transactionProfit)
    {
        // Update daily revenue and profit
        dailyRevenue += transactionValue;
        dailyProfit += transactionProfit;

        //Update most profitable customer field
        if (transactionProfit > mostProfitableTransactionProfit)
        {
            mostProfitableTransactionProfit = transactionProfit;
            mostProfitableCustomer = customer.customerName;
        }

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

        if (transactionValue > highestTransactionValue)
        {
            highestTransactionValue = transactionValue;
        }

        UpdateSummaryInfo();
    }

    public void UpdateSummaryInfo()
    {
        if (summaryScript != null) // Fix variable name
        {
            foreach (var entry in itemSales)
            {
                Debug.Log($"Item: {entry.Key}, Quantity: {entry.Value}");
            }
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

    private void UpdateButtonVisibility()
    {
        if (prevDayButton != null && nextDayButton != null)
        {
            prevDayButton.gameObject.SetActive(currentSummaryIndex > 0);
            nextDayButton.gameObject.SetActive(currentSummaryIndex < dailySummaries.Count - 1);
        }
    }

    public void ShowNextDay()
    {
        if (currentSummaryIndex < dailySummaries.Count - 1)
        {
            dailySummaries[currentSummaryIndex].SetActive(false);
            currentSummaryIndex++;
            dailySummaries[currentSummaryIndex].SetActive(true);
            UpdateButtonVisibility();
        }
    }

    public void ShowPreviousDay()
    {
        if (currentSummaryIndex > 0)
        {
            dailySummaries[currentSummaryIndex].SetActive(false);
            currentSummaryIndex--;
            dailySummaries[currentSummaryIndex].SetActive(true);
            UpdateButtonVisibility();
        }
    }

    public void PrepareNewDay()
    {
        if (dailySummaries.Count < currentDay)
        {
            InstantiateNewDaySummary();
        }
        ActivateCurrentDaySummary();
    }

    private void ActivateCurrentDaySummary()
    {
        foreach (var summary in dailySummaries)
        {
            summary.SetActive(false);
        }
        if (currentDay <= dailySummaries.Count)
        {
            dailySummaries[currentDay - 1].SetActive(true);
            currentSummaryIndex = currentDay - 1;
        }
    }

    private void InstantiateNewDaySummary()
    {
        GameObject newSummary = Instantiate(summaryPrefab, summaryParent);
        newSummary.SetActive(true);  // Ensure it's set to active immediately
        dailySummaries.Add(newSummary);
    }


}
