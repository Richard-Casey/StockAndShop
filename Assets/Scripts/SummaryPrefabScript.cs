using UnityEngine;
using TMPro;
using System; 
using System.Linq;

/// <summary>
/// Updates and manages the daily summary UI elements.
/// </summary>
public class SummaryPrefabScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI customerSatisfaction;
    [SerializeField] private TextMeshProUGUI dailyExpenses;
    [SerializeField] private TextMeshProUGUI dailyProfit;
    [SerializeField] private TextMeshProUGUI dailyRevenue;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;
    [SerializeField] private TextMeshProUGUI leastPopularItem;
    [SerializeField] private TextMeshProUGUI mostPopularItem;
    [SerializeField] private TextMeshProUGUI mostProfitableCustomer;
    [SerializeField] private TextMeshProUGUI mostProfitableTransactionAmount;
    [SerializeField] private TextMeshProUGUI mostProfitableTransactionProfit;
    [SerializeField] private TextMeshProUGUI numOfCustomersNum;
    [SerializeField] private TextMeshProUGUI stockShortagesCustomers;
    [SerializeField] private TextMeshProUGUI stockShortagesItems;


    /// <summary>
    /// Updates the summary UI with the provided data.
    /// </summary>
    /// <param name="day">The current day number.</param>
    /// <param name="numberOfCustomers">The number of customers.</param>
    /// <param name="mostPopular">The most popular item.</param>
    /// <param name="leastPopular">The least popular item.</param>
    /// <param name="highestTransaction">The highest transaction value.</param>
    /// <param name="profitableCustomer">The most profitable customer.</param>
    /// <param name="profitableAmount">The most profitable transaction amount.</param>
    /// <param name="profitableProfit">The profit from the most profitable transaction.</param>
    /// <param name="profit">The daily profit.</param>
    /// <param name="satisfaction">The customer satisfaction percentage.</param>
    /// <param name="shortagesCustomers">The number of customers affected by stock shortages.</param>
    /// <param name="shortagesItems">The number of items affected by stock shortages.</param>
    /// <param name="revenue">The daily revenue.</param>
    /// <param name="expenses">The daily expenses.</param>
    public void UpdateData(int day, int numberOfCustomers, string mostPopular, string leastPopular, float highestTransaction, string profitableCustomer, float profitableAmount, float profitableProfit, float profit, float satisfaction, int shortagesCustomers, int shortagesItems, float revenue, float expenses)
    {
        dayText.text = $"Day: {day}";
        numOfCustomersNum.text = numberOfCustomers.ToString();
        mostPopularItem.text = mostPopular;
        leastPopularItem.text = leastPopular;
        highestTransactionValueText.text = $"£{highestTransaction:F2}";
        mostProfitableCustomer.text = profitableCustomer;
        mostProfitableTransactionAmount.text = $"£{profitableAmount:F2}";
        mostProfitableTransactionProfit.text = $"£{profitableProfit:F2}";
        dailyProfit.text = $"£{profit:F2}";
        customerSatisfaction.text = $"{satisfaction:F0}%";
        stockShortagesCustomers.text = $"{shortagesCustomers} Customers";
        stockShortagesItems.text = $"{shortagesItems} Items";
        dailyRevenue.text = $"£{revenue:F2}";
        dailyExpenses.text = $"£{expenses:F2}";
    }

    /// <summary>
    /// Updates the summary UI using the data from a DailyStats object.
    /// </summary>
    /// <param name="stats">The daily statistics to display.</param>
    /// <param name="customersNotSatisfied">The number of customers not satisfied due to stock shortages.</param>
    /// <param name="itemsNotSatisfied">The number of items not satisfied due to stock shortages.</param>
    public void UpdateDataFromStats(DailyStats stats, int customersNotSatisfied, int itemsNotSatisfied)
    {
        UpdateData(
            stats.dayNumber,
            stats.numberOfCustomers,
            DetermineMostPopularItem(stats),
            DetermineLeastPopularItem(stats),
            stats.highestTransactionValue,
            stats.mostProfitableCustomer,
            DetermineTotalSpentByMostProfitableCustomer(stats),
            stats.mostProfitableTransactionProfit,
            stats.dailyProfit,
            stats.customerSatisfaction,
            customersNotSatisfied,
            itemsNotSatisfied,
            stats.dailyRevenue,
            stats.dailyExpenses
        );
    }

    /// <summary>
    /// Determines the total amount spent by the most profitable customer.
    /// </summary>
    /// <param name="stats">The daily statistics to analyze.</param>
    /// <returns>The total amount spent by the most profitable customer.</returns>
    private float DetermineTotalSpentByMostProfitableCustomer(DailyStats stats)
    {
        return 0; // Placeholder
    }

    /// <summary>
    /// Updates the most popular item text.
    /// </summary>
    /// <param name="itemName">The name of the most popular item.</param>
    public void UpdateMostPopularItem(string itemName)
    {
        mostPopularItem.text = itemName;
    }

    /// <summary>
    /// Updates the number of customers text.
    /// </summary>
    /// <param name="newNumberOfCustomers">The new number of customers.</param>
    public void UpdateNumberOfCustomers(int newNumberOfCustomers)
    {
        numOfCustomersNum.text = newNumberOfCustomers.ToString();
    }

    /// <summary>
    /// Determines the most popular item from the daily statistics.
    /// </summary>
    /// <param name="stats">The daily statistics to analyze.</param>
    /// <returns>The name of the most popular item.</returns>
    public string DetermineMostPopularItem(DailyStats stats)
    {
        if (stats.itemSales.Count == 0) return "N/A";
        return stats.itemSales.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }

    /// <summary>
    /// Determines the least popular item from the daily statistics.
    /// </summary>
    /// <param name="stats">The daily statistics to analyze.</param>
    /// <returns>The name of the least popular item.</returns>
    public string DetermineLeastPopularItem(DailyStats stats)
    {
        if (stats.itemSales.Count == 0) return "N/A";
        return stats.itemSales.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
    }

    /// <summary>
    /// Gets the number of customers from the UI.
    /// </summary>
    /// <returns>The number of customers as an integer.</returns>
    public int GetNumberOfCustomers() => int.Parse(numOfCustomersNum.text);

    /// <summary>
    /// Gets the daily revenue from the UI.
    /// </summary>
    /// <returns>The daily revenue as a float.</returns>
    public float GetDailyRevenue() => float.Parse(dailyRevenue.text.Replace("£", ""));

    /// <summary>
    /// Gets the daily expenses from the UI.
    /// </summary>
    /// <returns>The daily expenses as a float.</returns>
    public float GetDailyExpenses() => float.Parse(dailyExpenses.text.Replace("£", ""));

    /// <summary>
    /// Gets the daily profit from the UI.
    /// </summary>
    /// <returns>The daily profit as a float.</returns>
    public float GetDailyProfit() => float.Parse(dailyProfit.text.Replace("£", ""));

    /// <summary>
    /// Gets the most profitable customer from the UI.
    /// </summary>
    /// <returns>The name of the most profitable customer as a string.</returns>
    public string GetMostProfitableCustomer() => mostProfitableCustomer.text;

    /// <summary>
    /// Gets the highest transaction value from the UI.
    /// </summary>
    /// <returns>The highest transaction value as a float.</returns>
    public float GetHighestTransactionValue() => float.Parse(highestTransactionValueText.text.Replace("£", ""));

    /// <summary>
    /// Gets the most profitable transaction profit from the UI.
    /// </summary>
    /// <returns>The most profitable transaction profit as a float.</returns>
    public float GetMostProfitableTransactionProfit() => float.Parse(mostProfitableTransactionProfit.text.Replace("£", ""));

    /// <summary>
    /// Gets the most popular item from the UI.
    /// </summary>
    /// <returns>The name of the most popular item as a string.</returns>
    public string GetMostPopularItem() => mostPopularItem.text;

    /// <summary>
    /// Gets the least popular item from the UI.
    /// </summary>
    /// <returns>The name of the least popular item as a string.</returns>
    public string GetLeastPopularItem() => leastPopularItem.text;
}
