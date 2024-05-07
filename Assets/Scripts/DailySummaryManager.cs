using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public struct DailyStats
{
    public int dayNumber;
    public int numberOfCustomers;
    public float dailyRevenue;
    public float dailyExpenses;
    public float dailyProfit;
    public List<string> itemNames; // For tracking item sales
    public List<int> itemCounts;
    public string mostProfitableCustomer;
    public float highestTransactionValue;
    public float mostProfitableTransactionProfit;
    public float customerSatisfaction; // Stores customer satisfaction percentage
}

public class DailySummaryManager : MonoBehaviour
{
    public int currentDay = 1;
    private List<DailyStats> dailyStatsList = new List<DailyStats>();
    private CustomerSpawner customerSpawner;

    // UI References
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI numberOfCustomersText;
    [SerializeField] private TextMeshProUGUI dailyRevenueText;
    [SerializeField] private TextMeshProUGUI dailyExpensesText;
    [SerializeField] private TextMeshProUGUI dailyProfitText;
    [SerializeField] private TextMeshProUGUI mostProfitableCustomerText;
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;
    [SerializeField] private TextMeshProUGUI customerSatisfactionText;

    void Start()
    {
        customerSpawner = FindObjectOfType<CustomerSpawner>();
        InitializeDay();
    }

    void InitializeDay()
    {
        if (dailyStatsList.Count < currentDay)
        {
            AddNewDayStats();
        }
    }

    void AddNewDayStats()
    {
        DailyStats newDayStats = new DailyStats
        {
            dayNumber = currentDay,
            itemNames = new List<string>(),
            itemCounts = new List<int>(),
            dailyExpenses = 0,
            dailyProfit = 0,
            dailyRevenue = 0,
            mostProfitableCustomer = "",
            highestTransactionValue = 0,
            mostProfitableTransactionProfit = 0,
            customerSatisfaction = 100 // Assume starting reputation or carry it over
        };
        dailyStatsList.Add(newDayStats);
        UpdateUI();
    }

    public void PrepareDay()
    {
        if (dailyStatsList.Count < currentDay)
        {
            AddNewDayStats(); // Ensure a new day is started if not already prepared
        }
        else
        {
            ResetDailyStats(); // Reset stats for the new day
        }
        UpdateUI();
    }

    public void CheckAndEndDay()
    {
        if (customerSpawner.ActiveCustomers == 0)
        {
            // End the day and finalize stats
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

    void UpdateUI()
    {
        if (dailyStatsList.Count > 0 && currentDay <= dailyStatsList.Count)
        {
            DailyStats currentStats = dailyStatsList[currentDay - 1];
            dayText.text = "Day: " + currentStats.dayNumber.ToString();
            numberOfCustomersText.text = currentStats.numberOfCustomers.ToString();
            dailyRevenueText.text = $"£{currentStats.dailyRevenue:F2}";
            dailyExpensesText.text = $"£{currentStats.dailyExpenses:F2}";
            dailyProfitText.text = $"£{currentStats.dailyProfit:F2}";
            mostProfitableCustomerText.text = currentStats.mostProfitableCustomer;
            highestTransactionValueText.text = $"£{currentStats.highestTransactionValue:F2}";
            customerSatisfactionText.text = $"{currentStats.customerSatisfaction:F0}%";
        }
    }

    public void EndOfDaySummary()
    {
        // Log or update end of day summary
        Debug.Log("Day ended: " + currentDay);
        IncrementDay(); // Prepare for the next day
    }

    void IncrementDay()
    {
        currentDay++;
        ResetDailyStats();
    }

    void ResetDailyStats()
    {
        float lastSatisfaction = dailyStatsList.Count > 0 ? dailyStatsList.Last().customerSatisfaction : 100;
        DailyStats newDayStats = new DailyStats
        {
            dayNumber = currentDay,
            numberOfCustomers = 0,
            dailyRevenue = 0,
            dailyExpenses = 0,
            dailyProfit = 0,
            itemNames = new List<string>(),
            itemCounts = new List<int>(),
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

    public void RegisterTransaction(Customer customer, Dictionary<string, int> purchasedItems, float transactionValue, float transactionProfit)
    {
        if (dailyStatsList.Count == 0)
            InitializeDay();  // Ensure there's at least one day to work with

        DailyStats todayStats = dailyStatsList.Last();
        foreach (var item in purchasedItems)
        {
            int index = todayStats.itemNames.IndexOf(item.Key);
            if (index == -1)  // Item not found in the list
            {
                todayStats.itemNames.Add(item.Key);
                todayStats.itemCounts.Add(item.Value);
            }
            else
            {
                todayStats.itemCounts[index] += item.Value;
            }
        }

        todayStats.dailyRevenue += transactionValue;
        todayStats.dailyProfit += transactionProfit;
        if (transactionProfit > todayStats.mostProfitableTransactionProfit)
        {
            todayStats.mostProfitableTransactionProfit = transactionProfit;
            todayStats.mostProfitableCustomer = customer.customerName;
        }

        dailyStatsList[dailyStatsList.Count - 1] = todayStats; // Update the last element
        UpdateUI();
    }


}





//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using System.Linq;

//[System.Serializable]
//public struct DailyStats
//{
//    public int dayNumber;
//    public int numberOfCustomers;
//    public float dailyRevenue;
//    public float dailyExpenses;
//    public float dailyProfit;
//    public List<string> itemNames; // For tracking item sales
//    public List<int> itemCounts;
//    public string mostProfitableCustomer;
//    public float highestTransactionValue;
//    public float mostProfitableTransactionProfit;
//    public float customerSatisfaction; // Stores customer satisfaction percentage
//}

//public class DailySummaryManager : MonoBehaviour
//{
//    public int currentDay = 1;
//    public float dayEndTime = 17.00f;
//    private List<DailyStats> dailyStatsList = new List<DailyStats>();
//    private CustomerSpawner customerSpawner;

//    [SerializeField] private Transform summaryParent;
//    [SerializeField] private GameObject summaryPrefab;
//    private List<GameObject> dailySummaries = new List<GameObject>();
//    private int currentSummaryIndex = 0;

//    [SerializeField] private Button prevDayButton;
//    [SerializeField] private Button nextDayButton;

//    void Start()
//    {
//        customerSpawner = FindObjectOfType<CustomerSpawner>();
//        InstantiateSummaryPrefab();
//        PrepareDay(); // Ensure the day is prepared at game start
//    }

//    void InitializeDay()
//    {
//        if (dailyStatsList.Count < currentDay) // If there's no entry for the current day, add one
//        {
//            DailyStats newDayStats = new DailyStats
//            {
//                dayNumber = currentDay,
//                itemNames = new List<string>(),
//                itemCounts = new List<int>(),
//                dailyExpenses = 0,
//                dailyProfit = 0,
//                dailyRevenue = 0,
//                mostProfitableCustomer = "",
//                highestTransactionValue = 0,
//                mostProfitableTransactionProfit = 0
//            };
//            dailyStatsList.Add(newDayStats);
//            UpdateSummary(); // Update the summary to reflect the new day initialization
//        }
//    }

//    public void RegisterDailyExpenses(float amount)
//    {
//        if (currentDay > dailyStatsList.Count)
//        {
//            PrepareDay(); // Ensure day is prepared before registering expenses
//        }
//        DailyStats todayStats = dailyStatsList[currentDay - 1];
//        todayStats.dailyExpenses += amount;
//        dailyStatsList[currentDay - 1] = todayStats; // Update the list with modified day stats
//        UpdateSummary();
//    }


//    void InstantiateSummaryPrefab()
//    {
//        GameObject newSummary = Instantiate(summaryPrefab, summaryParent);
//        RectTransform rectTransform = newSummary.GetComponent<RectTransform>();

//        // Set size, scale, anchors, pivot, and position
//        rectTransform.sizeDelta = new Vector2(1298, 674);
//        rectTransform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
//        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
//        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
//        rectTransform.pivot = new Vector2(0.5f, 0.5f);
//        rectTransform.anchoredPosition = Vector2.zero;

//        dailySummaries.Add(newSummary);
//        if (dailySummaries.Count > 1)
//            dailySummaries[currentSummaryIndex].SetActive(false);
//        currentSummaryIndex = dailySummaries.Count - 1;
//        newSummary.SetActive(true);
//    }

//    public void RegisterCustomerEntry()
//    {
//        DailyStats todayStats = dailyStatsList[currentSummaryIndex];
//        todayStats.numberOfCustomers++;
//        dailyStatsList[currentSummaryIndex] = todayStats;
//        UpdateSummary(); // Update the summary immediately
//    }

//    public void RegisterTransaction(Customer customer, Dictionary<string, int> purchasedItems, float transactionValue, float transactionProfit)
//    {
//        DailyStats todayStats = dailyStatsList.LastOrDefault();
//        foreach (var item in purchasedItems)
//        {
//            int index = todayStats.itemNames.IndexOf(item.Key);
//            if (index == -1)
//            {
//                todayStats.itemNames.Add(item.Key);
//                todayStats.itemCounts.Add(item.Value);
//            }
//            else
//            {
//                todayStats.itemCounts[index] += item.Value;
//            }
//        }
//        todayStats.dailyRevenue += transactionValue;
//        todayStats.dailyProfit += transactionProfit;
//        if (transactionProfit > todayStats.mostProfitableTransactionProfit)
//        {
//            todayStats.mostProfitableTransactionProfit = transactionProfit;
//            todayStats.mostProfitableCustomer = customer.customerName;
//        }
//        if (transactionValue > todayStats.highestTransactionValue)
//            todayStats.highestTransactionValue = transactionValue;

//        dailyStatsList[dailyStatsList.Count - 1] = todayStats;
//        UpdateSummary(); // Update the summary right after updating the stats
//    }


//    public void PrepareDay()
//    {
//        if (dailyStatsList.Count < currentDay)
//        {
//            AddNewDayStats(); // Add a new day stats if not present
//        }
//        UpdateSummary(); // Update the summary UI
//    }

//    void AddNewDayStats()
//    {
//        DailyStats newDayStats = new DailyStats
//        {
//            dayNumber = currentDay,
//            itemNames = new List<string>(),
//            itemCounts = new List<int>(),
//            dailyExpenses = 0,
//            dailyProfit = 0,
//            dailyRevenue = 0,
//            mostProfitableCustomer = "",
//            highestTransactionValue = 0,
//            mostProfitableTransactionProfit = 0,
//            customerSatisfaction = customerSpawner.Reputation // Start with current reputation
//        };
//        dailyStatsList.Add(newDayStats);
//    }


//    public void EndOfDaySummary()
//    {
//        if (dailyStatsList.Count == 0 || dailyStatsList.Last().dayNumber != currentDay)
//        {
//            DailyStats newDayStats = new DailyStats
//            {
//                dayNumber = currentDay,
//                itemNames = new List<string>(),
//                itemCounts = new List<int>(),
//                customerSatisfaction = customerSpawner.Reputation
//            };
//            dailyStatsList.Add(newDayStats);
//        }

//        DebugLogDayStats(dailyStatsList.Last());

//        if (dailySummaries.Count > currentSummaryIndex)
//        {
//            var summaryScript = dailySummaries[currentSummaryIndex].GetComponent<SummaryPrefabScript>();
//            if (summaryScript != null)
//            {
//                summaryScript.UpdateDataFromStats(dailyStatsList.Last(), 0, 0); // Add actual logic to calculate unsatisfied customers
//            }
//        }

//        IncrementDay();
//    }


//    void DebugLogDayStats(DailyStats dayStats)
//    {
//        Debug.Log($"Day: {dayStats.dayNumber}\n" +
//                  $"Number of Customers: {dayStats.numberOfCustomers}\n" +
//                  $"Daily Revenue: £{dayStats.dailyRevenue:F2}\n" +
//                  $"Daily Expenses: £{dayStats.dailyExpenses:F2}\n" +
//                  $"Daily Profit: £{dayStats.dailyProfit:F2}\n" +
//                  $"Most Profitable Customer: {dayStats.mostProfitableCustomer}\n" +
//                  $"Highest Transaction Value: £{dayStats.highestTransactionValue:F2}\n" +
//                  $"Most Profitable Transaction Profit: £{dayStats.mostProfitableTransactionProfit:F2}\n" +
//                  $"Customer Satisfaction: {dayStats.customerSatisfaction}%\n" +
//                  $"Items Sold: {string.Join(", ", dayStats.itemNames.Zip(dayStats.itemCounts, (name, count) => $"{name} ({count})"))}");
//    }


//    public void OnOpenShopButtonPressed()
//    {
//        IncrementDay();  // Start a new day when the shop is opened
//    }


//    void UpdateButtonVisibility()
//    {
//        prevDayButton.gameObject.SetActive(currentSummaryIndex > 0);
//        nextDayButton.gameObject.SetActive(currentSummaryIndex < dailySummaries.Count - 1);
//    }

//    public void ShowNextDay()
//    {
//        if (currentSummaryIndex < dailySummaries.Count - 1)
//        {
//            dailySummaries[currentSummaryIndex].SetActive(false);
//            currentSummaryIndex++;
//            dailySummaries[currentSummaryIndex].SetActive(true);
//        }
//    }

//    public void ShowPreviousDay()
//    {
//        if (currentSummaryIndex > 0)
//        {
//            dailySummaries[currentSummaryIndex].SetActive(false);
//            currentSummaryIndex--;
//            dailySummaries[currentSummaryIndex].SetActive(true);
//        }
//    }

//    public void StartNewDay()
//    {
//        ResetDailyStats();  // Correct reset of stats for the new day
//        UpdateSummary();    // Update the summary to reflect the new day's starting point
//    }


//    void IncrementDay()
//    {
//        currentDay++;  // Increment the day count
//        ResetDailyStats();  // Reset stats for the new day
//    }

//    void ResetDailyStats()
//    {
//        float lastDaySatisfaction = dailyStatsList.Count > 0 ? dailyStatsList.Last().customerSatisfaction : 100;  // Default if it's the first day

//        // Create new day stats with a clean slate except for carried over values
//        DailyStats newDayStats = new DailyStats
//        {
//            dayNumber = currentDay,
//            numberOfCustomers = 0,
//            dailyRevenue = 0,
//            dailyExpenses = 0,
//            dailyProfit = 0,
//            itemNames = new List<string>(),
//            itemCounts = new List<int>(),
//            mostProfitableCustomer = "",
//            highestTransactionValue = 0,
//            mostProfitableTransactionProfit = 0,
//            customerSatisfaction = lastDaySatisfaction // Carry over the last recorded satisfaction
//        };

//        dailyStatsList.Add(newDayStats);  // Add the new day's stats to the list
//        currentSummaryIndex = dailyStatsList.Count - 1;  // Update index to the new day
//        UpdateSummary();  // Update the UI to reflect the reset
//    }



//    void UpdateSummaryForNewDay()
//    {
//        if (dailySummaries.Count == 0)
//        {
//            InstantiateSummaryPrefab();
//        }
//        else
//        {
//            var summaryScript = dailySummaries[currentSummaryIndex].GetComponent<SummaryPrefabScript>();
//            summaryScript.UpdateData(
//                currentDay, 0, "N/A", "N/A", 0, "N/A", 0, 0, 0, customerSpawner.Reputation, 0, 0, 0, 0);
//        }
//    }

//    public void RegisterCustomerDissatisfaction(int itemsNotFound, int customerCount)
//    {
//        if (dailyStatsList.Count > 0)
//        {
//            DailyStats todayStats = dailyStatsList.Last();
//            todayStats.numberOfCustomers += customerCount; // Example of how you might update dissatisfaction counts
//            dailyStatsList[dailyStatsList.Count - 1] = todayStats; // Make sure to assign back to list
//        }
//    }

//    public void UpdateDailyCustomerSatisfaction(float satisfactionChange)
//    {
//        if (dailyStatsList.Count > 0)
//        {
//            DailyStats todayStats = dailyStatsList.Last();
//            todayStats.customerSatisfaction += satisfactionChange; // Adjust satisfaction
//            todayStats.customerSatisfaction = Mathf.Clamp(todayStats.customerSatisfaction, 0, 100); // Keep within bounds
//            dailyStatsList[dailyStatsList.Count - 1] = todayStats; // Assign updated stats back to list
//        }
//    }

//    void UpdateSummary()
//    {
//        if (currentSummaryIndex >= 0 && currentSummaryIndex < dailySummaries.Count)
//        {
//            var summaryScript = dailySummaries[currentSummaryIndex].GetComponent<SummaryPrefabScript>();
//            DailyStats currentStats = dailyStatsList[currentSummaryIndex];

//            if (summaryScript != null)
//            {
//                // Update the summary prefab with the current day's stats
//                summaryScript.UpdateData(
//                    currentStats.dayNumber,
//                    currentStats.numberOfCustomers,
//                    "N/A", // Placeholder for most popular item
//                    "N/A", // Placeholder for least popular item
//                    currentStats.highestTransactionValue,
//                    currentStats.mostProfitableCustomer,
//                    DetermineTotalSpentByMostProfitableCustomer(currentStats),
//                    currentStats.mostProfitableTransactionProfit,
//                    currentStats.dailyProfit,
//                    currentStats.customerSatisfaction,
//                    0,  // Placeholder for shortages customers
//                    0,  // Placeholder for shortages items
//                    currentStats.dailyRevenue,
//                    currentStats.dailyExpenses
//                );
//            }
//        }
//        else
//        {
//            Debug.LogError("Current summary index is out of range.");
//        }
//    }

//    float DetermineTotalSpentByMostProfitableCustomer(DailyStats stats)
//    {
//        // Placeholder: Calculate the total spent by the most profitable customer
//        return 0; // Implement this method based on your data structures
//    }

//    // Implement or modify these placeholder methods as needed
//    private string DetermineMostPopularItem(DailyStats stats)
//    {
//        return "Item Name"; // Placeholder - implement your logic here
//    }

//    private string DetermineLeastPopularItem(DailyStats stats)
//    {
//        return "Item Name"; // Placeholder - implement your logic here
//    }


//    public void CheckAndEndDay()
//    {
//        if (DayHasEnded())
//        {
//            EndOfDaySummary();
//        }
//    }

//    private bool DayHasEnded()
//    {
//        // Check if the current time in DayCycle is greater than or equal to the day's duration
//        return DayCycle.Instance.currentTime >= DayCycle.Instance.dayDurationInSeconds;
//    }


//    private bool CheckIfTimeIsEndOfDay()
//    {
//        // Ensure that DayCycle.Instance is correctly checking for the end of day based on dayEndTime
//        return DayCycle.Instance.currentTime >= dayEndTime * 3600; // Convert hours to seconds if necessary
//    }

//}
