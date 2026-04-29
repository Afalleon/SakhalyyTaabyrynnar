using UnityEngine;
using UnityEngine.UI;

public class CellData : MonoBehaviour
{
    [SerializeField] private Text cellText; // ссылка на компонент Text для значения ячейки
    [HideInInspector] public char cellValue; // значение ячейки    
    [HideInInspector] public bool cellTrueValue = false; // по умолчанию ячейка не помечена как правильная

    private void Awake()
    {
        Button buttonComponent = GetComponent<Button>(); // получаем компонент Button
        if (buttonComponent)
        {
            buttonComponent.onClick.AddListener(() => CellSelected()); // клик - метод нажатия ячейки
            buttonComponent.onClick.AddListener(() => AudioManager.instance.PlayClick()); // клик - звук клика
        } 
    }

    // метод установки значения ячейки
    public void SetCell(char value)
    {
        cellText.text = value + "";
        cellValue = value;
        cellTrueValue = false;
    }

    // метод нажатия ячейки
    private void CellSelected()
    {
        RiddleManager.instance.SelectedLetter(this); // вызываем метод нажатия ячейки с буквой
    }
}