using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Панели")]
    public GameObject instructionPanel;   // Панель с инструкцией
    public GameObject resultsPanel;       // Панель с результатами

    [Header("Текст результатов")]
    public TextMeshProUGUI lastCoinsText;
    public TextMeshProUGUI recordText;

    private void Start()
    {
        // Скрываем панели при старте
        if (instructionPanel != null) instructionPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }

    // Кнопка "Старт"
    public void OnStartButton()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
        else
        {
            // Если GameManager не найден, грузим сцену напрямую
            UnityEngine.SceneManagement.SceneManager.LoadScene("0");
        }
    }

    // Кнопка "Результаты"
    public void OnResultsButton()
    {
        // Закрываем инструкцию если открыта
        if (instructionPanel != null) instructionPanel.SetActive(false);

        // Обновляем текст
        if (GameManager.Instance != null)
        {
            if (lastCoinsText != null)
                lastCoinsText.text = "Последний забег: " + GameManager.Instance.GetLastCoins() + " монет";
            if (recordText != null)
                recordText.text = "Рекорд: " + GameManager.Instance.GetRecord() + " монет";
        }
        else
        {
            // Читаем напрямую из PlayerPrefs если GameManager не существует
            if (lastCoinsText != null)
                lastCoinsText.text = "Последний забег: " + PlayerPrefs.GetInt("LastCoins", 0) + " монет";
            if (recordText != null)
                recordText.text = "Рекорд: " + PlayerPrefs.GetInt("RecordCoins", 0) + " монет";
        }

        // Показываем панель результатов
        if (resultsPanel != null) resultsPanel.SetActive(true);
    }

    // Кнопка "Инструкция"
    public void OnInstructionButton()
    {
        // Закрываем результаты если открыты
        if (resultsPanel != null) resultsPanel.SetActive(false);

        if (instructionPanel != null)
            instructionPanel.SetActive(true);
    }

    // Кнопка "Закрыть" на любой панели
    public void OnClosePanel()
    {
        if (instructionPanel != null) instructionPanel.SetActive(false);
        if (resultsPanel != null) resultsPanel.SetActive(false);
    }
}