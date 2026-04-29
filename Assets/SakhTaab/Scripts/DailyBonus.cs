using UnityEngine;
using System;
using UnityEngine.UI;

public class DailyBonus : MonoBehaviour
{
    public static DailyBonus instance; // экземпляр класса

    // ссылки на объекты в инспекторе
    [SerializeField] private GameObject bonusPanel; // панель бонуса
    [SerializeField] private Text bonusText; // компонент Text
    private int rewardAmount = 25; // количество прибавляемой награды

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

    private void Start()
    {
        CheckBonus(); // вызываем метод проверки доступности бонуса
    }

    // метод проверки доступности бонуса
    private void CheckBonus()
    {
        // получаем дату последнего захода (если её нет, берем очень старую дату)
        string lastDateString = PlayerPrefs.GetString("LastBonusDate", DateTime.MinValue.ToString());
        DateTime lastDate = DateTime.Parse(lastDateString);

        // сравниваем с текущей датой
        if (DateTime.Now.Date > lastDate.Date)
        {
            // бонус доступен
            bonusPanel.SetActive(true); // активируем панель бонуса
            bonusText.text = $"+{rewardAmount}"; // выводим в текст количество прибавляемой награды
            Debug.Log("Бонус готов к получению!");
        }
        else
        {
            // бонус уже был получен сегодня
            bonusPanel.SetActive(false);
            Debug.Log("Бонус уже забран!");
        }
    }

    // метод получения бонуса
    public static void ClaimBonus()
    {
        // сохраняем текущую дату как дату последнего получения
        PlayerPrefs.SetString("LastBonusDate", DateTime.Now.ToString());
        PlayerPrefs.Save();

        // начисляем награду
        int currentSuns = PlayerPrefs.GetInt("TotalSun", 764);
        PlayerPrefs.SetInt("TotalSun", currentSuns + instance.rewardAmount);
        PlayerPrefs.Save();
        Debug.Log($"Вы получили {instance.rewardAmount} солнц!");

        // делаем панель неактивной
        instance.bonusPanel.SetActive(false);
    }
}