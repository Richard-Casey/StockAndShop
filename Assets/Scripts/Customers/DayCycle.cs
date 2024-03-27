using System.Collections;
using UnityEngine;
using UnityEngine.UI; // For interacting with UI elements
using TMPro; // For TextMeshPro elements

public class DayCycle : MonoBehaviour
{
    public float dayDurationInSeconds = 600; // 10 Minutes for 1 day
    private float currentTime = 0;
    private bool isDayActive = false;
    private int timeMultiplier = 1;
    private bool isPaused = false;

    [SerializeField] private Button openShopButton; // Button to open the shop
    [SerializeField] private Button speedUpTimeButton; // Button to speed up time
    [SerializeField] private Button slowDownTimeButton; // Button to slow down time
    [SerializeField] private Button normalTimeButton; // Button to reset time speed
    [SerializeField] private TextMeshProUGUI clockText; // Clock display
    [SerializeField] private CustomerSpawner customerSpawner; // Reference to the CustomerSpawner script
    [SerializeField] private Button pauseButton;

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

    void Update()
    {
        if (isDayActive && !isPaused)
        {
            currentTime += Time.deltaTime * timeMultiplier; // Adjust for speed changes
            UpdateClock(); // Update the clock based on current time

            if (currentTime >= dayDurationInSeconds)
            {
                EndDay();
            }
        }
    }

    void UpdateClock()
    {
        float dayProgress = currentTime / dayDurationInSeconds;
        float dayHours = 9f + (dayProgress * 8f); // Assuming the shop is open from 9 AM to 5 PM
        int hours = (int)dayHours;
        int minutes = (int)((dayHours - hours) * 60f);
        clockText.text = string.Format("{0:D2}:{1:D2}", hours, minutes); // Format time in HH:MM
    }

    public void StartDay()
    {
        isDayActive = true;
        currentTime = 0; // Reset time for the start of the day
        clockText.gameObject.SetActive(true); // Show the clock

        customerSpawner.OpenShop(); // Tell the CustomerSpawner to start spawning customers

        openShopButton.gameObject.SetActive(false);

        SetTimeControlButtonsActive(true); // Enable time control buttons
        pauseButton.gameObject.SetActive(true); // Show the pause button
        InformationBar.Instance.DisplayMessage("Shop is now open!");
    }

    public void EndDay()
    {
        isDayActive = false;
        currentTime = 0;
        timeMultiplier = 1; // Reset time speed
        Time.timeScale = 1; // Reset Unity time scale

        customerSpawner.CloseShop(); // Tell the CustomerSpawner to stop spawning customers

        SetTimeControlButtonsActive(false); // Disable time control buttons
        pauseButton.gameObject.SetActive(false); // Hide the pause button
        clockText.gameObject.SetActive(false); // Hide the clock
        openShopButton.gameObject.SetActive(true);
        InformationBar.Instance.DisplayMessage("Day ended. Shop is now closed.");
    }

    private void SetTimeControlButtonsActive(bool isActive)
    {
        speedUpTimeButton.gameObject.SetActive(isActive);
        slowDownTimeButton.gameObject.SetActive(isActive);
        normalTimeButton.gameObject.SetActive(isActive);
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : timeMultiplier; // Pause or resume the game

        // Disable or enable time control buttons based on pause state
        speedUpTimeButton.interactable = !isPaused;
        slowDownTimeButton.interactable = !isPaused;
        normalTimeButton.interactable = !isPaused;

        InformationBar.Instance.DisplayMessage(isPaused ? "Game paused." : "Game resumed.");
    }

    public void SpeedUpTime()
    {
        // Incrementally increase the time scale
        if (timeMultiplier < 16) // Set a cap for the multiplier, for example, 16x
        {
            timeMultiplier *= 2;
            Time.timeScale = timeMultiplier;
            Debug.Log("Time sped up");
        }
    }

    public void SlowDownTime()
    {
        // Decrementally decrease the time scale
        if (timeMultiplier > 1) // Ensure timeMultiplier doesn't go below normal speed
        {
            timeMultiplier /= 2;
            Time.timeScale = timeMultiplier;
            Debug.Log("Time slowed down");
        }
    }

    public void NormalTime()
    {
        // Return time scale back to normal
        timeMultiplier = 1;
        Time.timeScale = timeMultiplier;
        Debug.Log("Time reset to normal");
    }

}
