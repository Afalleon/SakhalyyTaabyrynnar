using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // экземпл€р класса

    // ссылки на объекты в инспекторе
    [SerializeField] private AudioSource source; // источник звука
    [SerializeField] private AudioClip clickSound; // звук клика
    [SerializeField] private AudioClip trueAnswerSound; // звук правильного ответа
    [SerializeField] private AudioClip falseAnswerSound; // звук неправильного ответа

    // реализаци€ паттерна Singleton: гарантируем, что в игре только один менеджер звука
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
            Destroy(gameObject);
    }

    // метод воспроизведени€ звука клика
    public void PlayClick()
    {
        if (PlayerPrefs.GetInt("SoundEnabled", 1) == 1) // провер€ем, включен ли звук в настройках
        {
            source.PlayOneShot(clickSound); // проигрываем, если включен
        }
    }

    // метод дл€ звука правильного ответа
    public void PlayTrueAnswer()
    {
        if (PlayerPrefs.GetInt("SoundEnabled", 1) == 1)
        {
            source.PlayOneShot(trueAnswerSound);
        }
    }

    // метод дл€ звука неправильного ответа
    public void PlayFalseAnswer()
    {
        if (PlayerPrefs.GetInt("SoundEnabled", 1) == 1)
        {
            source.PlayOneShot(falseAnswerSound);
        }
    }
}