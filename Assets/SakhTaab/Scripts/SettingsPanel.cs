using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    // ссылки на объекты в инспекторе
    [SerializeField] private RiddleDataScriptable data; // ScriptableObject с загадками
    [SerializeField] private Button soundButton; // кнопка звука
    [SerializeField] private Button vibrationButton; // кнопка вибрации

    private Color onColor = Color.white; // цвет активной кнопки
    private Color offColor = new Color(0.63f, 0.63f, 0.63f); // цвет неактивной кнопки

    private void Start()
    {
        // при запуске устанавливаем цвета кнопок в зависимости от сохранений
        UpdateButtonVisuals("SoundEnabled", soundButton);
        UpdateButtonVisuals("VibrationEnabled", vibrationButton);
    }

    public void TurnOnOffSound() => ToggleSetting("SoundEnabled", soundButton); // переключатель звука
    public void TurnOnOffVibration() => ToggleSetting("VibrationEnabled", vibrationButton); // переключатель вибрации

    // метод открытия документации игры
    public void OpenDocuments()
    {
        string url = "https://docs.google.com/document/d/e/2PACX-1vTgSOVuBGN9a2rTd_w323aemjGwfpULaT10gm5KGrqNw4xxUea5hil9_19HLvz-STyeo9zXyxcRBY98/pub";
        Application.OpenURL(url); // открываем ссылку
    }

    // метод отправки письма разработчику
    public void SendEmail()
    {
        string email = "afalleon@gmail.com"; // почта
        string subject = "Оонньуу туһунан"; // тема письма
        string body = "Дорообо, 'Сахалыы таабырыннар (Якутские загадки)' оонньууттан суруйабын..."; // текст письма

        // кодируем спецсимволы (пробелы и т.д.), чтобы ссылка была корректной
        string mailto = string.Format("mailto:{0}?subject={1}&body={2}",
            email,
            Uri.EscapeDataString(subject),
            Uri.EscapeDataString(body));

        Application.OpenURL(mailto); // открываем ссылку
    }

    // метод очистки прогресса
    public void ClearProgress()
    {
        // для всех загадок
        for (int i = 0; i < data.riddles.Count; i++)
        {
            data.riddles[i].levelCompleted = false; // устанавливаем значение Не пройдено в ScriptableObject
        }

        // устанавливаем данные по умолчанию и сохраняем
        PlayerPrefs.SetInt("CurrentLevel", 0); // текущий уровень
        PlayerPrefs.SetInt("TotalSun", 764); // количество валюты
        PlayerPrefs.SetString("LastBonusDate", DateTime.MinValue.ToString()); // дата последнего захода в игру
        PlayerPrefs.Save(); // сохранение
    }

    // переключатель звука/вибрации
    private void ToggleSetting(string key, Button button)
    {
        // читаем текущее состояние (по умолчанию 1) и инвертируем
        int state = PlayerPrefs.GetInt(key, 1);
        int newState = (state == 1) ? 0 : 1;

        // сохраняем
        PlayerPrefs.SetInt(key, newState);
        PlayerPrefs.Save();
        
        // устанавливаем цвет кнопки
        UpdateButtonVisuals(key, button);

        // если включили вибрацию - даем тестовый отклик
        if (key == "VibrationEnabled" && newState == 1) Handheld.Vibrate();
    }

    // установщик цветов кнопок
    private void UpdateButtonVisuals(string key, Button button)
    {
        if (button == null) return;
        int state = PlayerPrefs.GetInt(key, 1); // получаем состояние кнопки (вкл/выкл)
        button.image.color = (state == 1) ? onColor : offColor; // устанавливаем цвет
    }
}