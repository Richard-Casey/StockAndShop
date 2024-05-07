using UnityEngine;
using TMPro;
using System; // This is required for Int32, but in Unity, you can usually just use 'int'

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
    private float DetermineTotalSpentByMostProfitableCustomer(DailyStats stats)
    {
        // Implement your logic to determine the total amount spent by the most profitable customer
        return 0; // Placeholder
    }


    public void UpdateMostPopularItem(string itemName)
    {
        mostPopularItem.text = itemName;
    }

    public void UpdateNumberOfCustomers(int newNumberOfCustomers)
    {
        numOfCustomersNum.text = newNumberOfCustomers.ToString();
    }

    private string DetermineMostPopularItem(DailyStats stats)
    {
        int maxCount = 0;
        string mostPopular = "N/A";
        foreach (var count in stats.itemCounts)
        {
            if (count > maxCount)
            {
                maxCount = count;
            }
        }
        return maxCount > 0 ? stats.itemNames[stats.itemCounts.IndexOf(maxCount)] : mostPopular;
    }

    private string DetermineLeastPopularItem(DailyStats stats)
    {
        int minCount = int.MaxValue;
        string leastPopular = "N/A";
        foreach (var count in stats.itemCounts)
        {
            if (count < minCount)
            {
                minCount = count;
            }
        }
        return minCount < int.MaxValue ? stats.itemNames[stats.itemCounts.IndexOf(minCount)] : leastPopular;
    }

    // Keep these if necessary for your architecture, otherwise consider restructuring to remove UI element data fetching
    public int GetNumberOfCustomers() => int.Parse(numOfCustomersNum.text);
    public float GetDailyRevenue() => float.Parse(dailyRevenue.text.Replace("£", ""));
    public float GetDailyExpenses() => float.Parse(dailyExpenses.text.Replace("£", ""));
    public float GetDailyProfit() => float.Parse(dailyProfit.text.Replace("£", ""));
    public string GetMostProfitableCustomer() => mostProfitableCustomer.text;
    public float GetHighestTransactionValue() => float.Parse(highestTransactionValueText.text.Replace("£", ""));
    public float GetMostProfitableTransactionProfit() => float.Parse(mostProfitableTransactionProfit.text.Replace("£", ""));
    public string GetMostPopularItem() => mostPopularItem.text;
    public string GetLeastPopularItem() => leastPopularItem.text;
}
