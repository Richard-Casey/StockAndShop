using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

/// <summary>
/// 
/// </summary>
public class TillManager : MonoBehaviour
{
    /// <summary>
    /// The till is occupied
    /// </summary>
    public static bool tillIsOccupied = false;
    /// <summary>
    /// The customer queue
    /// </summary>
    private Queue<Customer> customerQueue = new Queue<Customer>();
    /// <summary>
    /// The is processing
    /// </summary>
    private bool isProcessing = false;
    /// <summary>
    /// The till area
    /// </summary>
    public GameObject tillArea; // Assign the TillBG GameObject
    /// <summary>
    /// The till customer prefab
    /// </summary>
    public GameObject tillCustomerPrefab;



    /// <summary>
    /// Processes the customer at till.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProcessCustomerAtTill()
    {
        isProcessing = true;

        while (customerQueue.Count > 0)
        {
            Customer currentCustomer = customerQueue.Peek(); // Peek at the next customer to process but do not dequeue yet

            if (!TillManager.tillIsOccupied)
            {
                TillManager.tillIsOccupied = true; // Mark the till as occupied

                // Now it's safe to dequeue.
                customerQueue.Dequeue();

                // Activate the customer GameObject if it was previously deactivated
                currentCustomer.gameObject.SetActive(true);

                // Process the customer's transaction here and instantiate the till customer prefab
                GameObject tillCustomerInstance = ProcessTransaction(currentCustomer);

                // Wait for the transaction to simulate (4 seconds here as per your requirement)
                yield return new WaitForSeconds(4);

                // Destroy the instantiated till customer prefab and the customer GameObject after processing at the till
                Destroy(tillCustomerInstance);
                Destroy(currentCustomer.gameObject); // This now correctly removes the customer from the shopping area.

                TillManager.tillIsOccupied = false; // Mark the till as no longer occupied
            }

            yield return new WaitForEndOfFrame(); // Wait a bit before checking if we can process the next customer.
        }

        isProcessing = false;
    }


    /// <summary>
    /// Processes the transaction.
    /// </summary>
    /// <param name="customer">The customer.</param>
    /// <returns></returns>
    private GameObject ProcessTransaction(Customer customer)
    {
        GameObject tillCustomerInstance = Instantiate(tillCustomerPrefab, tillArea.transform);

        // Update customer information in the till customer instance
        tillCustomerInstance.transform.Find("CustomerName").GetComponent<TextMeshProUGUI>().text = customer.customerName;
        string itemsList = string.Join("\n", customer.purchasedItems.Select(item => item.itemName));
        string costsList = string.Join("\n", customer.purchasedItems.Select(item => $"£{item.price:F2}"));
        tillCustomerInstance.transform.Find("ListOfItems").GetComponent<TextMeshProUGUI>().text = itemsList;
        tillCustomerInstance.transform.Find("CostOfItems").GetComponent<TextMeshProUGUI>().text = costsList;
        tillCustomerInstance.transform.Find("Feedback").GetComponent<TextMeshProUGUI>().text = customer.feedback;

        // Calculate total cost and profit
        float totalCost = customer.purchasedItems.Sum(item => item.price * item.quantity);
        float totalProfit = customer.purchasedItems.Sum(item => item.profitPerItem * item.quantity);

        // Update total cost and profit in the till customer instance
        tillCustomerInstance.transform.Find("TotalCost").GetComponent<TextMeshProUGUI>().text = $"£{totalCost:F2}";
        tillCustomerInstance.transform.Find("ProfitTotal").GetComponent<TextMeshProUGUI>().text = $"£{totalProfit:F2}";

        // Update player's cash
        UpdatePlayersCash(totalCost);
        InformationBar.Instance.DisplayMessage($"{customer.customerName}. Total cost: £{totalCost:F2}, Total profit: £{totalProfit:F2}");

        // Convert List<Customer.PurchasedItem> to Dictionary<string, int>
        Dictionary<string, int> purchasedItemsDict = customer.purchasedItems
            .GroupBy(item => item.itemName)
            .ToDictionary(group => group.Key, group => group.Sum(item => item.quantity));

        // Update item sales and possibly the highest transaction in the DailySummaryManager
        DailySummaryManager summaryManager = FindObjectOfType<DailySummaryManager>();
        if (summaryManager != null)
        {
            // Pass purchasedItemsDict to RegisterTransaction method
            summaryManager.RegisterTransaction(customer, purchasedItemsDict, totalCost, totalProfit);
            summaryManager.CheckAndUpdateHighestTransactionValue(totalCost, customer.customerName, totalProfit);
        }
        else
        {
            Debug.LogError("DailySummaryManager not found in the scene.");
        }

        return tillCustomerInstance;
    }






    /// <summary>
    /// Updates the players cash.
    /// </summary>
    /// <param name="totalCost">The total cost.</param>
    void UpdatePlayersCash(float totalCost)
    {
        CashDisplay cashDisplay = FindObjectOfType<CashDisplay>();
        if (cashDisplay != null)
        {
            cashDisplay.SetCash(cashDisplay.cashOnHand + totalCost);
        }
    }

    /// <summary>
    /// Adds the customer to queue.
    /// </summary>
    /// <param name="customer">The customer.</param>
    public void AddCustomerToQueue(Customer customer)
    {
        customerQueue.Enqueue(customer);
        if (!isProcessing)
        {
            StartCoroutine(ProcessCustomerAtTill());
        }
    }


}
