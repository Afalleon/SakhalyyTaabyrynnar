using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

public class RiddleManager : MonoBehaviour
{
    // статические переменные
    public static RiddleManager instance; // экземпляр класса
    public static int currentRiddleIndex; // индекс текущего уровня
    public static int sunCount; // количество валюты

    // ссылки на объекты в инспекторе
    [SerializeField] private GameObject levelComplete; // панель завершенного уровня
    [SerializeField] private GameObject gameComplete; // панель завершенной игры
    [SerializeField] private RiddleDataScriptable data; // ScriptableObject с загадками
    [SerializeField] private Text sunText; // компонент Text для валюты
    [SerializeField] private Text levelID; // компонент Text для номера уровня
    [SerializeField] private Text riddleText; // компонент Text для текста загадки
    [SerializeField] private Image riddleImage; // компонент Image для изображения загадки
    [SerializeField] private Text addSunText; // компонент Text для добавленной валюты
    [SerializeField] private CellData[] wordCellList; // массив с ячейками ответа
    [SerializeField] private CellData[] letterCellList; // массив ячеек с буквами
    [SerializeField] private char[] yakutABC; // якутский алфавит
    
    // приватные переменные
    private GameStatus gameStatus; // статус игры
    private char[] lettersArray = new char[16]; // массив с буквами для ввода ответа
    private List<int> selectedLettersIndex; // список нажатых букв
    private string answerText; // ответ загадки
    private int currentAnswerIndex; // текущая пустая ячейка для ввода ответа
    private bool correctAnswer; // верный ответ
    
    // реализация паттерна Singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }            
        else
            Destroy(gameObject);
    }

    // открытие страницы уровня
    private void Start()
    {
        selectedLettersIndex = new List<int>(); // инициализируем список нажатых букв

        // если индекс текущего уровня больше последнего
        if (currentRiddleIndex == data.riddles.Count)
            currentRiddleIndex = currentRiddleIndex--; // устанавливаем текущим последний уровень

        SetRiddle(currentRiddleIndex); // вызываем метод установки загадки
    }

    // метод Следующий уровень
    public void SetNextRiddle()
    {
        levelComplete.SetActive(false); // закрываем панель завершенного уровня

        // если пройденный уровень был последним
        if (currentRiddleIndex == data.riddles.Count)
        {
            sunText.text = sunCount.ToString(); // устанавливаем количество валюты
            currentRiddleIndex = currentRiddleIndex--; // устанавливаем текущим последний уровень
            gameComplete.SetActive(true); // открываем панель завершенной игры
            return;
        }

        // если нет, вызываем метод установки загадки
        SetRiddle(currentRiddleIndex);
    }

    // метод установки загадки
    public void SetRiddle(int index)
    {
        gameStatus = GameStatus.Playing; // устанавливаем статус Играет
        sunText.text = sunCount.ToString(); // обновляем количество валюты
                
        // получаем данные новой загадки
        levelID.text = (index + 1).ToString(); // номер уровня
        riddleText.text = data.riddles[index].riddleText; // текст загадки
        answerText = data.riddles[index].answer; // ответ загадки

        ResetRiddle(); // подготавливаем ячейки

        // добавляем буквы ответа в массив с буквами для ввода ответа
        for (int i = 0; i < answerText.Length; i++)
        {
            lettersArray[i] = char.ToUpper(answerText[i]);
        }

        // оставшуюся часть заполняем случайными буквами
        for (int j = answerText.Length; j < lettersArray.Length; j++)
        {
            lettersArray[j] = yakutABC[UnityEngine.Random.Range(0, yakutABC.Length)];
        }

        // перемешиваем буквы
        lettersArray = ShuffleList.ShuffleListItems<char>(lettersArray.ToList()).ToArray();

        // устанавливаем буквы в ячейки
        for (int k = 0;  k < letterCellList.Length; k++)
        {
            letterCellList[k].SetCell(lettersArray[k]);
        }
    }

    // метод очистки загадки
    public void ResetRiddle()
    {
        // очищаем ячейки ответа
        for (int i = 0; i < wordCellList.Length; i++)
        {
            wordCellList[i].gameObject.SetActive(true);
            wordCellList[i].SetCell(' ');
        }

        // оставляем только нужное количество ячеек ответа
        for (int i = answerText.Length; i < wordCellList.Length; i++)
        {
            wordCellList[i].gameObject.SetActive(false);
        }

        // активируем все ячейки с буквами
        for (int i = 0; i < letterCellList.Length; i++)
        {
            letterCellList[i].gameObject.SetActive(true);
        }

        selectedLettersIndex.Clear(); // очищаем список нажатых букв
        currentAnswerIndex = 0; // устанавливаем текущую пустую ячейку для ввода ответа
        HelpManager.deleteFalseLettersIsUsed = false; // подсказка Удалить лишние не использована
        Array.Clear(lettersArray, 0, lettersArray.Length); // очищаем массив с буквами для ввода ответа
    }

    // метод нажатия ячейки с буквой
    public void SelectedLetter(CellData value)
    {
        // ничего не делаем, если статус игры Следующий или если все ячейки заполнены
        if (gameStatus == GameStatus.Next || wordCellList[currentAnswerIndex].cellValue != ' ')
            return;

        // вставляем букву в ячейку ответа и убираем нажатую ячейку
        selectedLettersIndex.Add(value.transform.GetSiblingIndex());
        value.gameObject.SetActive(false);
        wordCellList[currentAnswerIndex].SetCell(value.cellValue);

        // обновляем текущую ячейку для ввода ответа и проверяем ответ
        SetCurrentAnswerIndex();
        CheckAnswer();
    }

    // метод задания текущей ячейки для ввода ответа
    public void SetCurrentAnswerIndex()
    {
        // проходимся по всем ячейкам ответа
        for (int i = 0; i < answerText.Length; i++)
        {
            // если ячейка пустая
            if (wordCellList[i].cellValue == ' ')
            {
                currentAnswerIndex = i; // присваиваем индекс
                break; // выходим из цикла
            }
        }
    }

    // метод проверки ответа
    public void CheckAnswer()
    {
        // если все ячейки заполнены
        if (wordCellList[currentAnswerIndex].cellValue != ' ')
        {
            correctAnswer = true;

            // проходимся по всем ячейкам ответа
            for (int i = 0; i < answerText.Length; i++)
            {
                // если буквы ячейки и ответа не соответствуют
                if (char.ToUpper(answerText[i]) != char.ToUpper(wordCellList[i].cellValue))
                {
                    correctAnswer = false;
                    break;
                }
            }

            // если ответ верный
            if (correctAnswer)
            {
                AudioManager.instance.PlayTrueAnswer(); // проигрывание звука правильного ответа
                Debug.Log("Правильный ответ!");

                // если вибрация включена
                if (PlayerPrefs.GetInt("VibrationEnabled", 1) == 1)
                    Handheld.Vibrate(); // вибрация

                gameStatus = GameStatus.Next; // статус игры Следующий
                currentRiddleIndex++; // текущий уровень = следующий уровень

                CompleteRiddle(); // вызываем метод завершения уровня
            }
            // если ответ неверный
            else
            {
                AudioManager.instance.PlayFalseAnswer(); // проигрывание звука неправильного ответа
                Debug.Log("Неравильный ответ!");
            }
        }
    }

    // метод завершения уровня
    public void CompleteRiddle()
    {
        levelComplete.SetActive(true); // активируем панель завершенного уровня

        int currentCompleteRiddleIndex = currentRiddleIndex - 1; // берем индекс пройденного уровня

        // устанавливаем лексическое значение и изображение загадки
        riddleText.text = data.riddles[currentCompleteRiddleIndex].textDescription;
        riddleImage.sprite = data.riddles[currentCompleteRiddleIndex].imageDescription;

        // если уровень ранее не пройден
        if (data.riddles[currentCompleteRiddleIndex].levelCompleted == false)
        {
            // добавляем валюту (количество валюты = количество букв в слове)
            int addSunCount = data.riddles[currentCompleteRiddleIndex].answer.Length;
            addSunText.text = $"+{addSunCount}";
            sunCount += addSunCount;
        }

        data.riddles[currentCompleteRiddleIndex].levelCompleted = true; // отмечаем, что уровень пройден

        // сохраняем данные
        PlayerPrefs.SetInt("CurrentLevel", currentRiddleIndex);
        PlayerPrefs.SetInt("TotalSun", sunCount);
        PlayerPrefs.Save();
    }

    // метод удаления последней введенной буквы
    public void ResetLastCell()
    {
        // если хоть какая-то буква нажата
        if (selectedLettersIndex.Count > 0)
        {
            // идем от конца списка введенных букв
            for (int i = selectedLettersIndex.Count - 1; i >= 0; i--)
            {
                int index = selectedLettersIndex[i];

                // если буква не помечена как правильная
                if (!letterCellList[index].cellTrueValue)
                {
                    letterCellList[index].gameObject.SetActive(true); // возвращаем ячейку с буквой
                    selectedLettersIndex.Remove(index); // удаляем ячейку со списка нажатых букв

                    // идем от конца ячеек ответа
                    for (int j = wordCellList.Length - 1; j >= 0; j--)
                    {
                        // если ячейка не пустая и не помечена как правильная
                        if (wordCellList[j].cellValue != ' ' && !wordCellList[j].cellTrueValue)
                        {
                            wordCellList[j].SetCell(' '); // очищаем ячейку
                            break; // выходим
                        }                   
                    }

                    SetCurrentAnswerIndex(); // обновляем текущую ячейку для ввода ответа
                    break; // дальше не идем
                }
            }                           
        }
    }

    // метод удаления всех введенных букв
    public void ClearAnswer()
    {
        // если есть нажатые буквы
        if (selectedLettersIndex.Count > 0)
        {
            // для каждой введенной буквы
            foreach (int index in new List<int>(selectedLettersIndex))
            {
                // если буква не помечена как правильная
                if (!letterCellList[index].cellTrueValue)
                {
                    // возвращаем ячейку с буквой
                    letterCellList[index].gameObject.SetActive(true);
                    selectedLettersIndex.Remove(index);
                }
            }

            // для каждой ячейки ответа
            for (int i = 0; i < wordCellList.Length; i++)
            {
                // если ячейка не помечена как правильная
                if (!wordCellList[i].cellTrueValue)
                    wordCellList[i].SetCell(' '); // очищаем ячейку
            }

            SetCurrentAnswerIndex(); // обновляем текущую ячейку для ввода ответа
        }
    }
    
    // метод для получения статуса игры
    public GameStatus GetGameStatus()
    {
        return gameStatus;
    }

    // метод для получения ответа
    public string GetAnswer()
    {
        return answerText;
    }

    // метод для получения массива ячеек ответа
    public CellData[] GetWordCells()
    {
        return wordCellList;
    }

    // метод для получения массива ячеек с буквами
    public CellData[] GetLetterCells()
    {
        return letterCellList;
    }

    // метод Достаточно ли валюты
    public bool HasEnoughSun(int cost)
    {
        return sunCount >= cost;
    }

    // метод траты валюты
    public void SpendSun(int cost)
    {
        sunCount -= cost;
        sunText.text = sunCount.ToString();
        PlayerPrefs.SetInt("TotalSun", sunCount);
        PlayerPrefs.Save();
    }
}

// класс с данными загадки
[Serializable] public class RiddleData
{
    public string riddleText; // текст
    public string answer; // ответ
    public string textDescription; // лексическое значение
    public Sprite imageDescription; // изображение
    public bool levelCompleted = false; // уровень пройден (нет по умолчанию)
}

// перечисление состояний игры
public enum GameStatus
{
    Playing, // играет
    Next // следующий (уровень)
}