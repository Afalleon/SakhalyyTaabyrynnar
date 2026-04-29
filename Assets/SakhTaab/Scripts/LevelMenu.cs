using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    // ссылки на объекты в инспекторе
    [SerializeField] private GameObject levelButtonPrefab; // префаб кнопки
    [SerializeField] private Transform content; // объект Content из ScrollView
    [SerializeField] private RiddleDataScriptable data; // ScriptableObject с загадками
    [SerializeField] private Text sunText; // компонент Text для валюты

    private void Start()
    {
        sunText.text = RiddleManager.sunCount.ToString(); // устанавливаем количество валюты
        GenerateMenu(); // вызываем метод генерации меню
    }

    // метод генерации меню
    private void GenerateMenu()
    {
        // проходимся по всем уровням
        for (int i = 0; i < data.riddles.Count; i++)
        {
            GameObject obj = Instantiate(levelButtonPrefab, content); // создаем кнопку для каждого уровня
            Button btn = obj.GetComponent<Button>(); // получаем компонент Button

            int levelIndex = i; // сохраняем индекс для замыкания

            obj.GetComponentInChildren<Text>().text = (i + 1).ToString(); // пишем номер уровня

            // текущая и пройденные уровни
            if (i == 0 || data.riddles[i].levelCompleted || (i > 0 && data.riddles[i - 1].levelCompleted))
            {
                btn.interactable = true; // делаем кнопку активной
                btn.onClick.AddListener(() => LoadLevel(levelIndex)); // клик - метод загрузки уровня
                btn.onClick.AddListener(() => AudioManager.instance.PlayClick()); // клик - звук клика
            }
            // непройденные уровни
            else
            {
                btn.interactable = false; // делаем кнопку неактивной
            }
        }
    }

    // метод загрузки уровня
    private void LoadLevel(int index)
    {
        Debug.Log("Загружаем уровень: " + (index + 1));

        RiddleManager.currentRiddleIndex = index; // устанавливаем текущую загадку
        SceneManager.LoadScene(1); // открываем сцену GameScene
    }
}