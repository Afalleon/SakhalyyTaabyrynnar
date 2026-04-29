using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HelpManager : MonoBehaviour
{
    private int openLetterCost = 5; // стоимость подсказки Открыть букву
    private int deleteFalseCost = 10; // стоимость подсказки Удалить лишние
    private int openWordCost = 15; // стоимость подсказки Открыть слово
    [SerializeField] private RectTransform screenshotArea; // область скриншота для подсказки Спросить у людей
    public static bool deleteFalseLettersIsUsed; // использована ли подсказка Удалить лишние

    // метод Открыть букву
    public void OpenLetter()
    {
        var rm = RiddleManager.instance; // получаем экземпляр класса RiddleManager

        // если статус игры Next (введен правильный ответ), ничего не делаем
        if (rm.GetGameStatus() == GameStatus.Next)
            return;

        // если недостаточно валюты, ничего не делаем
        if (!rm.HasEnoughSun(openLetterCost))
        {
            Debug.Log("Не хватает солнц");
            return;
        }

        rm.ClearAnswer(); // очищаем ячейки для ввода
        rm.SpendSun(openLetterCost); // списываем валюту

        string answer = rm.GetAnswer(); // получаем слово ответа
        var words = rm.GetWordCells(); // получаем ячейки для ответа
        var letters = rm.GetLetterCells(); // получаем ячейки с буквами        

        // выбираем случайную букву
        bool cellIsNull = false;
        int index = 0;
        char correctChar = ' ';
        while (!cellIsNull)
        {
            index = Random.Range(0, answer.Length);
            correctChar = char.ToUpper(answer[index]);
            if (words[index].cellValue == ' ')
                cellIsNull = true;
        }

        // ставим букву в слово
        words[index].SetCell(correctChar);
        words[index].cellTrueValue = true; // указываем, что это правильная буква

        // находим и скрываем эту букву из списка ячеек с буквами
        for (int i = 0; i < letters.Length; i++)
        {
            if (letters[i].cellValue == correctChar && letters[i].gameObject.activeSelf)
            {
                letters[i].gameObject.SetActive(false);
                letters[i].cellTrueValue = true; // указываем, что это ячейка с правильной буквой
                break;
            }
        }

        rm.SetCurrentAnswerIndex(); // устанавливаем индекс текущей буквы для ответа
        rm.CheckAnswer(); // проверяем ответ
        gameObject.SetActive(false); // скрываем панель подсказок
    }

    // метод Удалить лишние
    public void DeleteFalseLetters()
    {
        // если подсказка уже использована в текущем уровне, ничего не делаем
        if (deleteFalseLettersIsUsed)
            return;

        var rm = RiddleManager.instance; // получаем экземпляр класса RiddleManager

        // если недостаточно валюты, ничего не делаем
        if (!rm.HasEnoughSun(deleteFalseCost))
        {
            Debug.Log("Не хватает солнц");
            return;
        }

        rm.ClearAnswer(); // очищаем ячейки для ввода
        rm.SpendSun(deleteFalseCost); // списываем валюту

        string answer = rm.GetAnswer(); // получаем слово ответа
        var letters = rm.GetLetterCells(); // получаем ячейки с буквами

        var correctLetters = answer.ToUpper().ToList(); // делаем список с правильными буквами

        // проходимся по всем ячейкам с буквами
        foreach (var cell in letters)
        {
            // если ячейка с правильной буквой - оставляем
            if (correctLetters.Contains(cell.cellValue))
            {
                correctLetters.Remove(cell.cellValue); // из списка удаляем букву, чтобы не было повторов
            }
            // если нет - скрываем
            else
            {
                cell.gameObject.SetActive(false);
            }
        }

        deleteFalseLettersIsUsed = true; // отмечаем, что подсказка использована, чтобы не было повторной покупки
        gameObject.SetActive(false); // скрываем панель подсказок
    }

    // метод Открыть слово
    public void OpenWord()
    {
        var rm = RiddleManager.instance; // получаем экземпляр класса RiddleManager

        // если недостаточно валюты, ничего не делаем
        if (!rm.HasEnoughSun(openWordCost))
        {
            Debug.Log("Не хватает солнц");
            return;
        }

        rm.ClearAnswer(); // очищаем ячейки для ввода
        rm.SpendSun(openWordCost); // списываем валюту

        var answer = rm.GetAnswer().ToCharArray(); // получаем слово ответа
        var words = rm.GetWordCells(); // получаем ячейки для ответа

        var nullWords = new List<int>(); // список пустых ячеек для ввода
        var lettersList = rm.GetLetterCells().ToList(); // список ячеек с буквами

        // добавляем пустые ячейки в список
        for (int i = 0; i < answer.Length; i++)
        {
            if (words[i].cellValue == ' ')
            {
                nullWords.Add(i);
            }
        }

        // вставляем правильные буквы в ячейки
        foreach (var index in nullWords)
        {
            foreach (var letters in lettersList)
            {
                if (char.ToUpper(answer[index]) == letters.cellValue && letters.gameObject.activeSelf)
                {
                    rm.SelectedLetter(letters); // вызываем метод нажатия ячейки с буквой
                    break;
                }
            }
        }

        gameObject.SetActive(false); // скрываем панель подсказок
    }

    // метод Спросить у людей
    public void AskPeople()
    {
        var rm = RiddleManager.instance; // получаем экземпляр класса RiddleManager
        rm.ClearAnswer(); // очищаем ячейки для ввода
        StartCoroutine(TakeScreenshotAndShare()); // вызываем корутиновый метод
    }

    // метод создания скриншота и открытия окна Поделиться
    private IEnumerator TakeScreenshotAndShare()
    {
        // скрываем панель подсказок
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;

        // ждем отрисовки кадра без панели
        yield return new WaitForEndOfFrame();

        // рассчитываем координаты области на экране
        Vector3[] corners = new Vector3[4];
        screenshotArea.GetWorldCorners(corners);

        // corners[0] - левый нижний угол, corners[2] - правый верхний
        int startX = (int)corners[0].x;
        int startY = (int)corners[0].y;
        int width = (int)corners[2].x - (int)corners[0].x;
        int height = (int)corners[2].y - (int)corners[0].y;

        // делаем захват пикселей
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
        screenShot.Apply();

        // сохраняем во временную папку
        byte[] bytes = screenShot.EncodeToPNG();
        string filePath = Path.Combine(Application.temporaryCachePath, "temp_riddle.png");
        File.WriteAllBytes(filePath, bytes);

        // очищаем память
        Destroy(screenShot);

        // возвращаем панель обратно
        cg.alpha = 1;

        // отправляем через NativeShare
        new NativeShare()
            .AddFile(filePath) // файл скриншота
            .SetText("Бу таабырыны таайарга көмөлөһүүй") // текст
            .SetSubject("Көмө наада") // тема письма (для email)
            .Share();
    }
}