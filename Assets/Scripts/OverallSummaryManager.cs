using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 
/// </summary>
public class OverallSummaryManager : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public struct OverallStats
    {
        /// <summary>
        /// The total customers
        /// </summary>
        public int totalCustomers;
        /// <summary>
        /// The total purchasing customers
        /// </summary>
        public int totalPurchasingCustomers;
        /// <summary>
        /// The most popular item
        /// </summary>
        public string mostPopularItem;
        /// <summary>
        /// The least popular item
        /// </summary>
        public string leastPopularItem;
        /// <summary>
        /// The highest transaction value
        /// </summary>
        public float highestTransactionValue;
        /// <summary>
        /// The most profitable customer
        /// </summary>
        public string mostProfitableCustomer;
        /// <summary>
        /// The most profitable amount
        /// </summary>
        public float mostProfitableAmount;
        /// <summary>
        /// The most profitable profit
        /// </summary>
        public float mostProfitableProfit;
        /// <summary>
        /// The total revenue
        /// </summary>
        public float totalRevenue;
        /// <summary>
        /// The total expenses
        /// </summary>
        public float totalExpenses;
        /// <summary>
        /// The total profit
        /// </summary>
        public float totalProfit;
        /// <summary>
        /// The average customer satisfaction
        /// </summary>
        public float averageCustomerSatisfaction;
        /// <summary>
        /// The total stock shortage per customer
        /// </summary>
        public int totalStockShortagePerCustomer;
        /// <summary>
        /// The total stock shortage per item
        /// </summary>
        public int totalStockShortagePerItem;
    }

    /// <summary>
    /// The overall stats
    /// </summary>
    public OverallStats overallStats;

    // Reference to DailySummaryManager
    /// <summary>
    /// The daily summary manager
    /// </summary>
    [SerializeField] private DailySummaryManager dailySummaryManager;

    // UI References for overall summary
    /// <summary>
    /// The total customers text
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalCustomersText;
    /// <summary>
    /// The total purchasing customers text
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalPurchasingCustomersText;
    /// <summary>
    /// The most popular item text
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostPopularItemText;
    /// <summary>
    /// The least popular item text
    /// </summary>
    [SerializeField] private TextMeshProUGUI leastPopularItemText;
    /// <summary>
    /// The highest transaction value text
    /// </summary>
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;
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
    /// The total revenue text
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalRevenueText;
    /// <summary>
    /// The total expenses text
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalExpensesText;
    /// <summary>
    /// The total profit text
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalProfitText;
    /// <summary>
    /// The average customer satisfaction text
    /// </summary>
    [SerializeField] private TextMeshProUGUI averageCustomerSatisfactionText;
    /// <summary>
    /// The total stock shortage per customer text
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalStockShortagePerCustomerText;
    /// <summary>
    /// The total stock shortage per item text
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalStockShortagePerItemText;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start()
    {
        if (dailySummaryManager == null)
        {
            dailySummaryManager = FindObjectOfType<DailySummaryManager>();
        }
        UpdateOverallStats();
    }

    /// <summary>
    /// Updates the overall stats.
    /// </summary>
    public void UpdateOverallStats()
    {
        List<DailyStats> dailyStatsList = dailySummaryManager.GetDailyStats();

        overallStats.totalCustomers = dailyStatsList.Sum(day => day.numberOfCustomers);
        overallStats.totalPurchasingCustomers = dailyStatsList.Sum(day => day.numberOfPurchasingCustomers);
        overallStats.totalRevenue = dailyStatsList.Sum(day => day.dailyRevenue);
        overallStats.totalExpenses = dailyStatsList.Sum(day => day.dailyExpenses);
        overallStats.totalProfit = dailyStatsList.Sum(day => day.dailyProfit);
        overallStats.averageCustomerSatisfaction = dailyStatsList.Any() ? dailyStatsList.Average(day => day.customerSatisfaction) : 0;
        overallStats.totalStockShortagePerCustomer = dailyStatsList.Sum(day => day.stockShortagePerCustomer);
        overallStats.totalStockShortagePerItem = dailyStatsList.Sum(day => day.stockShortagePerItem);

        Dictionary<string, int> aggregatedItemSales = new Dictionary<string, int>();
        foreach (var day in dailyStatsList)
        {
            foreach (var item in day.itemSales)
            {
                if (aggregatedItemSales.ContainsKey(item.Key))
                    aggregatedItemSales[item.Key] += item.Value;
                else
                    aggregatedItemSales[item.Key] = item.Value;
            }
        }

        if (aggregatedItemSales.Count > 0)
        {
            overallStats.mostPopularItem = aggregatedItemSales.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            overallStats.leastPopularItem = aggregatedItemSales.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
        }

        overallStats.highestTransactionValue = dailyStatsList.Any() ? dailyStatsList.Max(day => day.highestTransactionValue) : 0;

        // Find the most profitable transaction across all days
        if (dailyStatsList.Any())
        {
            var mostProfitableTransaction = dailyStatsList.OrderByDescending(day => day.mostProfitableTransactionProfit).FirstOrDefault();
            overallStats.mostProfitableCustomer = mostProfitableTransaction.mostProfitableCustomer;
            overallStats.mostProfitableAmount = mostProfitableTransaction.mostProfitableTransactionAmount;
            overallStats.mostProfitableProfit = mostProfitableTransaction.mostProfitableTransactionProfit;
        }

        UpdateUI();
    }



    /// <summary>
    /// Updates the UI.
    /// </summary>
    private void UpdateUI()
    {
        // Update all UI elements with the values from overallStats
        totalCustomersText.text = overallStats.totalCustomers.ToString();
        totalPurchasingCustomersText.text = overallStats.totalPurchasingCustomers.ToString();
        mostPopularItemText.text = overallStats.mostPopularItem;
        leastPopularItemText.text = overallStats.leastPopularItem;
        highestTransactionValueText.text = $"£{overallStats.highestTransactionValue:F2}";
        mostProfitableCustomerText.text = overallStats.mostProfitableCustomer;
        mostProfitableAmountText.text = $"£{overallStats.mostProfitableAmount:F2}";
        mostProfitableProfitText.text = $"£{overallStats.mostProfitableProfit:F2}";
        totalRevenueText.text = $"£{overallStats.totalRevenue:F2}";
        totalExpensesText.text = $"£{overallStats.totalExpenses:F2}";
        totalProfitText.text = $"£{overallStats.totalProfit:F2}";
        averageCustomerSatisfactionText.text = $"{overallStats.averageCustomerSatisfaction:F0}%";
        totalStockShortagePerCustomerText.text = $"Per Customer: {overallStats.totalStockShortagePerCustomer.ToString()}";
        totalStockShortagePerItemText.text = $"Per Item: {overallStats.totalStockShortagePerItem.ToString()}";
    }
}
