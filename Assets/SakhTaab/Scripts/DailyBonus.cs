using UnityEngine;
using System;
using UnityEngine.UI;

public class DailyBonus : MonoBehaviour
{
    public static DailyBonus instance; // экземпл€р класса

    // ссылки на объекты в инспекторе
    [SerializeField] private GameObject bonusPanel; // панель бонуса
    [SerializeField] private Text bonusText; // компонент Text
    private int rewardAmount = 25; // количество прибавл€емой награды

    // реализаци€ паттерна Singleton
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
        CheckBonus(); // вызываем метод проверки доступности бонуса
    }

    // метод проверки доступности бонуса
    private void CheckBonus()
    {
        // получаем дату последнего захода (если еЄ нет, берем очень старую дату)
        string lastDateString = PlayerPrefs.GetString("LastBonusDate", DateTime.MinValue.ToString());
        DateTime lastDate = DateTime.Parse(lastDateString);

        // сравниваем с текущей датой
        if (DateTime.Now.Date > lastDate.Date)
        {
            // бонус доступен
            bonusPanel.SetActive(true); // активируем панель бонуса
            bonusText.text = $"+{rewardAmount}"; // выводим в текст количество прибавл€емой награды
            Debug.Log("Ѕонус готов к получению!");
        }
        else
        {
            // бонус уже был получен сегодн€
            bonusPanel.SetActive(false);
            Debug.Log("Ѕонус уже забран!");
        }
    }

    // метод получени€ бонуса
    public static void ClaimBonus()
    {
        // сохран€ем текущую дату как дату последнего получени€
        PlayerPrefs.SetString("LastBonusDate", DateTime.Now.ToString());
        PlayerPrefs.Save();

        // начисл€ем награду
        int currentSuns = PlayerPrefs.GetInt("TotalSun", 764);
        PlayerPrefs.SetInt("TotalSun", currentSuns + instance.rewardAmount);
        PlayerPrefs.Save();
        Debug.Log($"¬ы получили {instance.rewardAmount} солнц!");

        // делаем панель неактивной
        instance.bonusPanel.SetActive(false);
    }
}