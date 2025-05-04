using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LineNumber : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField lineNumber;
    [SerializeField] private RectTransform lineNumbersRect;
    [SerializeField] private RectTransform codeRect;

    private void Start()
    {
        inputField.onValueChanged.AddListener(UpdateLineNumbers);
        UpdateLineNumbers(inputField.text);
    }

    private void Update()
    {
         if(lineNumbersRect.anchoredPosition.y != codeRect.anchoredPosition.y){
            Vector2 pos = lineNumbersRect.anchoredPosition;
            pos.y = codeRect.anchoredPosition.y; 
            lineNumbersRect.anchoredPosition = pos;
        } 
    }



    private void UpdateLineNumbers(string text)
    {
        int lineCount = text.Split('\n').Length;;
        lineNumber.text = "";

        for (int i = 1; i <= lineCount; i++)
        {
            lineNumber.text += i + "\n";
        }
    }
}