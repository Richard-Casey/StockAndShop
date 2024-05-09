using UnityEngine;

/// <summary>
/// 
/// </summary>
public class NavigationController : MonoBehaviour
{
    /// <summary>
    /// The cash display
    /// </summary>
    public CanvasGroup cashDisplay;
    /// <summary>
    /// The information bar
    /// </summary>
    public CanvasGroup informationBar;
    /// <summary>
    /// The main menu panel
    /// </summary>
    public CanvasGroup mainMenuPanel;
    /// <summary>
    /// The open shop button
    /// </summary>
    public CanvasGroup openShopButton;
    /// <summary>
    /// The overlay buttons UI
    /// </summary>
    public CanvasGroup overlayButtonsUI;
    /// <summary>
    /// The shelve panel
    /// </summary>
    public CanvasGroup shelvePanel;
    /// <summary>
    /// The start screen
    /// </summary>
    public CanvasGroup startScreen;
    /// <summary>
    /// The stock panel
    /// </summary>
    public CanvasGroup stockPanel;
    /// <summary>
    /// The summary panel
    /// </summary>
    public CanvasGroup summaryPanel;
    /// <summary>
    /// The till panel
    /// </summary>
    [Header("UI Panels")]
    public CanvasGroup tillPanel;
    /// <summary>
    /// The wholesale panel
    /// </summary>
    public CanvasGroup wholesalePanel;

    /// <summary>
    /// Awakes this instance.
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
    /// Sets the panel visibility.
    /// </summary>
    /// <param name="panel">The panel.</param>
    /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
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
    /// Shows the shelf panel.
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
    /// Shows the stock panel.
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
    /// Shows the summary panel.
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
    /// Shows the til l panel.
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
    /// Shows the wholesale panel.
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
    /// Starts the game.
    /// </summary>
    public void StartGame()
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

}
