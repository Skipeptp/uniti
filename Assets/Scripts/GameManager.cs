using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Имена сцен — проверь что они совпадают с названиями в Build Settings
    private const string GAME_SCENE = "0";
    private const string MENU_SCENE = "MainMenu";

    private void Awake()
    {
        // Singleton — один экземпляр на всю игру
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

    // Вызывается при старте игры с главного меню
    public void StartGame()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    // Вызывается когда игрок умер — сохраняем монеты и переходим в меню
    public void GameOver(int coinsCollected)
    {
        // Сохраняем последний результат
        PlayerPrefs.SetInt("LastCoins", coinsCollected);

        // Обновляем рекорд если побили
        int currentRecord = PlayerPrefs.GetInt("RecordCoins", 0);
        if (coinsCollected > currentRecord)
        {
            PlayerPrefs.SetInt("RecordCoins", coinsCollected);
        }

        PlayerPrefs.Save();

        // Загружаем главное меню
        SceneManager.LoadScene(MENU_SCENE);
    }

    public int GetLastCoins() => PlayerPrefs.GetInt("LastCoins", 0);
    public int GetRecord() => PlayerPrefs.GetInt("RecordCoins", 0);
}