using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the overall summary statistics of the shop, aggregating data from daily summaries.
/// </summary>
public class OverallSummaryManager : MonoBehaviour
{
    /// <summary>
    /// Represents the overall statistics of the shop.
    /// </summary>
    [System.Serializable]
    public struct OverallStats
    {
        /// <summary>
        /// The total number of customers.
        /// </summary>
        public int totalCustomers;

        /// <summary>
        /// The total number of purchasing customers.
        /// </summary>
        public int totalPurchasingCustomers;

        /// <summary>
        /// The most popular item sold.
        /// </summary>
        public string mostPopularItem;

        /// <summary>
        /// The least popular item sold.
        /// </summary>
        public string leastPopularItem;

        /// <summary>
        /// The highest transaction value recorded.
        /// </summary>
        public float highestTransactionValue;

        /// <summary>
        /// The most profitable customer.
        /// </summary>
        public string mostProfitableCustomer;

        /// <summary>
        /// The amount of the most profitable transaction.
        /// </summary>
        public float mostProfitableAmount;

        /// <summary>
        /// The profit from the most profitable transaction.
        /// </summary>
        public float mostProfitableProfit;

        /// <summary>
        /// The total revenue generated.
        /// </summary>
        public float totalRevenue;

        /// <summary>
        /// The total expenses incurred.
        /// </summary>
        public float totalExpenses;

        /// <summary>
        /// The total profit earned.
        /// </summary>
        public float totalProfit;

        /// <summary>
        /// The average customer satisfaction.
        /// </summary>
        public float averageCustomerSatisfaction;

        /// <summary>
        /// The total stock shortage per customer.
        /// </summary>
        public int totalStockShortagePerCustomer;

        /// <summary>
        /// The total stock shortage per item.
        /// </summary>
        public int totalStockShortagePerItem;
    }

    /// <summary>
    /// The overall statistics of the shop.
    /// </summary>
    public OverallStats overallStats;

    /// <summary>
    /// Reference to the DailySummaryManager for accessing daily statistics.
    /// </summary>
    [SerializeField] private DailySummaryManager dailySummaryManager;

    /// <summary>
    /// UI text component for displaying the total number of customers.
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalCustomersText;

    /// <summary>
    /// UI text component for displaying the total number of purchasing customers.
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalPurchasingCustomersText;

    /// <summary>
    /// UI text component for displaying the most popular item.
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostPopularItemText;

    /// <summary>
    /// UI text component for displaying the least popular item.
    /// </summary>
    [SerializeField] private TextMeshProUGUI leastPopularItemText;

    /// <summary>
    /// UI text component for displaying the highest transaction value.
    /// </summary>
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;

    /// <summary>
    /// UI text component for displaying the most profitable customer.
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableCustomerText;

    /// <summary>
    /// UI text component for displaying the amount of the most profitable transaction.
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableAmountText;

    /// <summary>
    /// UI text component for displaying the profit from the most profitable transaction.
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableProfitText;

    /// <summary>
    /// UI text component for displaying the total revenue.
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalRevenueText;

    /// <summary>
    /// UI text component for displaying the total expenses.
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalExpensesText;

    /// <summary>
    /// UI text component for displaying the total profit.
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalProfitText;

    /// <summary>
    /// UI text component for displaying the average customer satisfaction.
    /// </summary>
    [SerializeField] private TextMeshProUGUI averageCustomerSatisfactionText;

    /// <summary>
    /// UI text component for displaying the total stock shortage per customer.
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalStockShortagePerCustomerText;

    /// <summary>
    /// UI text component for displaying the total stock shortage per item.
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalStockShortagePerItemText;

    /// <summary>
    /// Initializes the OverallSummaryManager by finding the DailySummaryManager and updating the overall statistics.
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
    /// Updates the overall statistics by aggregating data from daily summaries.
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
    /// Updates the overall summary UI with the current overall statistics.
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
