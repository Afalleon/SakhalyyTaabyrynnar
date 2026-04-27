using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public RiddleDataScriptable data;
    public Button soundButton;
    public Button vibrationButton;

    private Color onColor = Color.white;
    private Color offColor = new Color(0.63f, 0.63f, 0.63f);

    private void Start()
    {
        // При запуске устанавливаем цвета кнопок в зависимости от сохранений
        UpdateButtonVisuals("SoundEnabled", soundButton);
        UpdateButtonVisuals("VibrationEnabled", vibrationButton);
    }

    public void TurnOnOffSound() => ToggleSetting("SoundEnabled", soundButton);
    public void TurnOnOffVibration() => ToggleSetting("VibrationEnabled", vibrationButton);

    public void OpenDocuments()
    {
        Application.OpenURL("https://docs.google.com/document/d/e/2PACX-1vTgSOVuBGN9a2rTd_w323aemjGwfpULaT10gm5KGrqNw4xxUea5hil9_19HLvz-STyeo9zXyxcRBY98/pub");
    }

    public void SendEmail()
    {
        string email = "afalleon@gmail.com"; // Почта
        string subject = "Оонньуу туһунан"; // Тема письма
        string body = "Дорообо, 'Сахалыы таабырыннар (Якутские загадки)' оонньууттан суруйабын..."; // Текст письма

        // Кодируем спецсимволы (пробелы и т.д.), чтобы ссылка была корректной
        string mailto = string.Format("mailto:{0}?subject={1}&body={2}",
            email,
            Uri.EscapeDataString(subject),
            Uri.EscapeDataString(body));

        Application.OpenURL(mailto);
    }

    public void ClearProgress()
    {
        for (int i = 0; i < data.riddles.Count; i++)
        {
            data.riddles[i].levelCompleted = false;
        }

        PlayerPrefs.SetInt("CurrentLevel", 0);
        PlayerPrefs.SetInt("TotalSun", 764);
        PlayerPrefs.SetString("LastBonusDate", DateTime.MinValue.ToString());
        PlayerPrefs.Save();
    }

    private void ToggleSetting(string key, Button button)
    {
        // Читаем текущее состояние (по умолчанию 1) и инвертируем
        int state = PlayerPrefs.GetInt(key, 1);
        int newState = (state == 1) ? 0 : 1;

        PlayerPrefs.SetInt(key, newState);
        PlayerPrefs.Save();

        UpdateButtonVisuals(key, button);

        // Если включили вибрацию - даем тестовый отклик
        if (key == "VibrationEnabled" && newState == 1) Handheld.Vibrate();
    }

    private void UpdateButtonVisuals(string key, Button button)
    {
        if (button == null) return;
        int state = PlayerPrefs.GetInt(key, 1);
        button.image.color = (state == 1) ? onColor : offColor;
    }
}