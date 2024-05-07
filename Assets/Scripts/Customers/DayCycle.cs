using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayCycle : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI clockText; // Clock display
    [SerializeField] private CustomerSpawner customerSpawner; // Reference to the CustomerSpawner script
    [SerializeField] private DailySummaryManager dailySummaryManager; // Reference to the DailySummaryManager

    public float currentTime = 0; // Added this line to declare currentTime
    private bool isDayActive = false;
    private bool isPaused = false;
    [SerializeField] private Button normalTimeButton, openShopButton, pauseButton, slowDownTimeButton, speedUpTimeButton;
    private int timeMultiplier = 1;
    public float dayDurationInSeconds = 600; // Duration of a game day in seconds
    public static DayCycle Instance { get; private set; }

    private void Start()
    {
        openShopButton.onClick.AddListener(StartDay);
        speedUpTimeButton.onClick.AddListener(SpeedUpTime);
        slowDownTimeButton.onClick.AddListener(SlowDownTime);
        normalTimeButton.onClick.AddListener(NormalTime);
        pauseButton.onClick.AddListener(TogglePause);

        SetTimeControlButtonsActive(false); // Initially disable time control buttons
        pauseButton.gameObject.SetActive(false); // Initially hide the pause button
    }

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

    void UpdateClock()
    {
        float dayProgress = currentTime / dayDurationInSeconds;
        float dayHours = 9f + (dayProgress * 8f); // Simulating a work day from 9 AM to 5 PM
        int hours = (int)dayHours;
        int minutes = (int)((dayHours - hours) * 60f);
        clockText.text = string.Format("{0:D2}:{1:D2}", hours, minutes);
    }

    public void EndDay()
    {
        if (customerSpawner.ActiveCustomers > 0)
        {
            return; // Wait for all customers to exit
        }

        FinalizeDay();
    }

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
        if (dailySummaryManager != null)
        {
            dailySummaryManager.CheckAndEndDay();
        }
    }


    public void StartDay()
    {
        if (!isDayActive)
        {
            isDayActive = true;
            currentTime = 0;
            customerSpawner.OpenShop();
            openShopButton.gameObject.SetActive(false);
            SetTimeControlButtonsActive(true);
            pauseButton.gameObject.SetActive(true);
            clockText.gameObject.SetActive(true);

            if (dailySummaryManager != null)
                dailySummaryManager.PrepareDay(); // Prepare for a new day

            InformationBar.Instance.DisplayMessage("Shop is now open!");
        }
    }

    private void SetTimeControlButtonsActive(bool isActive)
    {
        speedUpTimeButton.gameObject.SetActive(isActive);
        slowDownTimeButton.gameObject.SetActive(isActive);
        normalTimeButton.gameObject.SetActive(isActive);
    }

    public void NormalTime()
    {
        timeMultiplier = 1;
        Time.timeScale = timeMultiplier;
    }

    public void SlowDownTime()
    {
        if (timeMultiplier > 1)
        {
            timeMultiplier /= 2;
            Time.timeScale = timeMultiplier;
        }
    }

    public void SpeedUpTime()
    {
        if (timeMultiplier < 16)
        {
            timeMultiplier *= 2;
            Time.timeScale = timeMultiplier;
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : timeMultiplier;
        speedUpTimeButton.interactable = !isPaused;
        slowDownTimeButton.interactable = !isPaused;
        normalTimeButton.interactable = !isPaused;

        InformationBar.Instance.DisplayMessage(isPaused ? "Game paused." : "Game resumed.");
    }
}
