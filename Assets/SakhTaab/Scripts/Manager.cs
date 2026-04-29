using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    // ссылки на объекты в инспекторе
    [SerializeField] private RiddleDataScriptable data; // ScriptableObject с загадками
    [SerializeField] private GameObject bonusPanel; // панель бонуса
    [SerializeField] private GameObject[] allPanels; // массив со всеми остальными панелями
    private GameInput _input; // переменная для хранения экземпляра системы ввода

    private void Awake()
    {
        _input = new GameInput(); // создаем экземпляр сгенерированного класса ввода (New Input System)        
        _input.UI.Back.performed += ctx => HandleBackPress(); // когда действие Back выполнено, вызывается метод HandleBackPress
    }

    private void OnEnable() => _input.Enable(); // включаем карту ввода, когда объект становится активным
    private void OnDisable() => _input.Disable(); // выключаем карту ввода, когда объект деактивируется

    private void Start()
    {
        Application.targetFrameRate = 120; // устанавливаем целевой FPS в 120

        // для всех уровней до текущего
        for (int i = 0; i < PlayerPrefs.GetInt("CurrentLevel", 0); i++)
        {
            data.riddles[i].levelCompleted = true; // устанавливаем значение Пройдено в ScriptableObject
        }

        RiddleManager.sunCount = PlayerPrefs.GetInt("TotalSun", 764); // загружаем количество валюты
    }

    // метод нажатия кнопки Назад телефона
    private void HandleBackPress()
    {
        // если открыта панель бонуса
        if (bonusPanel != null && bonusPanel.activeSelf)
        {
            AudioManager.instance.PlayClick(); // проигрываем звук клика
            DailyBonus.ClaimBonus(); // вызываем метод получения бонуса
            return;
        }

        // если есть хоть какая-то панель
        if (allPanels != null)
        {
            // проходимся по каждому панелю
            foreach (GameObject panel in allPanels)
            {
                // если панель открыта
                if (panel != null && panel.activeSelf)
                {
                    AudioManager.instance.PlayClick(); // проигрываем звук клика
                    panel.SetActive(false); // закрываем панель
                    return;
                }
            }
        }

        // если панелей нет
        AudioManager.instance.PlayClick(); // проигрываем звук клика
        btnBack_Click(); // вызываем метод кнопки Назад
    }

    // метод кнопки Начать
    public void btnStart_Click()
    {
        // проходимся по всем уровням и устанавливаем текущий
        for (int i = 0; i < data.riddles.Count; i++)
            if (i == 0 || data.riddles[i].levelCompleted || (i > 0 && data.riddles[i - 1].levelCompleted))
                RiddleManager.currentRiddleIndex = i;
        SceneManager.LoadScene(1); // открываем сцену GameScene
    }

    // Метод кнопки Назад
    public void btnBack_Click()
    {
        // назад, если это не начальная сцена
        if (SceneManager.GetActiveScene().buildIndex > 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        // выход, если это начальная
        else
            Application.Quit();
    }

    // Метод кнопки Уровни
    public void btnLevels_Click()
    {
        SceneManager.LoadScene(2); // открываем сцену LevelMenu
    }
}