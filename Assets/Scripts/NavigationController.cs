using UnityEngine;

public class NavigationController : MonoBehaviour
{
    [Header("UI Panels")]
    public CanvasGroup tillPanel;
    public CanvasGroup stockPanel;
    public CanvasGroup shelvePanel;
    public CanvasGroup summaryPanel;
    public CanvasGroup mainMenuPanel;
    public CanvasGroup wholesalePanel;
    public CanvasGroup startScreen;
    public CanvasGroup cashDisplay;
    public CanvasGroup overlayButtonsUI;
    public CanvasGroup openShopButton;
    public CanvasGroup informationBar;

    private void SetPanelVisibility(CanvasGroup panel, bool isVisible)
    {
        panel.alpha = isVisible ? 1 : 0; // 1 is fully visible, 0 is fully transparent
        panel.blocksRaycasts = isVisible;
        panel.interactable = isVisible;
    }

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

    public void QuitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }

    
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

}
