using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents the daily statistics of the shop.
/// </summary>
[System.Serializable]
public struct DailyStats
{
    /// <summary>
    /// The day number.
    /// </summary>
    public int dayNumber;

    /// <summary>
    /// The number of customers who entered the shop.
    /// </summary>
    public int numberOfCustomers;

    /// <summary>
    /// The number of customers who made a purchase.
    /// </summary>
    public int numberOfPurchasingCustomers;

    /// <summary>
    /// The total revenue for the day.
    /// </summary>
    public float dailyRevenue;

    /// <summary>
    /// The total expenses for the day.
    /// </summary>
    public float dailyExpenses;

    /// <summary>
    /// The total profit for the day.
    /// </summary>
    public float dailyProfit;

    /// <summary>
    /// A dictionary of items sold and their quantities.
    /// </summary>
    public Dictionary<string, int> itemSales;

    /// <summary>
    /// The name of the most profitable customer.
    /// </summary>
    public string mostProfitableCustomer;

    /// <summary>
    /// The highest transaction value of the day.
    /// </summary>
    public float highestTransactionValue;

    /// <summary>
    /// The profit from the most profitable transaction.
    /// </summary>
    public float mostProfitableTransactionProfit;

    /// <summary>
    /// The amount from the most profitable transaction.
    /// </summary>
    public float mostProfitableTransactionAmount;

    /// <summary>
    /// The customer satisfaction rating for the day.
    /// </summary>
    public float customerSatisfaction;

    /// <summary>
    /// The number of stock shortages per customer.
    /// </summary>
    public int stockShortagePerCustomer;

    /// <summary>
    /// The number of stock shortages per item.
    /// </summary>
    public int stockShortagePerItem;
}


/// <summary>
/// Manages the daily summary and statistics of the shop.
/// </summary>
public class DailySummaryManager : MonoBehaviour
{
    /// <summary>
    /// The current day number.
    /// </summary>
    public int currentDay = 0;

    /// <summary>
    /// The list of daily statistics.
    /// </summary>
    private List<DailyStats> dailyStatsList = new List<DailyStats>();

    /// <summary>
    /// The customer spawner.
    /// </summary>
    private CustomerSpawner customerSpawner;

    /// <summary>
    /// The script for the summary prefab.
    /// </summary>
    [SerializeField] private SummaryPrefabScript summaryPrefabScript;

    /// <summary>
    /// Text component for displaying the day number.
    /// </summary>
    [SerializeField] private TextMeshProUGUI dayText;

    /// <summary>
    /// Text component for displaying the number of customers.
    /// </summary>
    [SerializeField] private TextMeshProUGUI numberOfCustomersText;

    /// <summary>
    /// Text component for displaying the number of purchasing customers.
    /// </summary>
    [SerializeField] private TextMeshProUGUI numberOfPurchasingCustomersText;

    /// <summary>
    /// Text component for displaying the daily revenue.
    /// </summary>
    [SerializeField] private TextMeshProUGUI dailyRevenueText;

    /// <summary>
    /// Text component for displaying the daily expenses.
    /// </summary>
    [SerializeField] private TextMeshProUGUI dailyExpensesText;

    /// <summary>
    /// Text component for displaying the daily profit.
    /// </summary>
    [SerializeField] private TextMeshProUGUI dailyProfitText;

    /// <summary>
    /// Text component for displaying the most profitable customer.
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableCustomerText;

    /// <summary>
    /// Text component for displaying the amount from the most profitable transaction.
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableAmountText;

    /// <summary>
    /// Text component for displaying the profit from the most profitable transaction.
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableProfitText;

    /// <summary>
    /// Text component for displaying the highest transaction value.
    /// </summary>
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;

    /// <summary>
    /// Text component for displaying the customer satisfaction rating.
    /// </summary>
    [SerializeField] private TextMeshProUGUI customerSatisfactionText;

    /// <summary>
    /// Text component for displaying the most popular item.
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostPopularItemText;

    /// <summary>
    /// Text component for displaying the least popular item.
    /// </summary>
    [SerializeField] private TextMeshProUGUI leastPopularItemText;

    /// <summary>
    /// Text component for displaying the stock shortage per customer.
    /// </summary>
    [SerializeField] private TextMeshProUGUI stockShortagePerCustomerText;

    /// <summary>
    /// Text component for displaying the stock shortage per item.
    /// </summary>
    [SerializeField] private TextMeshProUGUI stockShortagePerItemText;

    /// <summary>
    /// The overall summary manager.
    /// </summary>
    [SerializeField] private OverallSummaryManager overallSummaryManager;




    /// <summary>
    /// Initializes the daily summary manager.
    /// </summary>
    void Start()
    {
        
        customerSpawner = FindObjectOfType<CustomerSpawner>();
        if (dailyStatsList.Count == 0) // Ensuring that we start with an initial day if no days are present
        {
            
            AddNewDayStats();  // Add the first day with index 0 without incrementing currentDay
        }
        UpdateUI();  // Ensure UI reflects the initial state
        
    }

    /// <summary>
    /// Checks and updates the highest transaction value.
    /// </summary>
    /// <param name="transactionValue">The transaction value.</param>
    /// <param name="customerName">The customer's name.</param>
    /// <param name="transactionProfit">The transaction profit.</param>
    public void CheckAndUpdateHighestTransactionValue(float transactionValue, string customerName, float transactionProfit)
    {
        int lastIndex = dailyStatsList.Count - 1; // Correctly getting the last index
        DailyStats lastStats = dailyStatsList[lastIndex]; // Getting a reference, not a copy

        if (transactionValue > lastStats.highestTransactionValue)
        {
            lastStats.highestTransactionValue = transactionValue;
            lastStats.mostProfitableCustomer = customerName;
            lastStats.mostProfitableTransactionProfit = transactionProfit;

            dailyStatsList[lastIndex] = lastStats; // Updating the list with modified stats

            UpdateUI(); // Explicitly calling UpdateUI to refresh the display
        }
    }




    /// <summary>
    /// Initializes the day.
    /// </summary>
    void InitializeDay()
    {
        if (dailyStatsList.Count < currentDay)
        {
            AddNewDayStats();
        }
    }

    /// <summary>
    /// Initializes item sales for the current day.
    /// </summary>
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






    /// <summary>
    /// Adds new daily statistics for a new day.
    /// </summary>
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

    /// <summary>
    /// Registers a transaction.
    /// </summary>
    /// <param name="customer">The customer making the transaction.</param>
    /// <param name="purchasedItems">The items purchased.</param>
    /// <param name="transactionValue">The value of the transaction.</param>
    /// <param name="transactionProfit">The profit from the transaction.</param>
    public void RegisterTransaction(Customer customer, Dictionary<string, int> purchasedItems, float transactionValue, float transactionProfit)
    {
        int lastIndex = dailyStatsList.Count - 1;
        DailyStats currentDayStats = dailyStatsList[lastIndex];

        foreach (var item in purchasedItems)
        {
            if (currentDayStats.itemSales.ContainsKey(item.Key))
            {
                currentDayStats.itemSales[item.Key] += item.Value;
            }
            else
            {
                currentDayStats.itemSales[item.Key] = item.Value;
            }
        }

        if (!customer.hasPurchasedToday)
        {
            currentDayStats.numberOfPurchasingCustomers++;
            customer.hasPurchasedToday = true;
        }
        currentDayStats.dailyRevenue += transactionValue;
        currentDayStats.dailyProfit += transactionProfit;

        // Check for highest transaction value
        if (transactionValue > currentDayStats.highestTransactionValue)
        {
            currentDayStats.highestTransactionValue = transactionValue;
            currentDayStats.mostProfitableCustomer = customer.customerName;
            currentDayStats.mostProfitableTransactionAmount = transactionValue;
            currentDayStats.mostProfitableTransactionProfit = transactionProfit;
        }

        dailyStatsList[lastIndex] = currentDayStats;
        UpdateUI();
    }

    /// <summary>
    /// Starts a new day.
    /// </summary>
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

    /// <summary>
    /// Prepares for a new day by resetting daily statistics.
    /// </summary>
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

    /// <summary>
    /// Saves the current day's statistics.
    /// </summary>
    private void SaveCurrentDayStats()
    {
        // Save current day stats if needed
        Debug.Log("Stats saved for day " + currentDay);
    }

    /// <summary>
    /// Prepares the day by resetting statistics and adding new day stats.
    /// </summary>
    public void PrepareDay()
    {
        Debug.Log("[DailySummaryManager] PrepareDay called for day " + currentDay);
        if (dailyStatsList.Count < currentDay + 1)
        {
            AddNewDayStats();
        }
        ResetDailyStats();
    }

    /// <summary>
    /// Checks and ends the day if no customers are left.
    /// </summary>
    public void CheckAndEndDay()
    {
        Debug.Log("[DailySummaryManager] CheckAndEndDay called with ActiveCustomers: " + customerSpawner.ActiveCustomers);
        if (customerSpawner.ActiveCustomers == 0)
        {
            EndOfDaySummary();
        }
    }

    /// <summary>
    /// Registers a customer entry.
    /// </summary>
    public void RegisterCustomerEntry()
    {
        if (dailyStatsList.Count == 0) return; // Safety check

        int lastIndex = dailyStatsList.Count - 1;
        DailyStats todayStats = dailyStatsList[lastIndex];
        todayStats.numberOfCustomers++;
        dailyStatsList[lastIndex] = todayStats;
        UpdateUI();
    }

    /// <summary>
    /// Registers customer dissatisfaction.
    /// </summary>
    /// <param name="itemsNotFound">The number of items not found.</param>
    /// <param name="customerCount">The number of customers affected.</param>
    public void RegisterCustomerDissatisfaction(int itemsNotFound, int customerCount)
    {
        DailyStats todayStats = dailyStatsList.Last();
        todayStats.numberOfCustomers += customerCount; // Modify based on your logic
        dailyStatsList[dailyStatsList.Count - 1] = todayStats;
        UpdateUI();
    }

    /// <summary>
    /// Updates daily customer satisfaction.
    /// </summary>
    /// <param name="satisfactionChange">The change in satisfaction.</param>
    public void UpdateDailyCustomerSatisfaction(float satisfactionChange)
    {
        DailyStats todayStats = dailyStatsList.Last();
        todayStats.customerSatisfaction += satisfactionChange;
        todayStats.customerSatisfaction = Mathf.Clamp(todayStats.customerSatisfaction, 0, 100);
        dailyStatsList[dailyStatsList.Count - 1] = todayStats;
        UpdateUI();
    }

    /// <summary>
    /// Updates the UI with the latest statistics.
    /// </summary>
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
            mostPopularItemText.text = DetermineMostPopularItem(currentStats);
            leastPopularItemText.text = DetermineLeastPopularItem(currentStats);
            mostProfitableAmountText.text = $"£{currentStats.mostProfitableTransactionAmount:F2}";
            mostProfitableProfitText.text = $"£{currentStats.mostProfitableTransactionProfit:F2}";
            stockShortagePerCustomerText.text = $"Per Customer: {currentStats.stockShortagePerCustomer}";
            stockShortagePerItemText.text = $"Per Item: {currentStats.stockShortagePerItem}";

            // Now update the OverallSummaryManager UI as well
            if (overallSummaryManager != null)
            {
                overallSummaryManager.UpdateOverallStats();
            }

        }
    }


    /// <summary>
    /// Determines the most popular item of the day.
    /// </summary>
    /// <param name="stats">The daily statistics.</param>
    /// <returns>The most popular item.</returns>
    string DetermineMostPopularItem(DailyStats stats)
    {
        if (stats.itemSales.Count == 0) return "N/A";
        return stats.itemSales.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }

    /// <summary>
    /// Determines the least popular item of the day.
    /// </summary>
    /// <param name="stats">The daily statistics.</param>
    /// <returns>The least popular item.</returns>
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

    /// <summary>
    /// Ends the day and provides a summary.
    /// </summary>
    public void EndOfDaySummary()
    {
        Debug.Log("Day ended: " + currentDay);        
    }

    /// <summary>
    /// Increments the day counter and adds new day statistics.
    /// </summary>
    public void IncrementDay()
    {
        currentDay++;
        AddNewDayStats();
    }

    /// <summary>
    /// Starts a new day without resetting statistics.
    /// </summary>
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

    /// <summary>
    /// Resets the daily statistics.
    /// </summary>
    void ResetDailyStats()
    {
        float lastSatisfaction = dailyStatsList.Count > 0 ? dailyStatsList.Last().customerSatisfaction : 100;
        DailyStats newDayStats = new DailyStats
        {
            dayNumber = currentDay,
            numberOfCustomers = 0,
            dailyRevenue = 0,
            dailyExpenses = (currentDay > 0) ? 0 : (dailyStatsList.Count > 0 ? dailyStatsList.Last().dailyExpenses : 0),
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

    /// <summary>
    /// Gets the daily statistics.
    /// </summary>
    /// <returns>A list of daily statistics.</returns>
    public List<DailyStats> GetDailyStats()
    {
        // Returns a shallow copy of the dailyStatsList to prevent modification from outside
        return new List<DailyStats>(dailyStatsList);
    }

    /// <summary>
    /// Registers daily expenses.
    /// </summary>
    /// <param name="amount">The amount of expenses.</param>
    public void RegisterDailyExpenses(float amount)
    {
        if (dailyStatsList.Count == 0)
            InitializeDay();  // Ensure there's at least one day to work with

        DailyStats todayStats = dailyStatsList.Last();  // Use Last() now that you have included System.Linq
        todayStats.dailyExpenses += amount;
        dailyStatsList[dailyStatsList.Count - 1] = todayStats; // Update the last element
        UpdateUI();
    }

    /// <summary>
    /// Registers stock shortages.
    /// </summary>
    /// <param name="customersAffected">The number of customers affected.</param>
    /// <param name="itemsNotFound">The number of items not found.</param>
    public void RegisterStockShortage(int customersAffected, int itemsNotFound)
    {
        int lastIndex = dailyStatsList.Count - 1;
        DailyStats todayStats = dailyStatsList[lastIndex];

        todayStats.stockShortagePerCustomer += customersAffected;
        todayStats.stockShortagePerItem += itemsNotFound;

        dailyStatsList[lastIndex] = todayStats; // Update the list with modified stats
        UpdateUI(); // Explicitly calling UpdateUI to refresh the display
    }


}
