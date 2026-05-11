using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject instructionPanel;
    public GameObject resultsPanel;

    [Header("Results Text")]
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
        int last = PlayerPrefs.GetInt("LastCoins", 0);
        int rec = PlayerPrefs.GetInt("RecordCoins", 0);

        Debug.Log("LastCoins from PlayerPrefs: " + last);
        Debug.Log("RecordCoins from PlayerPrefs: " + rec);
        Debug.Log("lastCoinsText is null: " + (lastCoinsText == null));
        Debug.Log("recordText is null: " + (recordText == null));

        if (lastCoinsText != null)
            lastCoinsText.text = "Последний забег: " + last + " монет";
        if (recordText != null)
            recordText.text = "Рекорд: " + rec + " монет";

        if (mainPanel != null) mainPanel.SetActive(false);
        if (instructionPanel != null) instructionPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(true);
    }
    public void OnResetRecord()
    {
        PlayerPrefs.SetInt("RecordCoins", 0);
        PlayerPrefs.SetInt("LastCoins", 0);
        PlayerPrefs.Save();

        if (lastCoinsText != null)
            lastCoinsText.text = "Последний забег: 0 монет";
        if (recordText != null)
            recordText.text = "Рекорд: 0 монет";

        Debug.Log("Record reset!");
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