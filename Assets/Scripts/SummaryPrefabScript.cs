using UnityEngine;
using TMPro;
using System; // This is required for Int32, but in Unity, you can usually just use 'int'
using System.Linq;

/// <summary>
/// 
/// </summary>
public class SummaryPrefabScript : MonoBehaviour
{
    /// <summary>
    /// The customer satisfaction
    /// </summary>
    [SerializeField] private TextMeshProUGUI customerSatisfaction;
    /// <summary>
    /// The daily expenses
    /// </summary>
    [SerializeField] private TextMeshProUGUI dailyExpenses;
    /// <summary>
    /// The daily profit
    /// </summary>
    [SerializeField] private TextMeshProUGUI dailyProfit;
    /// <summary>
    /// The daily revenue
    /// </summary>
    [SerializeField] private TextMeshProUGUI dailyRevenue;
    /// <summary>
    /// The day text
    /// </summary>
    [SerializeField] private TextMeshProUGUI dayText;
    /// <summary>
    /// The highest transaction value text
    /// </summary>
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;
    /// <summary>
    /// The least popular item
    /// </summary>
    [SerializeField] private TextMeshProUGUI leastPopularItem;
    /// <summary>
    /// The most popular item
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostPopularItem;
    /// <summary>
    /// The most profitable customer
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableCustomer;
    /// <summary>
    /// The most profitable transaction amount
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableTransactionAmount;
    /// <summary>
    /// The most profitable transaction profit
    /// </summary>
    [SerializeField] private TextMeshProUGUI mostProfitableTransactionProfit;
    /// <summary>
    /// The number of customers number
    /// </summary>
    [SerializeField] private TextMeshProUGUI numOfCustomersNum;
    /// <summary>
    /// The stock shortages customers
    /// </summary>
    [SerializeField] private TextMeshProUGUI stockShortagesCustomers;
    /// <summary>
    /// The stock shortages items
    /// </summary>
    [SerializeField] private TextMeshProUGUI stockShortagesItems;

    /// <summary>
    /// Updates the data.
    /// </summary>
    /// <param name="day">The day.</param>
    /// <param name="numberOfCustomers">The number of customers.</param>
    /// <param name="mostPopular">The most popular.</param>
    /// <param name="leastPopular">The least popular.</param>
    /// <param name="highestTransaction">The highest transaction.</param>
    /// <param name="profitableCustomer">The profitable customer.</param>
    /// <param name="profitableAmount">The profitable amount.</param>
    /// <param name="profitableProfit">The profitable profit.</param>
    /// <param name="profit">The profit.</param>
    /// <param name="satisfaction">The satisfaction.</param>
    /// <param name="shortagesCustomers">The shortages customers.</param>
    /// <param name="shortagesItems">The shortages items.</param>
    /// <param name="revenue">The revenue.</param>
    /// <param name="expenses">The expenses.</param>
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
    /// Updates the data from stats.
    /// </summary>
    /// <param name="stats">The stats.</param>
    /// <param name="customersNotSatisfied">The customers not satisfied.</param>
    /// <param name="itemsNotSatisfied">The items not satisfied.</param>
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


    // This is a placeholder. You'll need to define how you calculate this based on your game's data structures.
    /// <summary>
    /// Determines the total spent by most profitable customer.
    /// </summary>
    /// <param name="stats">The stats.</param>
    /// <returns></returns>
    private float DetermineTotalSpentByMostProfitableCustomer(DailyStats stats)
    {
        // Implement your logic to determine the total amount spent by the most profitable customer
        return 0; // Placeholder
    }


    /// <summary>
    /// Updates the most popular item.
    /// </summary>
    /// <param name="itemName">Name of the item.</param>
    public void UpdateMostPopularItem(string itemName)
    {
        mostPopularItem.text = itemName;
    }

    /// <summary>
    /// Updates the number of customers.
    /// </summary>
    /// <param name="newNumberOfCustomers">The new number of customers.</param>
    public void UpdateNumberOfCustomers(int newNumberOfCustomers)
    {
        numOfCustomersNum.text = newNumberOfCustomers.ToString();
    }

    /// <summary>
    /// Determines the most popular item.
    /// </summary>
    /// <param name="stats">The stats.</param>
    /// <returns></returns>
    public string DetermineMostPopularItem(DailyStats stats)
    {
        if (stats.itemSales.Count == 0) return "N/A";
        return stats.itemSales.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
    }

    /// <summary>
    /// Determines the least popular item.
    /// </summary>
    /// <param name="stats">The stats.</param>
    /// <returns></returns>
    public string DetermineLeastPopularItem(DailyStats stats)
    {
        if (stats.itemSales.Count == 0) return "N/A";
        return stats.itemSales.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;
    }



    // Keep these if necessary for your architecture, otherwise consider restructuring to remove UI element data fetching
    /// <summary>
    /// Gets the number of customers.
    /// </summary>
    /// <returns></returns>
    public int GetNumberOfCustomers() => int.Parse(numOfCustomersNum.text);
    /// <summary>
    /// Gets the daily revenue.
    /// </summary>
    /// <returns></returns>
    public float GetDailyRevenue() => float.Parse(dailyRevenue.text.Replace("£", ""));
    /// <summary>
    /// Gets the daily expenses.
    /// </summary>
    /// <returns></returns>
    public float GetDailyExpenses() => float.Parse(dailyExpenses.text.Replace("£", ""));
    /// <summary>
    /// Gets the daily profit.
    /// </summary>
    /// <returns></returns>
    public float GetDailyProfit() => float.Parse(dailyProfit.text.Replace("£", ""));
    /// <summary>
    /// Gets the most profitable customer.
    /// </summary>
    /// <returns></returns>
    public string GetMostProfitableCustomer() => mostProfitableCustomer.text;
    /// <summary>
    /// Gets the highest transaction value.
    /// </summary>
    /// <returns></returns>
    public float GetHighestTransactionValue() => float.Parse(highestTransactionValueText.text.Replace("£", ""));
    /// <summary>
    /// Gets the most profitable transaction profit.
    /// </summary>
    /// <returns></returns>
    public float GetMostProfitableTransactionProfit() => float.Parse(mostProfitableTransactionProfit.text.Replace("£", ""));
    /// <summary>
    /// Gets the most popular item.
    /// </summary>
    /// <returns></returns>
    public string GetMostPopularItem() => mostPopularItem.text;
    /// <summary>
    /// Gets the least popular item.
    /// </summary>
    /// <returns></returns>
    public string GetLeastPopularItem() => leastPopularItem.text;
}
