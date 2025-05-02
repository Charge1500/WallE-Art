using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineNumber : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text lineNumberText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform lineNumbersRect;

    private void Start()
    {
        inputField.onValueChanged.AddListener(UpdateLineNumbers);
        UpdateLineNumbers(inputField.text);
    }

    private void SyncScroll()
    {
        Vector2 textPos = inputField.textComponent.rectTransform.anchoredPosition;
        lineNumbersRect.anchoredPosition = new Vector2(
            lineNumbersRect.anchoredPosition.x,
            textPos.y
        );
    }



    private void UpdateLineNumbers(string text)
    {
        int lineCount = text.Split('\n').Length;;
        lineNumberText.text = "";

        for (int i = 1; i <= lineCount; i++)
        {
            lineNumberText.text += i + "\n";
        }
    }
}