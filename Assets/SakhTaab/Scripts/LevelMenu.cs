using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public GameObject levelButtonPrefab; // Префаб кнопки
    public Transform content;            // Объект Content из ScrollView
    public RiddleDataScriptable data;             // Ваш ScriptableObject с загадками
    [SerializeField] private Text sunText;

    void Start()
    {
        sunText.text = RiddleManager.sunCount.ToString();
        GenerateMenu();
    }

    void GenerateMenu()
    {
        for (int i = 0; i < data.riddles.Count; i++)
        {
            GameObject obj = Instantiate(levelButtonPrefab, content);
            Button btn = obj.GetComponent<Button>();
            
            int levelIndex = i; // Сохраняем индекс для замыкания

            // Настраиваем текст на кнопке (например, номер уровня)
            obj.GetComponentInChildren<Text>().text = (i + 1).ToString();

            if (i == 0 || data.riddles[i].levelCompleted || (i > 0 && data.riddles[i - 1].levelCompleted))
            {
                // Вешаем событие нажатия
                btn.interactable = true;
                btn.onClick.AddListener(() => LoadLevel(levelIndex));
                btn.onClick.AddListener(() => AudioManager.instance.PlayClick());                
            }
            else
            {
                btn.interactable = false;
            }
        }
    }

    void LoadLevel(int index)
    {
        // Логика загрузки уровня
        Debug.Log("Загружаем уровень: " + (index + 1));

        RiddleManager.currentRiddleIndex = index;
        SceneManager.LoadScene(1);
    }
}