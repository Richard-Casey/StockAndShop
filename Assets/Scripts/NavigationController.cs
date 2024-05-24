using UnityEngine;

/// <summary>
/// Manages the navigation and visibility of various UI panels in the game.
/// </summary>
public class NavigationController : MonoBehaviour
{
    /// <summary>
    /// The CanvasGroup for the cash display panel.
    /// </summary>
    public CanvasGroup cashDisplay;

    /// <summary>
    /// The CanvasGroup for the information bar.
    /// </summary>
    public CanvasGroup informationBar;

    /// <summary>
    /// The CanvasGroup for the main menu panel.
    /// </summary>
    public CanvasGroup mainMenuPanel;

    /// <summary>
    /// The CanvasGroup for the open shop button.
    /// </summary>
    public CanvasGroup openShopButton;

    /// <summary>
    /// The CanvasGroup for the overlay buttons UI.
    /// </summary>
    public CanvasGroup overlayButtonsUI;

    /// <summary>
    /// The CanvasGroup for the shelve panel.
    /// </summary>
    public CanvasGroup shelvePanel;

    /// <summary>
    /// The CanvasGroup for the start screen.
    /// </summary>
    public CanvasGroup startScreen;

    /// <summary>
    /// The CanvasGroup for the stock panel.
    /// </summary>
    public CanvasGroup stockPanel;

    /// <summary>
    /// The CanvasGroup for the summary panel.
    /// </summary>
    public CanvasGroup summaryPanel;

    [Header("UI Panels")]
    /// <summary>
    /// The CanvasGroup for the till panel.
    /// </summary>
    public CanvasGroup tillPanel;

    /// <summary>
    /// The CanvasGroup for the wholesale panel.
    /// </summary>
    public CanvasGroup wholesalePanel;

    /// <summary>
    /// Initializes the visibility of UI panels when the script instance is loaded.
    /// </summary>
    void Awake()
    {
        SetPanelVisibility(tillPanel, false);
        SetPanelVisibility(stockPanel, false);
        SetPanelVisibility(summaryPanel, false);
        SetPanelVisibility(shelvePanel, false);
        SetPanelVisibility(mainMenuPanel, true);
        SetPanelVisibility(wholesalePanel, false);
        SetPanelVisibility(startScreen, true);
        SetPanelVisibility(cashDisplay, false);
        SetPanelVisibility(overlayButtonsUI, false);
        SetPanelVisibility(openShopButton, false);
        SetPanelVisibility(informationBar, false);

    }

    /// <summary>
    /// Sets the visibility of a given panel.
    /// </summary>
    /// <param name="panel">The CanvasGroup of the panel to set visibility for.</param>
    /// <param name="isVisible">True to make the panel visible, false to make it invisible.</param>
    private void SetPanelVisibility(CanvasGroup panel, bool isVisible)
    {
        panel.alpha = isVisible ? 1 : 0; // 1 is fully visible, 0 is fully transparent
        panel.blocksRaycasts = isVisible;
        panel.interactable = isVisible;
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }

    /// <summary>
    /// Shows the shelf panel and hides other panels.
    /// </summary>
    public void ShowShelfPanel()
    {
        SetPanelVisibility(tillPanel, false);
        SetPanelVisibility(stockPanel, false);
        SetPanelVisibility(summaryPanel, false);
        SetPanelVisibility(shelvePanel, true);
        SetPanelVisibility(mainMenuPanel, false);
        SetPanelVisibility(wholesalePanel, false);
        SetPanelVisibility(startScreen, false);
        SetPanelVisibility(cashDisplay, true);
        SetPanelVisibility(overlayButtonsUI, true);
        SetPanelVisibility(openShopButton, true);
        SetPanelVisibility(informationBar, true);

    }

    /// <summary>
    /// Shows the stock panel and hides other panels.
    /// </summary>
    public void ShowStockPanel()
    {
        SetPanelVisibility(tillPanel, false);
        SetPanelVisibility(stockPanel, true);
        SetPanelVisibility(summaryPanel, false);
        SetPanelVisibility(shelvePanel, false);
        SetPanelVisibility(mainMenuPanel, false);
        SetPanelVisibility(wholesalePanel, false);
        SetPanelVisibility(startScreen, false);
        SetPanelVisibility(cashDisplay, true);
        SetPanelVisibility(overlayButtonsUI, true);
        SetPanelVisibility(openShopButton, true);
        SetPanelVisibility(informationBar, true);

    }

    /// <summary>
    /// Shows the summary panel and hides other panels.
    /// </summary>
    public void ShowSummaryPanel()
    {
        SetPanelVisibility(tillPanel, false);
        SetPanelVisibility(stockPanel, false);
        SetPanelVisibility(summaryPanel, true);
        SetPanelVisibility(shelvePanel, false);
        SetPanelVisibility(mainMenuPanel, false);
        SetPanelVisibility(wholesalePanel, false);
        SetPanelVisibility(startScreen, false);
        SetPanelVisibility(cashDisplay, true);
        SetPanelVisibility(overlayButtonsUI, true);
        SetPanelVisibility(openShopButton, true);
        SetPanelVisibility(informationBar, true);

    }

    /// <summary>
    /// Shows the till panel and hides other panels.
    /// </summary>
    public void ShowTilLPanel()
    {
        SetPanelVisibility(tillPanel, true);
        SetPanelVisibility(stockPanel, false);
        SetPanelVisibility(summaryPanel, false);
        SetPanelVisibility(shelvePanel, false);
        SetPanelVisibility(mainMenuPanel, false);
        SetPanelVisibility(wholesalePanel, false);
        SetPanelVisibility(startScreen, false);
        SetPanelVisibility(cashDisplay, true);
        SetPanelVisibility(overlayButtonsUI, true);
        SetPanelVisibility(openShopButton, true);
        SetPanelVisibility(informationBar, true);
    }

    /// <summary>
    /// Shows the wholesale panel and hides other panels.
    /// </summary>
    public void ShowWholesalePanel()
    {
        SetPanelVisibility(tillPanel, false);
        SetPanelVisibility(stockPanel, false);
        SetPanelVisibility(summaryPanel, false);
        SetPanelVisibility(shelvePanel, false);
        SetPanelVisibility(mainMenuPanel, false);
        SetPanelVisibility(wholesalePanel, true);
        SetPanelVisibility(startScreen, false);
        SetPanelVisibility(cashDisplay, true);
        SetPanelVisibility(overlayButtonsUI, true);
        SetPanelVisibility(openShopButton, true);
        SetPanelVisibility(informationBar, true);

    }

    /// <summary>
    /// Starts the game by showing the till panel and hiding other panels.
    /// </summary>
    public void StartGame()
    {
        SetPanelVisibility(tillPanel, false);
        SetPanelVisibility(stockPanel, false);
        SetPanelVisibility(summaryPanel, false);
        SetPanelVisibility(shelvePanel, false);
        SetPanelVisibility(mainMenuPanel, false);
        SetPanelVisibility(wholesalePanel, true);
        SetPanelVisibility(startScreen, false);
        SetPanelVisibility(cashDisplay, true);
        SetPanelVisibility(overlayButtonsUI, true);
        SetPanelVisibility(openShopButton, true);
        SetPanelVisibility(informationBar, true);

    }

}
