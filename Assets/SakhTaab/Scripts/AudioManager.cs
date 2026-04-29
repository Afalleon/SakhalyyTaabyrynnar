using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // экземпляр класса

    // ссылки на объекты в инспекторе
    [SerializeField] private AudioSource source; // источник звука
    [SerializeField] private AudioClip clickSound; // звук клика
    [SerializeField] private AudioClip trueAnswerSound; // звук правильного ответа
    [SerializeField] private AudioClip falseAnswerSound; // звук неправильного ответа

    // реализация паттерна Singleton: гарантируем, что в игре только один менеджер звука
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
            Destroy(gameObject);
    }

    // метод воспроизведения звука клика
    public void PlayClick()
    {
        if (PlayerPrefs.GetInt("SoundEnabled", 1) == 1) // проверяем, включен ли звук в настройках
        {
            source.PlayOneShot(clickSound); // проигрываем, если включен
        }
    }

    // метод для звука правильного ответа
    public void PlayTrueAnswer()
    {
        if (PlayerPrefs.GetInt("SoundEnabled", 1) == 1)
        {
            source.PlayOneShot(trueAnswerSound);
        }
    }

    // метод для звука неправильного ответа
    public void PlayFalseAnswer()
    {
        if (PlayerPrefs.GetInt("SoundEnabled", 1) == 1)
        {
            source.PlayOneShot(falseAnswerSound);
        }
    }
}