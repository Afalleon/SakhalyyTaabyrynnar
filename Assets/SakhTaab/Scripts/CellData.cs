using UnityEngine;
using UnityEngine.UI;

public class CellData : MonoBehaviour
{
    [SerializeField] private Text cellText;
    [HideInInspector] public char cellValue;
    private Button buttonComponent;
    [HideInInspector] public bool cellTrueValue = false;

    private void Awake()
    {
        buttonComponent = GetComponent<Button>();
        if (buttonComponent)
        {
            buttonComponent.onClick.AddListener(() => CellSelected());
            buttonComponent.onClick.AddListener(() => AudioManager.instance.PlayClick());
        } 
    }

    public void SetCell(char value)
    {
        cellText.text = value + "";
        cellValue = value;
        cellTrueValue = false;
    }

    private void CellSelected()
    {
        RiddleManager.instance.SelectedLetter(this);
    }
}