using UnityEngine;
using UnityEngine.UI;

public class CellData : MonoBehaviour
{
    [SerializeField] private Text cellText; // ссылка на компонент Text дл€ значени€ €чейки
    [HideInInspector] public char cellValue; // значение €чейки    
    [HideInInspector] public bool cellTrueValue = false; // по умолчанию €чейка не помечена как правильна€

    private void Awake()
    {
        Button buttonComponent = GetComponent<Button>(); // получаем компонент Button
        if (buttonComponent)
        {
            buttonComponent.onClick.AddListener(() => CellSelected()); // клик - метод нажати€ €чейки
            buttonComponent.onClick.AddListener(() => AudioManager.instance.PlayClick()); // клик - звук клика
        } 
    }

    // метод установки значени€ €чейки
    public void SetCell(char value)
    {
        cellText.text = value + "";
        cellValue = value;
        cellTrueValue = false;
    }

    // метод нажати€ €чейки
    private void CellSelected()
    {
        RiddleManager.instance.SelectedLetter(this); // вызываем метод нажати€ €чейки с буквой
    }
}