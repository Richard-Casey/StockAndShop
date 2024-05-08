using UnityEngine;
using UnityEngine.UI;

public class CustomerSpawner : MonoBehaviour
{
    //private float reputation = 1.0f; // Starting reputation

    [SerializeField] private float reputation = 5f;

    private bool shopIsOpen = false;
    private float spawnRate = 5f; // Start with a default spawn rate of 5 seconds
    private float spawnTimer = 0f;
    public GameObject customerPrefab; // Reference to your Customer prefab
    public DailySummaryManager dailySummaryManager;
    public float maxBudget = 50.0f;
    public float minBudget = 1.0f;
    public Transform shoppingBGParent; // Reference to the ShoppingBG GameObject
    public int activeCustomers { get; private set; } = 0;
    public int ActiveCustomers => activeCustomers;

    public void CustomerEntered()
    {
        activeCustomers++;
    }

    public void CustomerExited()
    {
        activeCustomers--;
        // Only check for ending the day if no customers are left and the day is officially over
        if (activeCustomers == 0 && !DayCycle.Instance.isDayActive)
        {
            FindObjectOfType<DailySummaryManager>().CheckAndEndDay();
        }
    }


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

    public void CloseShop()
    {
        Debug.Log("[CustomerSpawner] CloseShop called. DayCycle active status: " + DayCycle.Instance.isDayActive);
        shopIsOpen = false;
    }

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



    public void UpdateReputation(float change)
    {
        reputation += change;
        reputation = Mathf.Clamp(reputation, 1f, 100f); // Clamp between 1% and 100%


    }

    public float Reputation => reputation;
}
