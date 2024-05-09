using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class OverallSummaryManager : MonoBehaviour
{
    [System.Serializable]
    public struct OverallStats
    {
        public int totalCustomers;
        public int totalPurchasingCustomers;
        public string mostPopularItem;
        public string leastPopularItem;
        public float highestTransactionValue;
        public string mostProfitableCustomer;
        public float mostProfitableAmount;
        public float mostProfitableProfit;
        public float totalRevenue;
        public float totalExpenses;
        public float totalProfit;
        public float averageCustomerSatisfaction;
        public int totalStockShortagePerCustomer;
        public int totalStockShortagePerItem;
    }

    public OverallStats overallStats;

    // Reference to DailySummaryManager
    [SerializeField] private DailySummaryManager dailySummaryManager;

    // UI References for overall summary
    [SerializeField] private TextMeshProUGUI totalCustomersText;
    [SerializeField] private TextMeshProUGUI totalPurchasingCustomersText;
    [SerializeField] private TextMeshProUGUI mostPopularItemText;
    [SerializeField] private TextMeshProUGUI leastPopularItemText;
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;
    [SerializeField] private TextMeshProUGUI mostProfitableCustomerText;
    [SerializeField] private TextMeshProUGUI mostProfitableAmountText;
    [SerializeField] private TextMeshProUGUI mostProfitableProfitText;
    [SerializeField] private TextMeshProUGUI totalRevenueText;
    [SerializeField] private TextMeshProUGUI totalExpensesText;
    [SerializeField] private TextMeshProUGUI totalProfitText;
    [SerializeField] private TextMeshProUGUI averageCustomerSatisfactionText;
    [SerializeField] private TextMeshProUGUI totalStockShortagePerCustomerText;
    [SerializeField] private TextMeshProUGUI totalStockShortagePerItemText;

    void Start()
    {
        if (dailySummaryManager == null)
        {
            dailySummaryManager = FindObjectOfType<DailySummaryManager>();
        }
        UpdateOverallStats();
    }

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
