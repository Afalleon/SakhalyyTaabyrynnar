using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

public class RiddleManager : MonoBehaviour
{
    public static RiddleManager instance;

    [SerializeField] GameObject gameComplete;
    [SerializeField] GameObject levelComplete;

    [SerializeField] private RiddleDataScriptable data;
    [SerializeField] private Text riddleText;
    [SerializeField] private Text sunText;
    [SerializeField] private Text addSunText;
    [SerializeField] private Image riddleImage;
    [SerializeField] private CellData[] wordCellList;
    [SerializeField] private CellData[] letterCellList;

    private GameStatus gameStatus = GameStatus.Playing;
    private char[] lettersArray = new char[16];

    private List<int> selectedLettersIndex;
    public static int currentAnswerIndex, currentRiddleIndex, sunCount;
    private bool correctAnswer = true;
    private string answerText;

    [SerializeField] private char[] yakutABC;
    [SerializeField] private Text levelID;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }            
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        selectedLettersIndex = new List<int>();
        if (currentRiddleIndex == data.riddles.Count)
            currentRiddleIndex = currentRiddleIndex - 1;
        SetRiddle(currentRiddleIndex);
    }

    public void SetNextRiddle()
    {
        levelComplete.SetActive(false);

        if (currentRiddleIndex == data.riddles.Count)
        {
            sunText.text = sunCount.ToString();
            currentRiddleIndex = currentRiddleIndex - 1;
            gameComplete.SetActive(true);
            return;
        }

        SetRiddle(currentRiddleIndex);
    }

    public void SetRiddle(int index)
    {
        sunText.text = sunCount.ToString();
        gameStatus = GameStatus.Playing;
        answerText = data.riddles[index].answer;
        riddleText.text = data.riddles[index].riddleText;
        levelID.text = (index + 1).ToString();
        HelpManager.deleteFalseLettersIsUsed = false;

        ResetRiddle();

        selectedLettersIndex.Clear();
        Array.Clear(lettersArray, 0, lettersArray.Length);

        for (int i = 0; i < answerText.Length; i++)
        {
            lettersArray[i] = char.ToUpper(answerText[i]);
        }

        for (int j = answerText.Length; j < lettersArray.Length; j++)
        {
            lettersArray[j] = yakutABC[UnityEngine.Random.Range(0, yakutABC.Length)];
        }

        lettersArray = ShuffleList.ShuffleListItems<char>(lettersArray.ToList()).ToArray();

        for (int k = 0;  k < letterCellList.Length; k++)
        {
            letterCellList[k].SetCell(lettersArray[k]);
        }
    }

    public void ResetRiddle()
    {
        for (int i = 0; i < wordCellList.Length; i++)
        {
            wordCellList[i].gameObject.SetActive(true);
            wordCellList[i].SetCell(' ');
        }
        for (int i = answerText.Length; i < wordCellList.Length; i++)
        {
            wordCellList[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < letterCellList.Length; i++)
        {
            letterCellList[i].gameObject.SetActive(true);
        }
        selectedLettersIndex.Clear();
        currentAnswerIndex = 0;
    }

    public void SelectedLetter(CellData value)
    {
        if (gameStatus == GameStatus.Next || wordCellList[currentAnswerIndex].cellValue != ' ') return;

        selectedLettersIndex.Add(value.transform.GetSiblingIndex());
        value.gameObject.SetActive(false);
        wordCellList[currentAnswerIndex].SetCell(value.cellValue);

        SetCurrentAnswerIndex();
        CheckAnswer();
    }

    public void ResetLastCell()
    {
        if (selectedLettersIndex.Count > 0)
        {
            for (int i = selectedLettersIndex.Count - 1; i >= 0; i--)
            {
                int index = selectedLettersIndex[i];
                if (!letterCellList[index].cellTrueValue)
                {
                    letterCellList[index].gameObject.SetActive(true);
                    selectedLettersIndex.Remove(index);
                    for (int j = wordCellList.Length - 1; j >= 0; j--)
                    {
                        if (wordCellList[j].cellValue != ' ' && !wordCellList[j].cellTrueValue)
                        {
                            wordCellList[j].SetCell(' ');
                            break;
                        }                   
                    }
                    SetCurrentAnswerIndex();
                    break;
                }
            }                           
        }
    }

    public void CompleteRiddle()
    {
        levelComplete.SetActive(true);

        int currentCompleteRiddleIndex = currentRiddleIndex - 1;

        riddleText.text = data.riddles[currentCompleteRiddleIndex].textDescription;
        riddleImage.sprite = data.riddles[currentCompleteRiddleIndex].imageDescription;

        if (data.riddles[currentCompleteRiddleIndex].levelCompleted == false)
        {
            int addSunCount = data.riddles[currentCompleteRiddleIndex].answer.Length;
            addSunText.text = $"+{addSunCount.ToString()}";
            sunCount += addSunCount;
        }
        data.riddles[currentCompleteRiddleIndex].levelCompleted = true;
                
        PlayerPrefs.SetInt("CurrentLevel", currentRiddleIndex);
        PlayerPrefs.SetInt("TotalSun", sunCount);
        PlayerPrefs.Save();
    }

    // новое
    public void CheckAnswer()
    {
        if (wordCellList[currentAnswerIndex].cellValue != ' ')
        {
            correctAnswer = true;
            for (int i = 0; i < answerText.Length; i++)
            {
                if (char.ToUpper(answerText[i]) != char.ToUpper(wordCellList[i].cellValue))
                {
                    correctAnswer = false;
                    break;
                }
            }

            if (correctAnswer)
            {
                AudioManager.instance.PlayTrueAnswer();
                Debug.Log("Correct Answer");
                if (PlayerPrefs.GetInt("VibrationEnabled", 1) == 1)
                    Handheld.Vibrate();
                gameStatus = GameStatus.Next;
                currentRiddleIndex++;

                CompleteRiddle();             
            }
            else
                AudioManager.instance.PlayFalseAnswer();
        }
    }
    public void SetCurrentAnswerIndex()
    {
        for (int i = 0; i < answerText.Length; i++)
        {
            if (wordCellList[i].cellValue == ' ')
            {
                currentAnswerIndex = i;
                break;
            }
        }
    }

    public void ClearAnswer()
    {
        // возвращаем все выбранные буквы обратно
        if (selectedLettersIndex.Count > 0)
        {
            foreach (int index in new List<int>(selectedLettersIndex))
            {
                if (!letterCellList[index].cellTrueValue)
                {
                    letterCellList[index].gameObject.SetActive(true);
                    selectedLettersIndex.Remove(index);
                }
            }

            for (int i = 0; i < wordCellList.Length; i++)
            {
                if (!wordCellList[i].cellTrueValue)
                    wordCellList[i].SetCell(' ');
            }

            SetCurrentAnswerIndex();
        }            
    }

    public GameStatus GetGameStatus()
    {
        return gameStatus;
    }

    public string GetAnswer()
    {
        return answerText;
    }

    public CellData[] GetLetterCells()
    {
        return letterCellList;
    }

    public CellData[] GetWordCells()
    {
        return wordCellList;
    }

    public bool HasEnoughSun(int cost)
    {
        return sunCount >= cost;
    }

    public void SpendSun(int cost)
    {
        sunCount -= cost;
        sunText.text = sunCount.ToString();
        PlayerPrefs.SetInt("TotalSun", sunCount);
        PlayerPrefs.Save();
    }
}

[Serializable]
public class RiddleData
{
    public string riddleText;
    public string answer;
    public string textDescription;
    public Sprite imageDescription;
    public bool levelCompleted = false;
}

public enum GameStatus
{
    Next,
    Playing
}