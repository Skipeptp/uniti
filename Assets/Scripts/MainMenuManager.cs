using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Панели")]
    public GameObject mainPanel;
    public GameObject instructionPanel;
    public GameObject resultsPanel;

    [Header("Текст результатов")]
    public TextMeshProUGUI lastCoinsText;
    public TextMeshProUGUI recordText;

    private void Start()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (instructionPanel != null) instructionPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    public void OnStartButton()
    {
        SceneManager.LoadScene("0");
    }

    public void OnResultsButton()
    {
        if (lastCoinsText != null)
            lastCoinsText.text = "Последний забег: " + PlayerPrefs.GetInt("LastCoins", 0) + " монет";
        if (recordText != null)
            recordText.text = "Рекорд: " + PlayerPrefs.GetInt("RecordCoins", 0) + " монет";

        if (mainPanel != null) mainPanel.SetActive(false);
        if (instructionPanel != null) instructionPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(true);
    }

    public void OnInstructionButton()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
        if (instructionPanel != null) instructionPanel.SetActive(true);
    }

    public void OnClosePanel()
    {
        if (instructionPanel != null) instructionPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }
}