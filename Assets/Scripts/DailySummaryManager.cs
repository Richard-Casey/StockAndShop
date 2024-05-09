using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public struct DailyStats
{
    /// <summary>
    /// The day number
    /// </summary>
    public int dayNumber;
    /// <summary>
    /// The number of customers
    /// </summary>
    public int numberOfCustomers;
    /// <summary>
    /// The number of purchasing customers
    /// </summary>
    public int numberOfPurchasingCustomers;
    /// <summary>
    /// The daily revenue
    /// </summary>
    public float dailyRevenue;
    /// <summary>
    /// The daily expenses
    /// </summary>
    public float dailyExpenses;
    /// <summary>
    /// The daily profit
    /// </summary>
    public float dailyProfit;
    /// <summary>
    /// The item sales
    /// </summary>
    public Dictionary<string, int> itemSales;
    /// <summary>
    /// The most profitable customer
    /// </summary>
    public string mostProfitableCustomer;
    /// <summary>
    /// The highest transaction value
    /// </summary>
    public float highestTransactionValue;
    /// <summary>
    /// The most profitable transaction profit
    /// </summary>
    public float mostProfitableTransactionProfit;
    /// <summary>
    /// The most profitable transaction amount
    /// </summary>
    public float mostProfitableTransactionAmount;
    /// <summary>
    /// The customer satisfaction
    /// </summary>
    public float customerSatisfaction;
    /// <summary>
    /// The stock shortage per customer
    /// </summary>
    public int stockShortagePerCustomer;
    /// <summary>
    /// The stock shortage per item
    /// </summary>
    public int stockShortagePerItem;
}




/// <summary>
/// 
/// </summary>
public class DailySummaryManager : MonoBehaviour
{
    /// <summary>
    /// The current day
    /// </summary>
    public int currentDay = 0;
    /// <summary>
    /// The daily stats list
    /// </summary>
    private List<DailyStats> dailyStatsList = new List<DailyStats>();
    /// <summary>
    /// The customer spawner
    /// </summary>
    private CustomerSpawner customerSpawner;
    /// <summary>
    /// The summary prefab script
    /// </summary>
    [SerializeField] private SummaryPrefabScript summaryPrefabScript;


    // UI References
    /// <summary>
    /// The day text
    /// </summary>
    [SerializeField] private TextMeshProUGUI dayText;
    /// <summary>
    /// The number of customers text
    /// </summary>
    [SerializeField] private TextMeshProUGUI numberOfCustomersText;
    /// <summary>
    /// The number of purchasing customers text
    /// </summary>
    [SerializeField] private TextMeshProUGUI numberOfPurchasingCustomersText;
    /// <summary>
    /// The daily revenue text
    /// </summary>
    [SerializeField] private TextMeshProUGUI dailyRevenueText;
    /// <summary>
    /// The daily expenses text
    /// </summary>
    [SerializeField] private TextMeshProUGUI dailyExpensesText;
    /// <summary>
    /// The daily profit text
    /// </summary>
    [SerializeField] private TextMeshProUGUI dailyProfitText;
    /// <summary>
    /// The most profitable customer text
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableCustomerText;
    /// <summary>
    /// The most profitable amount text
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableAmountText;
    /// <summary>
    /// The most profitable profit text
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableProfitText;
    /// <summary>
    /// The highest transaction value text
    /// </summary>
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;
    /// <summary>
    /// The customer satisfaction text
    /// </summary>
    [SerializeField] private TextMeshProUGUI customerSatisfactionText;
    /// <summary>
    /// The most popular item text
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostPopularItemText;
    /// <summary>
    /// The least popular item text
    /// </summary>
    [SerializeField] private TextMeshProUGUI leastPopularItemText;
    /// <summary>
    /// The stock shortage per customer text
    /// </summary>
    [SerializeField] private TextMeshProUGUI stockShortagePerCustomerText;
    /// <summary>
    /// The stock shortage per item text
    /// </summary>
    [SerializeField] private TextMeshProUGUI stockShortagePerItemText;
    /// <summary>
    /// The overall summary manager
    /// </summary>
    [SerializeField] private OverallSummaryManager overallSummaryManager;





    /// <summary>
    /// Starts this instance.
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
    /// Checks the and update highest transaction value.
    /// </summary>
    /// <param name="transactionValue">The transaction value.</param>
    /// <param name="customerName">Name of the customer.</param>
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
    /// Initializes the item sales.
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
    /// Adds the new day stats.
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
    /// Registers the transaction.
    /// </summary>
    /// <param name="customer">The customer.</param>
    /// <param name="purchasedItems">The purchased items.</param>
    /// <param name="transactionValue">The transaction value.</param>
    /// <param name="transactionProfit">The transaction profit.</param>
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





    //public void RegisterTransaction(Customer customer, Dictionary<string, int> purchasedItems, float transactionValue, float transactionProfit)
    //{
    //    int lastIndex = dailyStatsList.Count - 1;
    //    DailyStats currentDayStats = dailyStatsList[lastIndex];

    //    // Registering the sales of each item
    //    foreach (var item in purchasedItems)
    //    {
    //        if (currentDayStats.itemSales.ContainsKey(item.Key))
    //            currentDayStats.itemSales[item.Key] += item.Value;
    //        else
    //            currentDayStats.itemSales[item.Key] = item.Value;
    //    }

    //    currentDayStats.numberOfPurchasingCustomers++;
    //    currentDayStats.dailyRevenue += transactionValue;
    //    currentDayStats.dailyProfit += transactionProfit;

    //    // Check for highest transaction value
    //    if (transactionValue > currentDayStats.highestTransactionValue)
    //    {
    //        currentDayStats.highestTransactionValue = transactionValue;
    //        // Other updates can be done here if needed
    //    }

    //    // Check for most profitable transaction
    //    if (transactionProfit > currentDayStats.mostProfitableTransactionProfit)
    //    {
    //        currentDayStats.mostProfitableTransactionProfit = transactionProfit;
    //        currentDayStats.mostProfitableCustomer = customer.customerName;
    //        // Assuming you want to track the total amount as well for the most profitable transaction
    //        currentDayStats.mostProfitableTransactionAmount = transactionValue;
    //    }

    //    // Update the list with new stats
    //    dailyStatsList[lastIndex] = currentDayStats;

    //    // Update UI
    //    UpdateUI();
    //}




    /// <summary>
    /// Starts the new day.
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
    /// Prepares for new day.
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
    /// Saves the current day stats.
    /// </summary>
    private void SaveCurrentDayStats()
    {
        // Save current day stats if needed
        Debug.Log("Stats saved for day " + currentDay);
    }


    /// <summary>
    /// Prepares the day.
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
    /// Checks the and end day.
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
    /// Registers the customer entry.
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
    /// Registers the customer dissatisfaction.
    /// </summary>
    /// <param name="itemsNotFound">The items not found.</param>
    /// <param name="customerCount">The customer count.</param>
    public void RegisterCustomerDissatisfaction(int itemsNotFound, int customerCount)
    {
        DailyStats todayStats = dailyStatsList.Last();
        todayStats.numberOfCustomers += customerCount; // Modify based on your logic
        dailyStatsList[dailyStatsList.Count - 1] = todayStats;
        UpdateUI();
    }

    /// <summary>
    /// Updates the daily customer satisfaction.
    /// </summary>
    /// <param name="satisfactionChange">The satisfaction change.</param>
    public void UpdateDailyCustomerSatisfaction(float satisfactionChange)
    {
        DailyStats todayStats = dailyStatsList.Last();
        todayStats.customerSatisfaction += satisfactionChange;
        todayStats.customerSatisfaction = Mathf.Clamp(todayStats.customerSatisfaction, 0, 100);
        dailyStatsList[dailyStatsList.Count - 1] = todayStats;
        UpdateUI();
    }

    /// <summary>
    /// Updates the UI.
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
    /// Determines the most popular item.
    /// </summary>
    /// <param name="stats">The stats.</param>
    /// <returns></returns>
    string DetermineMostPopularItem(DailyStats stats)
    {
        if (stats.itemSales.Count == 0) return "N/A";
        return stats.itemSales.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }

    /// <summary>
    /// Determines the least popular item.
    /// </summary>
    /// <param name="stats">The stats.</param>
    /// <returns></returns>
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
    /// Ends the of day summary.
    /// </summary>
    public void EndOfDaySummary()
    {
        Debug.Log("Day ended: " + currentDay);        
    }

    /// <summary>
    /// Increments the day.
    /// </summary>
    public void IncrementDay()
    {
        currentDay++;
        AddNewDayStats();
    }

    /// <summary>
    /// Starts the new day without reset.
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
    /// Resets the daily stats.
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
    /// Gets the daily stats.
    /// </summary>
    /// <returns></returns>
    public List<DailyStats> GetDailyStats()
    {
        // Returns a shallow copy of the dailyStatsList to prevent modification from outside
        return new List<DailyStats>(dailyStatsList);
    }


    /// <summary>
    /// Registers the daily expenses.
    /// </summary>
    /// <param name="amount">The amount.</param>
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
    /// Registers the stock shortage.
    /// </summary>
    /// <param name="customersAffected">The customers affected.</param>
    /// <param name="itemsNotFound">The items not found.</param>
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
