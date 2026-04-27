using UnityEngine;
using System;
using UnityEngine.UI;

public class DailyBonus : MonoBehaviour
{
    public static DailyBonus instance;

    public GameObject bonusPanel;  // Ссылка на панель бонуса
    public Text bonusText;
    public int rewardAmount = 25;  // Сколько даем монет/солнц

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        CheckBonus();
    }

    public void CheckBonus()
    {
        // 1. Получаем дату последнего захода (если её нет, берем очень старую дату)
        string lastDateString = PlayerPrefs.GetString("LastBonusDate", DateTime.MinValue.ToString());
        DateTime lastDate = DateTime.Parse(lastDateString);

        // 2. Сравниваем с текущей датой
        if (DateTime.Now.Date > lastDate.Date)
        {
            // Бонус доступен!
            bonusPanel.SetActive(true);
            bonusText.text = $"+{rewardAmount}";
            Debug.Log("Бонус готов к получению!");
        }
        else
        {
            // Бонус уже был получен сегодня
            bonusPanel.SetActive(false);
            Debug.Log("Бонус уже забран!");
        }
    }

    public static void ClaimBonus()
    {
        // 1. Сохраняем текущую дату как дату последнего получения
        PlayerPrefs.SetString("LastBonusDate", DateTime.Now.ToString());
        PlayerPrefs.Save();

        // 2. Начисляем награду
        int currentSuns = PlayerPrefs.GetInt("TotalSun", 764);
        PlayerPrefs.SetInt("TotalSun", currentSuns + instance.rewardAmount);
        PlayerPrefs.Save();
        Debug.Log($"Вы получили {instance.rewardAmount} солнц!");

        // 3. Делаем панель неактивной
        instance.bonusPanel.SetActive(false);
    }
}