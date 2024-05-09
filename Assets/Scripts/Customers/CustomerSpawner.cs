using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class CustomerSpawner : MonoBehaviour
{
    /// <summary>
    /// The reputation
    /// </summary>
    [SerializeField] private float reputation = 5f;

    /// <summary>
    /// The shop is open
    /// </summary>
    private bool shopIsOpen = false;
    /// <summary>
    /// The spawn rate
    /// </summary>
    private float spawnRate = 5f; // Start with a default spawn rate of 5 seconds
    /// <summary>
    /// The spawn timer
    /// </summary>
    private float spawnTimer = 0f;
    /// <summary>
    /// The customer prefab
    /// </summary>
    public GameObject customerPrefab; // Reference to your Customer prefab
    /// <summary>
    /// The daily summary manager
    /// </summary>
    public DailySummaryManager dailySummaryManager;
    /// <summary>
    /// The maximum budget
    /// </summary>
    public float maxBudget = 50.0f;
    /// <summary>
    /// The minimum budget
    /// </summary>
    public float minBudget = 1.0f;
    /// <summary>
    /// The shopping bg parent
    /// </summary>
    public Transform shoppingBGParent; // Reference to the ShoppingBG GameObject
    /// <summary>
    /// Gets the active customers.
    /// </summary>
    /// <value>
    /// The active customers.
    /// </value>
    public int activeCustomers { get; private set; } = 0;
    /// <summary>
    /// Gets the active customers.
    /// </summary>
    /// <value>
    /// The active customers.
    /// </value>
    public int ActiveCustomers => activeCustomers;

    /// <summary>
    /// Customers the entered.
    /// </summary>
    public void CustomerEntered()
    {
        activeCustomers++;
    }

    /// <summary>
    /// Customers the exited.
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
    /// Adjusts the spawn rate based on reputation.
    /// </summary>
    void AdjustSpawnRateBasedOnReputation()
    {
        // This formula adjusts the spawn rate so that a lower reputation results in fewer customers
        // and a higher reputation results in more frequent customers.
        // For a reputation of 5, the spawn rate might be around 15 seconds between customers.
        // As the reputation increases, the spawn rate decreases, allowing for more customers to enter.

        // Adjust these values to change how drastically the spawn rate changes with reputation.
        float minSpawnRate = 15f; // Minimum time between spawns at lowest reputation
        float maxSpawnRate = 2f;  // Maximum spawn frequency at highest reputation

        // Calculate spawn rate based on reputation
        // Linear interpolation from minSpawnRate to maxSpawnRate based on reputation percentage
        spawnRate = Mathf.Lerp(minSpawnRate, maxSpawnRate, reputation / 100f);

        // Optionally, log the new spawn rate for debugging
        // Debug.Log($"New spawn rate: {spawnRate} seconds.");
    }

    /// <summary>
    /// Spawns the customer.
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
    /// Updates this instance.
    /// </summary>
    void Update()
    {
        if (shopIsOpen && shoppingBGParent.childCount <= 9) // Allows for exactly 10 customers
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
    /// Updates the reputation.
    /// </summary>
    /// <param name="change">The change.</param>
    public void UpdateReputation(float change)
    {
        reputation += change;
        reputation = Mathf.Clamp(reputation, 1f, 100f); // Clamp between 1% and 100%


    }

    /// <summary>
    /// Gets the reputation.
    /// </summary>
    /// <value>
    /// The reputation.
    /// </value>
    public float Reputation => reputation;
}
