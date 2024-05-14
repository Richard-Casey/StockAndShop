using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the spawning of customers in the shop.
/// </summary>
public class CustomerSpawner : MonoBehaviour
{
    /// <summary>
    /// The reputation of the shop.
    /// </summary>
    [SerializeField] private float reputation = 5f;

    /// <summary>
    /// Indicates whether the shop is open.
    /// </summary>
    private bool shopIsOpen = false;

    /// <summary>
    /// The rate at which customers spawn, in seconds.
    /// </summary>
    private float spawnRate = 5f; // Start with a default spawn rate of 5 seconds

    /// <summary>
    /// The timer for spawning customers.
    /// </summary>
    private float spawnTimer = 0f;

    /// <summary>
    /// The prefab for the customer.
    /// </summary>
    public GameObject customerPrefab; // Reference to Customer prefab

    /// <summary>
    /// Reference to the daily summary manager.
    /// </summary>
    public DailySummaryManager dailySummaryManager;

    /// <summary>
    /// The maximum budget for a customer.
    /// </summary>
    public float maxBudget = 50.0f;

    /// <summary>
    /// The minimum budget for a customer.
    /// </summary>
    public float minBudget = 1.0f;

    /// <summary>
    /// The parent transform for the shopping background.
    /// </summary>
    public Transform shoppingBGParent; // Reference to the ShoppingBG GameObject

    /// <summary>
    /// The number of active customers in the shop.
    /// </summary>
    public int activeCustomers { get; private set; } = 0;

    /// <summary>
    /// Gets the number of active customers in the shop.
    /// </summary>
    public int ActiveCustomers => activeCustomers;

    /// <summary>
    /// Called when a customer enters the shop.
    /// </summary>
    public void CustomerEntered()
    {
        activeCustomers++;
    }

    /// <summary>
    /// Called when a customer exits the shop.
    /// </summary>
    public void CustomerExited()
    {
        activeCustomers--;
        // Only check for ending the day if no customers are left and the day is officially over
        if (activeCustomers == 0 && !DayCycle.Instance.isDayActive)
        {
            FindObjectOfType<DailySummaryManager>().CheckAndEndDay();
        }
    }

    /// <summary>
    /// Adjusts the spawn rate of customers based on the shop's reputation.
    /// </summary>
    void AdjustSpawnRateBasedOnReputation()
    {
        // This formula adjusts the spawn rate so that a lower reputation results in fewer customers
        // and a higher reputation results in more frequent customers.
        // For a reputation of 5, the spawn rate might be around 15 seconds between customers.
        // As the reputation increases, the spawn rate decreases, allowing for more customers to enter.

        float minSpawnRate = 15f; // Minimum time between spawns at lowest reputation
        float maxSpawnRate = 2f;  // Maximum spawn frequency at highest reputation

        // Calculate spawn rate based on reputation
        // Linear interpolation from minSpawnRate to maxSpawnRate based on reputation percentage
        spawnRate = Mathf.Lerp(minSpawnRate, maxSpawnRate, reputation / 100f);
    }

    /// <summary>
    /// Spawns a new customer in the shop.
    /// </summary>
    void SpawnCustomer()
    {
        float randomBudget = Random.Range(minBudget, maxBudget);
        GameObject customerObject = Instantiate(customerPrefab, shoppingBGParent);
        Customer customer = customerObject.GetComponent<Customer>();
        customer.budget = randomBudget;
        LayoutRebuilder.ForceRebuildLayoutImmediate(shoppingBGParent.GetComponent<RectTransform>());
        dailySummaryManager.RegisterCustomerEntry();
        InformationBar.Instance.DisplayMessage($"A new customer has entered the shop.");
    }

    /// <summary>
    /// Updates the customer spawner.
    /// </summary>
    void Update()
    {
        if (shopIsOpen && shoppingBGParent.childCount <= 9) // Allows for a max of exactly 10 customers
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnCustomer();
                AdjustSpawnRateBasedOnReputation();
                spawnTimer = spawnRate;
            }
        }

        AdjustSpawnRateBasedOnReputation();
    }

    /// <summary>
    /// Closes the shop.
    /// </summary>
    public void CloseShop()
    {
        Debug.Log("[CustomerSpawner] CloseShop called. DayCycle active status: " + DayCycle.Instance.isDayActive);
        shopIsOpen = false;
    }

    /// <summary>
    /// Opens the shop.
    /// </summary>
    public void OpenShop()
    {
        if (!DayCycle.Instance.isDayActive)
        {
            DayCycle.Instance.StartNewDay(); // Handle day start properly
        }
        shopIsOpen = true;
        spawnTimer = spawnRate; // Reset spawn timer
        Debug.Log("Shop is now open.");
    }


    /// <summary>
    /// Updates the reputation of the shop.
    /// </summary>
    /// <param name="change">The change in reputation.</param>
    public void UpdateReputation(float change)
    {
        reputation += change;
        reputation = Mathf.Clamp(reputation, 1f, 100f); // Clamp between 1% and 100%


    }

    /// <summary>
    /// Gets the reputation of the shop.
    /// </summary>
    public float Reputation => reputation;
}
