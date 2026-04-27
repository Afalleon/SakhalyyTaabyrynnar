using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] private RiddleDataScriptable data;
    public GameObject bonusPanel;
    public GameObject[] allPanels;
    private GameInput _input;

    private void Awake()
    {
        _input = new GameInput();
        _input.UI.Back.performed += ctx => HandleBackPress();
    }

    private void OnEnable() => _input.Enable();
    private void OnDisable() => _input.Disable();

    private void Start()
    {
        Application.targetFrameRate = 120;
        for (int i = 0; i < PlayerPrefs.GetInt("CurrentLevel", 0); i++)
        {
            data.riddles[i].levelCompleted = true;
        }

        RiddleManager.sunCount = PlayerPrefs.GetInt("TotalSun", 764);
    }

    void HandleBackPress()
    {
        // 1. Если открыта панель подсказок — сначала закрываем её
        if (bonusPanel != null && bonusPanel.activeSelf)
        {
            AudioManager.instance.PlayClick();
            DailyBonus.ClaimBonus();
            return; // Выходим из метода, чтобы не выйти в меню сразу
        }

        if (allPanels != null)
        {
            foreach (GameObject panel in allPanels)
            {
                if (panel != null && panel.activeSelf)
                {
                    AudioManager.instance.PlayClick();
                    panel.SetActive(false);
                    return;
                }
            }
        }

        AudioManager.instance.PlayClick();
        btnBack_Click();
    }

    public void btnStart_Click()
    {
        for (int i = 0; i < data.riddles.Count; i++)
            if (i == 0 || data.riddles[i].levelCompleted || (i > 0 && data.riddles[i - 1].levelCompleted))
                RiddleManager.currentRiddleIndex = i;
        SceneManager.LoadScene(1);
    }

    public void btnBack_Click()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        else
            Application.Quit();
    }

    public void btnLevels_Click()
    {
        SceneManager.LoadScene(2);
    }
}