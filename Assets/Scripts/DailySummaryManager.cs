using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// This is the data stored and therefore displayed on the summary panel
[System.Serializable]
public struct DailyStats
{
    public int dayNumber;
    public int numberOfCustomers;
    public int numberOfPurchasingCustomers;
    public float dailyRevenue;
    public float dailyExpenses;
    public float dailyProfit;
    public Dictionary<string, int> itemSales; // For tracking item sales
    public string mostProfitableCustomer;
    public float highestTransactionValue;
    public float mostProfitableTransactionProfit;
    public float customerSatisfaction; // Stores customer satisfaction percentage
}


public class DailySummaryManager : MonoBehaviour
{
    public int currentDay = 0;
    private List<DailyStats> dailyStatsList = new List<DailyStats>();
    private CustomerSpawner customerSpawner;
    [SerializeField] private SummaryPrefabScript summaryPrefabScript;


    // UI References
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI numberOfCustomersText;
    [SerializeField] private TextMeshProUGUI numberOfPurchasingCustomersText;
    [SerializeField] private TextMeshProUGUI dailyRevenueText;
    [SerializeField] private TextMeshProUGUI dailyExpensesText;
    [SerializeField] private TextMeshProUGUI dailyProfitText;
    [SerializeField] private TextMeshProUGUI mostProfitableCustomerText;
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;
    [SerializeField] private TextMeshProUGUI customerSatisfactionText;
    [SerializeField] private TextMeshProUGUI mostPopularItemText;
    [SerializeField] private TextMeshProUGUI leastPopularItemText;




    void Start()
    {
        
        customerSpawner = FindObjectOfType<CustomerSpawner>();
        if (dailyStatsList.Count == 0) // Ensuring that we start with an initial day if no days are present
        {
            
            AddNewDayStats();  // Add the first day with index 0 without incrementing currentDay
        }
        UpdateUI();  // Ensure UI reflects the initial state
        
    }


    void InitializeDay()
    {
        if (dailyStatsList.Count < currentDay)
        {
            AddNewDayStats();
        }
    }

    public void InitializeItemSales()
    {
        var currentStats = dailyStatsList.Last();
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();

        // Assuming inventoryItems is a list of all items that could potentially be sold
        foreach (var item in inventoryManager.inventoryItems)
        {
            if (!currentStats.itemSales.ContainsKey(item.itemName))
            {
                currentStats.itemSales[item.itemName] = 0;  // Initialize with zero sales
            }
        }
    }







    public void AddNewDayStats()
    {
        DailyStats newDayStats = new DailyStats
        {
            dayNumber = currentDay,
            numberOfCustomers = 0,
            dailyRevenue = 0,
            dailyExpenses = 0,
            dailyProfit = 0,
            itemSales = new Dictionary<string, int>(),
            mostProfitableCustomer = "",
            highestTransactionValue = 0,
            mostProfitableTransactionProfit = 0,
            customerSatisfaction = 100  // Assuming a starting satisfaction score
        };
        dailyStatsList.Add(newDayStats);
        InitializeItemSales();  // Ensure all available items are tracked from the start of the day
        UpdateUI();
    }



    public void RegisterTransaction(Customer customer, Dictionary<string, int> purchasedItems, float transactionValue, float transactionProfit)
    {
        DailyStats todayStats = dailyStatsList.Last();

        foreach (var item in purchasedItems)
        {
            // Ensure the item is in the dictionary, then increment its count
            if (todayStats.itemSales.ContainsKey(item.Key))
            {
                todayStats.itemSales[item.Key] += item.Value;
            }
        }

        // Increment the number of purchasing customers if there are items in the purchase.
        if (purchasedItems.Count > 0)
        {
            todayStats.numberOfPurchasingCustomers++;
            Debug.Log($"Number of purchasing customers updated: {todayStats.numberOfPurchasingCustomers}");
        }

        // Update item sales count.
        foreach (var item in purchasedItems)
        {
            if (todayStats.itemSales.ContainsKey(item.Key))
            {
                todayStats.itemSales[item.Key] += item.Value;
                Debug.Log($"Updated {item.Key}: {todayStats.itemSales[item.Key]}");
            }
            else
            {
                todayStats.itemSales.Add(item.Key, item.Value);
                Debug.Log($"Added new item {item.Key}: {item.Value}");
            }
        }

        // Update for the most profitable transaction if this transaction's value is higher than the current highest.
        if (transactionValue > todayStats.highestTransactionValue)
        {
            todayStats.highestTransactionValue = transactionValue;
            todayStats.mostProfitableCustomer = customer.customerName;
            todayStats.mostProfitableTransactionProfit = transactionProfit;
            Debug.Log($"New highest transaction: {transactionValue} by {customer.customerName}");
        }

        // Update the day's revenue and profit.
        todayStats.dailyRevenue += transactionValue;
        todayStats.dailyProfit += transactionProfit;
        Debug.Log($"Total Revenue for the day: {todayStats.dailyRevenue}, Total Profit for the day: {todayStats.dailyProfit}");

        // Replace the last day's stats with updated stats.
        dailyStatsList[dailyStatsList.Count - 1] = todayStats;

        UpdateUI();
    }



    public void StartNewDay()
    {
        if (currentDay == 0)
        {
            // Handle first day without resetting expenses
            Debug.Log("First day started without resetting daily expenses.");
        }
        else
        {
            PrepareForNewDay();
        }
    }

    public void PrepareForNewDay()
    {
        // This method resets the stats for any new day after the first
        if (currentDay > 0)
        { // Make sure we're not on the first day
            ResetDailyStats(); // Reset all daily stats
        }
        Debug.Log("Prepared for a new day, stats reset as needed.");
        UpdateUI();
    }


    private void SaveCurrentDayStats()
    {
        // Save current day stats if needed
        Debug.Log("Stats saved for day " + currentDay);
    }


    public void PrepareDay()
    {
        Debug.Log("[DailySummaryManager] PrepareDay called for day " + currentDay);
        if (dailyStatsList.Count < currentDay + 1)
        {
            AddNewDayStats();
        }
        ResetDailyStats();
    }


    public void CheckAndEndDay()
    {
        Debug.Log("[DailySummaryManager] CheckAndEndDay called with ActiveCustomers: " + customerSpawner.ActiveCustomers);
        if (customerSpawner.ActiveCustomers == 0)
        {
            EndOfDaySummary();
        }
    }

    public void RegisterCustomerEntry()
    {
        DailyStats todayStats = dailyStatsList[currentDay - 1];
        todayStats.numberOfCustomers++;
        dailyStatsList[currentDay - 1] = todayStats;
        UpdateUI();
    }

    public void RegisterCustomerDissatisfaction(int itemsNotFound, int customerCount)
    {
        DailyStats todayStats = dailyStatsList.Last();
        todayStats.numberOfCustomers += customerCount; // Modify based on your logic
        dailyStatsList[dailyStatsList.Count - 1] = todayStats;
        UpdateUI();
    }

    public void UpdateDailyCustomerSatisfaction(float satisfactionChange)
    {
        DailyStats todayStats = dailyStatsList.Last();
        todayStats.customerSatisfaction += satisfactionChange;
        todayStats.customerSatisfaction = Mathf.Clamp(todayStats.customerSatisfaction, 0, 100);
        dailyStatsList[dailyStatsList.Count - 1] = todayStats;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (dailyStatsList.Count > 0)
        {
            var currentStats = dailyStatsList.Last();
            dayText.text = currentStats.dayNumber.ToString();
            numberOfCustomersText.text = $"{currentStats.numberOfCustomers}";
            numberOfPurchasingCustomersText.text = $"{currentStats.numberOfPurchasingCustomers}";
            dailyRevenueText.text = $"£{currentStats.dailyRevenue:F2}";
            dailyExpensesText.text = $"£{currentStats.dailyExpenses:F2}";
            dailyProfitText.text = $"£{currentStats.dailyProfit:F2}";
            mostProfitableCustomerText.text = currentStats.mostProfitableCustomer;
            highestTransactionValueText.text = $"£{currentStats.highestTransactionValue:F2}";
            customerSatisfactionText.text = $"{currentStats.customerSatisfaction:F0}%";

            // Directly update the most and least popular items
            mostPopularItemText.text = DetermineMostPopularItem(currentStats);
            leastPopularItemText.text = DetermineLeastPopularItem(currentStats);
            Debug.Log("Most Popular Item: " + mostPopularItemText.text);
            Debug.Log("Least Popular Item: " + leastPopularItemText.text);
        }
    }

    string DetermineMostPopularItem(DailyStats stats)
    {
        if (stats.itemSales.Count == 0) return "N/A";
        return stats.itemSales.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }

    string DetermineLeastPopularItem(DailyStats stats)
    {
        // Retrieve initial quantities on the shelf from ShelfManager
        var shelfQuantities = FindObjectOfType<ShelfManager>().GetShelfItemQuantities();

        // Calculate the total units involved (initial stock minus remaining stock to find out total units moved)
        Dictionary<string, int> totalUnitsMoved = new Dictionary<string, int>();

        foreach (var item in shelfQuantities)
        {
            int unitsSold = stats.itemSales.ContainsKey(item.Key) ? stats.itemSales[item.Key] : 0;
            totalUnitsMoved[item.Key] = unitsSold;  // Only count the units actually sold
        }

        // Finding the least popular item by identifying the item with the lowest units moved
        if (totalUnitsMoved.Count == 0)
            return "N/A";

        var leastPopular = totalUnitsMoved.OrderBy(x => x.Value).ThenByDescending(x => shelfQuantities[x.Key]).FirstOrDefault();
        return leastPopular.Key;  // Assumes there's always at least one item
    }








    public void EndOfDaySummary()
    {
        Debug.Log("Day ended: " + currentDay);        
    }

    public void IncrementDay()
    {
        currentDay++;
        AddNewDayStats();
    }

    public void StartNewDayWithoutReset()
    {
        Debug.Log("First day starts, keeping all previous stats including expenses.");
        // Assuming 'currentDay' is still 0 and will be incremented after this method finishes
        if (currentDay == 0)
        {
            // We are starting the first day, so do not reset expenses or other stats
            UpdateUI(); // Update the UI to reflect the current stats without resetting
        }
        else
        {
            // For subsequent days, we reset the stats
            PrepareForNewDay();
        }
    }

    void ResetDailyStats()
    {
        float lastSatisfaction = dailyStatsList.Count > 0 ? dailyStatsList.Last().customerSatisfaction : 100;
        DailyStats newDayStats = new DailyStats
        {
            dayNumber = currentDay,
            numberOfCustomers = 0,
            dailyRevenue = 0,
            dailyExpenses = (currentDay > 0) ? 0 : dailyStatsList.Last().dailyExpenses,
            dailyProfit = 0,
            itemSales = new Dictionary<string, int>(),
            mostProfitableCustomer = "",
            highestTransactionValue = 0,
            mostProfitableTransactionProfit = 0,
            customerSatisfaction = lastSatisfaction
        };

        dailyStatsList.Add(newDayStats);
        UpdateUI();
    }

    public void RegisterDailyExpenses(float amount)
    {
        if (dailyStatsList.Count == 0)
            InitializeDay();  // Ensure there's at least one day to work with

        DailyStats todayStats = dailyStatsList.Last();  // Use Last() now that you have included System.Linq
        todayStats.dailyExpenses += amount;
        dailyStatsList[dailyStatsList.Count - 1] = todayStats; // Update the last element
        UpdateUI();
    }
}
