using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class HelpManager : MonoBehaviour
{
    public RectTransform screenshotArea;
    public int openLetterCost = 5;
    public int deleteFalseCost = 10;
    public int openWordCost = 15;
    public static bool deleteFalseLettersIsUsed;

    public void OpenLetter()
    {
        var rm = RiddleManager.instance;

        if (rm.GetGameStatus() == GameStatus.Next)
            return;

        if (!rm.HasEnoughSun(openLetterCost))
        {
            Debug.Log("Не хватает солнц");
            return;
        }

        rm.ClearAnswer();
        rm.SpendSun(openLetterCost);

        string answer = rm.GetAnswer();
        var letters = rm.GetLetterCells();
        var words = rm.GetWordCells();

        // выбираем случайную букву
        bool cellIsNull = false;
        int index = 0;
        char correctChar = ' ';
        while (!cellIsNull)
        {
            index = UnityEngine.Random.Range(0, answer.Length);
            correctChar = char.ToUpper(answer[index]);
            if (words[index].cellValue == ' ')
                cellIsNull = true;
        }

        // ставим букву в слово
        words[index].SetCell(correctChar);
        words[index].cellTrueValue = true;

        // находим и скрываем эту букву из списка
        for (int i = 0; i < letters.Length; i++)
        {
            if (letters[i].cellValue == correctChar && letters[i].gameObject.activeSelf)
            {
                letters[i].gameObject.SetActive(false);
                letters[i].cellTrueValue = true;
                break;
            }
        }

        rm.SetCurrentAnswerIndex();
        rm.CheckAnswer();
        gameObject.SetActive(false);
    }

    public void DeleteFalseLetters()
    {
        if (deleteFalseLettersIsUsed)
            return;

        var rm = RiddleManager.instance;

        if (!rm.HasEnoughSun(deleteFalseCost))
        {
            Debug.Log("Не хватает солнц");
            return;
        }

        rm.ClearAnswer();
        rm.SpendSun(deleteFalseCost);

        string answer = rm.GetAnswer();
        var letters = rm.GetLetterCells();

        var correctLetters = answer.ToUpper().ToList();

        foreach (var cell in letters)
        {
            if (correctLetters.Contains(cell.cellValue))
            {
                correctLetters.Remove(cell.cellValue);
            }
            else
            {
                cell.gameObject.SetActive(false);
            }
        }

        deleteFalseLettersIsUsed = true;
        gameObject.SetActive(false);
    }

    public void OpenWord()
    {
        var rm = RiddleManager.instance;

        if (!rm.HasEnoughSun(openWordCost))
        {
            Debug.Log("Не хватает солнц");
            return;
        }

        rm.ClearAnswer();
        rm.SpendSun(openWordCost);

        var answer = rm.GetAnswer().ToCharArray();
        var words = rm.GetWordCells();  

        List<int> correctWords = new();
        List<CellData> lettersList = rm.GetLetterCells().ToList();        

        for (int i = 0; i < answer.Length; i++)
        {
            if (words[i].cellValue == ' ')
            {
                correctWords.Add(i);
            }
        }

        foreach (var index in correctWords)
        {
            foreach (var letters in lettersList)
            {
                if (char.ToUpper(answer[index]) == letters.cellValue && letters.gameObject.activeSelf)
                {
                    rm.SelectedLetter(letters);
                    break;
                }
            }
        }

        gameObject.SetActive(false);
    }

    public void AskPeople()
    {
        var rm = RiddleManager.instance;
        rm.ClearAnswer();
        StartCoroutine(TakeScreenshotAndShare());
    }

    private System.Collections.IEnumerator TakeScreenshotAndShare()
    {
        // 1. Скрываем панель подсказок
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;

        // Ждем отрисовки кадра без панели
        yield return new WaitForEndOfFrame();

        // 2. Рассчитываем координаты области на экране
        Vector3[] corners = new Vector3[4];
        screenshotArea.GetWorldCorners(corners);

        // corners[0] - левый нижний угол, corners[2] - правый верхний
        int startX = (int)corners[0].x;
        int startY = (int)corners[0].y;
        int width = (int)corners[2].x - (int)corners[0].x;
        int height = (int)corners[2].y - (int)corners[0].y;

        // 3. Делаем захват пикселей
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
        screenShot.Apply();

        // 4. Сохраняем во временную папку
        byte[] bytes = screenShot.EncodeToPNG();
        string filePath = Path.Combine(Application.temporaryCachePath, "temp_riddle.png");
        File.WriteAllBytes(filePath, bytes);

        // Очищаем память
        Destroy(screenShot);

        // 5. Возвращаем панель обратно
        cg.alpha = 1;

        // 6. Отправляем через NativeShare
        new NativeShare()
            .AddFile(filePath)
            .SetText("Бу таабырыны таайарга көмөлөһүүй")
            .SetSubject("Көмө наада") // Для email
            .Share();
    }
}