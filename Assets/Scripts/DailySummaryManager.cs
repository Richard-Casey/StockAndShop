using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct DailyStats
{
    public int dayNumber;
    public int numberOfCustomers;
    public int numberOfPurchasingCustomers;
    public float dailyRevenue;
    public float dailyExpenses;
    public float dailyProfit;
    public Dictionary<string, int> itemSales;
    public string mostProfitableCustomer;
    public float highestTransactionValue;
    public float mostProfitableTransactionProfit;
    public float mostProfitableTransactionAmount;
    public float customerSatisfaction;
    public int stockShortagePerCustomer;
    public int stockShortagePerItem;
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
    [SerializeField] private TextMeshProUGUI mostProfitableAmountText;
    [SerializeField] private TextMeshProUGUI mostProfitableProfitText;
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;
    [SerializeField] private TextMeshProUGUI customerSatisfactionText;
    [SerializeField] private TextMeshProUGUI mostPopularItemText;
    [SerializeField] private TextMeshProUGUI leastPopularItemText;
    [SerializeField] private TextMeshProUGUI stockShortagePerCustomerText;
    [SerializeField] private TextMeshProUGUI stockShortagePerItemText;





    void Start()
    {
        
        customerSpawner = FindObjectOfType<CustomerSpawner>();
        if (dailyStatsList.Count == 0) // Ensuring that we start with an initial day if no days are present
        {
            
            AddNewDayStats();  // Add the first day with index 0 without incrementing currentDay
        }
        UpdateUI();  // Ensure UI reflects the initial state
        
    }

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
        int lastIndex = dailyStatsList.Count - 1;
        DailyStats currentDayStats = dailyStatsList[lastIndex];

        // Registering the sales of each item
        foreach (var item in purchasedItems)
        {
            if (currentDayStats.itemSales.ContainsKey(item.Key))
                currentDayStats.itemSales[item.Key] += item.Value;
            else
                currentDayStats.itemSales[item.Key] = item.Value;
        }

        currentDayStats.numberOfPurchasingCustomers++;
        currentDayStats.dailyRevenue += transactionValue;
        currentDayStats.dailyProfit += transactionProfit;

        // Check for highest transaction value
        if (transactionValue > currentDayStats.highestTransactionValue)
        {
            currentDayStats.highestTransactionValue = transactionValue;
            // Other updates can be done here if needed
        }

        // Check for most profitable transaction
        if (transactionProfit > currentDayStats.mostProfitableTransactionProfit)
        {
            currentDayStats.mostProfitableTransactionProfit = transactionProfit;
            currentDayStats.mostProfitableCustomer = customer.customerName;
            // Assuming you want to track the total amount as well for the most profitable transaction
            currentDayStats.mostProfitableTransactionAmount = transactionValue;
        }

        // Update the list with new stats
        dailyStatsList[lastIndex] = currentDayStats;

        // Update UI
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
            mostPopularItemText.text = DetermineMostPopularItem(currentStats);
            leastPopularItemText.text = DetermineLeastPopularItem(currentStats);
            mostProfitableAmountText.text = $"£{currentStats.mostProfitableTransactionAmount:F2}";
            mostProfitableProfitText.text = $"£{currentStats.mostProfitableTransactionProfit:F2}";
            stockShortagePerCustomerText.text = $"Per Customer: {currentStats.stockShortagePerCustomer}";
            stockShortagePerItemText.text = $"Per Item: {currentStats.stockShortagePerItem}";
            
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
