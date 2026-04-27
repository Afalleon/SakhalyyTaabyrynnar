using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource source;
    public AudioClip clickSound;
    public AudioClip trueAnswerSound;
    public AudioClip falseAnswerSound;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else 
            Destroy(gameObject);
    }

    public void PlayClick()
    {
        if (PlayerPrefs.GetInt("SoundEnabled", 1) == 1)
        {
            source.PlayOneShot(clickSound);
        }
    }

    public void PlayTrueAnswer()
    {
        if (PlayerPrefs.GetInt("SoundEnabled", 1) == 1)
        {
            source.PlayOneShot(trueAnswerSound);
        }
    }

    public void PlayFalseAnswer()
    {
        if (PlayerPrefs.GetInt("SoundEnabled", 1) == 1)
        {
            source.PlayOneShot(falseAnswerSound);
        }
    }
}