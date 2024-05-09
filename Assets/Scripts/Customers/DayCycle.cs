using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 
/// </summary>
public class DayCycle : MonoBehaviour
{
    /// <summary>
    /// The clock text
    /// </summary>
    [SerializeField] public TextMeshProUGUI clockText; // Clock display
    /// <summary>
    /// The customer spawner
    /// </summary>
    [SerializeField] private CustomerSpawner customerSpawner; // Reference to the CustomerSpawner script
    /// <summary>
    /// The daily summary manager
    /// </summary>
    [SerializeField] private DailySummaryManager dailySummaryManager; // Reference to the DailySummaryManager

    /// <summary>
    /// The current time
    /// </summary>
    public float currentTime = 0; // Added this line to declare currentTime
    /// <summary>
    /// The current day
    /// </summary>
    public int currentDay = 0;
    /// <summary>
    /// The is day active
    /// </summary>
    public bool isDayActive = false;
    /// <summary>
    /// The is paused
    /// </summary>
    private bool isPaused = false;
    /// <summary>
    /// The normal time button
    /// </summary>
    [SerializeField] public Button normalTimeButton, openShopButton, pauseButton, slowDownTimeButton, speedUpTimeButton;
    /// <summary>
    /// The time multiplier
    /// </summary>
    private int timeMultiplier = 1;
    /// <summary>
    /// The day duration in seconds
    /// </summary>
    public float dayDurationInSeconds = 600; // Duration of a game day in seconds
    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>
    /// The instance.
    /// </value>
    public static DayCycle Instance { get; private set; }
    /// <summary>
    /// The is first day started
    /// </summary>
    public bool isFirstDayStarted = false;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    private void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        openShopButton.onClick.AddListener(StartDay);
        speedUpTimeButton.onClick.AddListener(SpeedUpTime);
        slowDownTimeButton.onClick.AddListener(SlowDownTime);
        normalTimeButton.onClick.AddListener(NormalTime);
        pauseButton.onClick.AddListener(TogglePause);

        SetTimeControlButtonsActive(false); // Initially disable time control buttons
        pauseButton.gameObject.SetActive(false); // Initially hide the pause button

        // Ensure UI reflects the actual initial state which should be 'Day 0'
        if (dailySummaryManager != null)
        {
            dailySummaryManager.UpdateUI();
        }
    }

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    void Update()
    {
        if (isDayActive && !isPaused)
        {
            currentTime += Time.deltaTime * timeMultiplier;
            UpdateClock();
            if (currentTime >= dayDurationInSeconds)
                EndDay();
        }
    }

    /// <summary>
    /// Updates the clock.
    /// </summary>
    void UpdateClock()
    {
        float dayProgress = currentTime / dayDurationInSeconds;
        float dayHours = 9f + (dayProgress * 8f); // Simulating a work day from 9 AM to 5 PM
        int hours = (int)dayHours;
        int minutes = (int)((dayHours - hours) * 60f);
        clockText.text = string.Format("{0:D2}:{1:D2}", hours, minutes);
    }

    /// <summary>
    /// Ends the day.
    /// </summary>
    public void EndDay()
    {
        Debug.Log("[DayCycle] EndDay called with activeCustomers: " + customerSpawner.ActiveCustomers);
        if (customerSpawner.ActiveCustomers > 0)
        {
            Debug.Log("[DayCycle] Cannot end day, customers still active.");
            return;
        }
        FinalizeDay();
    }

    /// <summary>
    /// Finalizes the day.
    /// </summary>
    private void FinalizeDay()
    {
        isDayActive = false;
        currentTime = 0;
        timeMultiplier = 1;
        Time.timeScale = 1;

        customerSpawner.CloseShop();
        SetTimeControlButtonsActive(false);
        pauseButton.gameObject.SetActive(false);
        clockText.gameObject.SetActive(false);
        openShopButton.gameObject.SetActive(true);

        InformationBar.Instance.DisplayMessage("Day ended. Shop is now closed.");
        dailySummaryManager.EndOfDaySummary();
        currentDay++;  // Increment here after all end-of-day processing
    }

    /// <summary>
    /// Starts the day.
    /// </summary>
    public void StartDay()
    {
        Debug.Log("[DayCycle] StartDay called. isDayActive: " + isDayActive);
        if (!isDayActive)
        {
            isDayActive = true;
            currentTime = 0;
            customerSpawner.OpenShop();
            openShopButton.gameObject.SetActive(false);
            SetTimeControlButtonsActive(true);
            pauseButton.gameObject.SetActive(true);
            clockText.gameObject.SetActive(true);

            if (currentDay == 0)
            {
                Debug.Log("[DayCycle] Handling first day.");
                dailySummaryManager.StartNewDayWithoutReset(); // Make sure this handles day 0 properly
            }
            else
            {
                Debug.Log("[DayCycle] Handling new day.");
                dailySummaryManager.PrepareForNewDay(); // This should only be called from day 1 onwards
            }

            InformationBar.Instance.DisplayMessage("Shop is now open!");
        }
    }







    /// <summary>
    /// Sets the time control buttons active.
    /// </summary>
    /// <param name="isActive">if set to <c>true</c> [is active].</param>
    public void SetTimeControlButtonsActive(bool isActive)
    {
        speedUpTimeButton.gameObject.SetActive(isActive);
        slowDownTimeButton.gameObject.SetActive(isActive);
        normalTimeButton.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// Normals the time.
    /// </summary>
    public void NormalTime()
    {
        timeMultiplier = 1;
        Time.timeScale = timeMultiplier;
    }

    /// <summary>
    /// Slows down time.
    /// </summary>
    public void SlowDownTime()
    {
        if (timeMultiplier > 1)
        {
            timeMultiplier /= 2;
            Time.timeScale = timeMultiplier;
        }
    }

    /// <summary>
    /// Speeds up time.
    /// </summary>
    public void SpeedUpTime()
    {
        if (timeMultiplier < 16)
        {
            timeMultiplier *= 2;
            Time.timeScale = timeMultiplier;
        }
    }

    /// <summary>
    /// Toggles the pause.
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : timeMultiplier;
        speedUpTimeButton.interactable = !isPaused;
        slowDownTimeButton.interactable = !isPaused;
        normalTimeButton.interactable = !isPaused;

        InformationBar.Instance.DisplayMessage(isPaused ? "Game paused." : "Game resumed.");
    }

    /// <summary>
    /// Opens the shop UI updates.
    /// </summary>
    public void OpenShopUIUpdates()
    {
        SetTimeControlButtonsActive(true);
        pauseButton.gameObject.SetActive(true);
        clockText.gameObject.SetActive(true);
    }

    /// <summary>
    /// Starts the new day.
    /// </summary>
    public void StartNewDay()
    {
        if (currentDay == 0)
        {
            // When it's the transition from day 0 to day 1
            dailySummaryManager.StartNewDayWithoutReset(); // Starts the day without resetting stats
        }
        else
        {
            // Normal day start for subsequent days
            dailySummaryManager.PrepareForNewDay(); // Resets stats including expenses
        }
        currentDay++; // Increment the day counter for the next operation
    }

}
