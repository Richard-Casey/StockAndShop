using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class TillManager : MonoBehaviour
{
    public GameObject tillArea; // Assign the TillBG GameObject
    public GameObject tillCustomerPrefab;
    private Queue<Customer> customerQueue = new Queue<Customer>();
    private bool isProcessing = false;
    public static bool tillIsOccupied = false;

    public void AddCustomerToQueue(Customer customer)
    {
        customerQueue.Enqueue(customer);
        if (!isProcessing)
        {
            StartCoroutine(ProcessCustomerAtTill());
        }
    }



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

        // Update item sales in the DailySummaryManager
        DailySummaryManager summaryManager = FindObjectOfType<DailySummaryManager>();
        if (summaryManager != null)
        {
            // Pass purchasedItemsDict to RegisterTransaction method
            summaryManager.RegisterTransaction(customer, purchasedItemsDict, totalCost, totalProfit);
        }
        else
        {
            Debug.LogError("DailySummaryManager not found in the scene.");
        }

        return tillCustomerInstance;
    }




    void UpdatePlayersCash(float totalCost)
    {
        CashDisplay cashDisplay = FindObjectOfType<CashDisplay>();
        if (cashDisplay != null)
        {
            cashDisplay.SetCash(cashDisplay.cashOnHand + totalCost);
        }
    }


}
