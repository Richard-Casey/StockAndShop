using UnityEngine;
using TMPro;

public class SummaryPrefabScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI numOfCustomersNum;
    [SerializeField] private TextMeshProUGUI mostPopularItem;
    [SerializeField] private TextMeshProUGUI leastPopularItem;
    [SerializeField] private TextMeshProUGUI highestTransactionValueText;
    [SerializeField] private TextMeshProUGUI mostProfitableCustomer;
    [SerializeField] private TextMeshProUGUI mostProfitableTransactionAmount;
    [SerializeField] private TextMeshProUGUI mostProfitableTransactionProfit;
    [SerializeField] private TextMeshProUGUI dailyProfit;
    [SerializeField] private TextMeshProUGUI customerSatisfaction;
    [SerializeField] private TextMeshProUGUI stockShortagesCustomers;
    [SerializeField] private TextMeshProUGUI stockShortagesItems;
    [SerializeField] private TextMeshProUGUI dailyRevenue;
    [SerializeField] private TextMeshProUGUI dailyExpenses;

    // This method will be called to update the prefab with the day's summary data
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

    public void UpdateNumberOfCustomers(int newNumberOfCustomers)
    {
        numOfCustomersNum.text = newNumberOfCustomers.ToString();
    }

    public void UpdateMostPopularItem(string itemName)
    {
        mostPopularItem.text = itemName;
    }
}
