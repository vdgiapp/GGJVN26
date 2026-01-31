using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject gameOverCanvas;

    [Header("Tooltip")]
    [SerializeField] private Text tooltipText;
    [SerializeField] private Text holdingText;

    [Header("Result")]
    [SerializeField] private Text resultText;

    public void ShowGameUI()
    {
        menuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        pauseCanvas.SetActive(false);
        gameOverCanvas.SetActive(false);
    }

    public void ShowPauseUI(bool show)
    {
        pauseCanvas.SetActive(show);
    }
    
    public void ShowGameOverUI(bool show)
    {
        gameOverCanvas.SetActive(show);
    }

    public void ShowTooltip(string message)
    {
        tooltipText.text = message;
        tooltipText.gameObject.SetActive(true);
    }

    public void ShowHoldingText(string text)
    {
        holdingText.text = text;
    }

    public void SetResultCount(int count, int total)
    {
        resultText.text = $"Satisfied customers: {count}/{total}";
    }

    public void HideTooltip()
    {
        tooltipText.text = "";
        tooltipText.gameObject.SetActive(false);
    }
}