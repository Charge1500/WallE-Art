using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Interprete;
public class InputFieldLineParser : MonoBehaviour
{

    [Header("Componente UI")]
    public TMP_InputField inputField;

    [Header("Configuración de Colores para Resaltado")]
    public Color keywordColor = new Color(0.3f, 0.5f, 1f);         // Azul claro
    public Color functionKeywordColor = new Color(0.2f, 0.7f, 0.7f); // Cian
    public Color numberLiteralColor = new Color(1f, 0.6f, 0.2f);     // Naranja
    public Color stringLiteralColor = new Color(0.6f, 0.8f, 0.3f);     // Verde lima
    public Color identifierColor = new Color(0.9f, 0.9f, 0.9f);      // Casi blanco
    public Color operatorColor = new Color(1f, 0.7f, 0.7f);        // Rosa claro
    public Color punctuationColor = new Color(0.7f, 0.7f, 0.7f);     // Gris
    public Color unknownTokenColor = Color.red;
    private Lexer lexer;
    private bool isUpdatingText = false; 
    private int lastCaretPosition = -1;
    private string lastProcessedTextState = "";

    public struct LineParseInfo
    {
        
        public int LineNumber;

        public string LineContent;

        public int OriginalLineStartIndex;

        public int OriginalLineEndIndex;

        public LineParseInfo(int lineNumber, string lineContent, int startIndex, int endIndex)
        {
            LineNumber = lineNumber;
            LineContent = lineContent;
            OriginalLineStartIndex = startIndex;
            OriginalLineEndIndex = endIndex;
        }
    }

    void Start()
    {
        lexer = new Lexer();

        inputField.onValueChanged.AddListener(HandleInputChanged);

        lastCaretPosition = inputField.caretPosition;
        lastProcessedTextState = GetCleanText(inputField.text); 

        
        //if (!string.IsNullOrEmpty(lastProcessedTextState))ProcessAllText();
    }

    private void HandleInputChanged(string newText)
    {
        ProcessCurrentLine();
        lastProcessedTextState = newText; 
        lastCaretPosition = inputField.caretPosition;
    }

    public LineParseInfo GetCurrentCaretLineInfo(string fullText)
    {
        int caretPosition = inputField.caretPosition;

        caretPosition = Mathf.Clamp(caretPosition, 0, fullText.Length);

        if (string.IsNullOrEmpty(fullText))
        {
            return new LineParseInfo(1, "", 0, 0);
        }

        int lineStartIndex = 0;
    
        if (caretPosition > 0)
        {
            int prevNewlineIndex = fullText.LastIndexOf('\n', caretPosition - 1);
            if (prevNewlineIndex != -1) lineStartIndex = prevNewlineIndex + 1;
        }

        int lineEndIndex = fullText.Length;
    
        int nextNewlineIndex = fullText.IndexOf('\n', caretPosition);
        if (nextNewlineIndex != -1) lineEndIndex = nextNewlineIndex;
        
        string currentLineText = "";
        if (lineStartIndex <= lineEndIndex) currentLineText = fullText.Substring(lineStartIndex, lineEndIndex - lineStartIndex);
        

        int lineNumber = 1;
        for (int i = 0; i < lineStartIndex; i++)
        {
            if (i < fullText.Length && fullText[i] == '\n') lineNumber++;
            
        }

        return new LineParseInfo(lineNumber, currentLineText, lineStartIndex, lineEndIndex);
    }
    

    public void ProcessCurrentLine()
    {
        if (inputField == null || isUpdatingText) return;

        LineParseInfo lineInfo = GetCurrentCaretLineInfo(inputField.text);
        isUpdatingText = true;
        int originalCaretGlobalPosition = inputField.caretPosition;
        string originalFullRichText = inputField.text;
        string originalFullPlainText = GetCleanText(originalFullRichText);

        
        List<Token> tokens = lexer.Tokenize(lineInfo.LineContent);
        lexer.errors.Clear();

        StringBuilder formattedLineBuilder = new StringBuilder();
        FormatLineWithTokens(tokens, formattedLineBuilder, lineInfo.LineContent);

        //string richTextBeforeLine = GetRichTextSubstring(originalFullRichText, 0, lineInfo.OriginalLineStartIndex);
        string richTextAfterLine = "";

        StringBuilder newFullRichTextBuilder = new StringBuilder();
        //newFullRichTextBuilder.Append(richTextBeforeLine);
        newFullRichTextBuilder.Append(formattedLineBuilder.ToString());

        int plainTextIndexForEndOfCurrentLineContent = lineInfo.OriginalLineStartIndex + lineInfo.LineContent.Length;

        if (plainTextIndexForEndOfCurrentLineContent < originalFullPlainText.Length) {
            if (originalFullPlainText[plainTextIndexForEndOfCurrentLineContent] == '\n') {
                newFullRichTextBuilder.Append('\n'); 
                
                //richTextAfterLine = GetRichTextSubstring(originalFullRichText, plainTextIndexForEndOfCurrentLineContent + 1, originalFullPlainText.Length - (plainTextIndexForEndOfCurrentLineContent + 1) );

            } else {
                 
                //richTextAfterLine = GetRichTextSubstring(originalFullRichText, plainTextIndexForEndOfCurrentLineContent, originalFullPlainText.Length - plainTextIndexForEndOfCurrentLineContent );
            }
            newFullRichTextBuilder.Append(richTextAfterLine);
        }


        inputField.text = newFullRichTextBuilder.ToString();
        //RestoreCaretPosition(originalFullRichText, inputField.text, originalCaretGlobalPosition);

        isUpdatingText = false;
        lastProcessedTextState = GetCleanText(inputField.text);
    }

    private void FormatLineWithTokens(List<Token> tokens, StringBuilder builder, string originalLineContent)
    {
        int currentPositionInLine = 0; 

        foreach (Token token in tokens)
        {
            if (token.Type == TokenType.EndOfFile) continue;

            int tokenStartColumnInLine = token.Column - 1;

            if (tokenStartColumnInLine > currentPositionInLine)
            {
                builder.Append(EscapeRichText(originalLineContent.Substring(currentPositionInLine, tokenStartColumnInLine - currentPositionInLine)));
            }

            string colorHex = "";
            bool useUnderline = false;

            switch (token.Type)
            {
                case TokenType.SpawnKeyword: case TokenType.ColorKeyword: case TokenType.SizeKeyword:
                case TokenType.DrawLineKeyword: case TokenType.DrawCircleKeyword: case TokenType.DrawRectangleKeyword:
                case TokenType.FillKeyword: case TokenType.GoToKeyword:
                    colorHex = ColorUtility.ToHtmlStringRGB(keywordColor);
                    break;

                case TokenType.GetActualXKeyword: case TokenType.GetActualYKeyword: case TokenType.GetCanvasSizeKeyword:
                case TokenType.GetColorCountKeyword: case TokenType.IsBrushColorKeyword: case TokenType.IsBrushSizeKeyword:
                case TokenType.IsCanvasColorKeyword:
                    colorHex = ColorUtility.ToHtmlStringRGB(functionKeywordColor);
                    break;

                case TokenType.NumberLiteral:
                    colorHex = ColorUtility.ToHtmlStringRGB(numberLiteralColor);
                    break;

                case TokenType.StringLiteral:
                    builder.Append($"<color=#{ColorUtility.ToHtmlStringRGB(stringLiteralColor)}>");
                    // El lexer en el string devuelve el valor sin las comillas, así que se añaden de nuevo
                    builder.Append("\"");
                    builder.Append(EscapeRichText(token.Value));
                    builder.Append("\"");
                    builder.Append("</color>");
                    currentPositionInLine = tokenStartColumnInLine + token.Value.Length + 2; // +2 por las comillas
                    continue; // Saltar el coloreado genérico de abajo

                case TokenType.Identifier:
                    colorHex = ColorUtility.ToHtmlStringRGB(identifierColor);
                    break;

                case TokenType.PlusOperator: case TokenType.MinusOperator: case TokenType.MultiplyOperator:
                case TokenType.DivideOperator: case TokenType.PowerOperator: case TokenType.ModuloOperator:
                case TokenType.AssignmentOperator: case TokenType.EqualOperator: case TokenType.NotEqualOperator:
                case TokenType.GreaterOperator: case TokenType.LessOperator: case TokenType.GreaterEqualOperator:
                case TokenType.LessEqualOperator: case TokenType.AndOperator: case TokenType.OrOperator:
                case TokenType.NotOperator:
                    colorHex = ColorUtility.ToHtmlStringRGB(operatorColor);
                    break;

                case TokenType.LeftParen: case TokenType.RightParen: case TokenType.Comma:
                case TokenType.LeftBracket: case TokenType.RightBracket:
                    colorHex = ColorUtility.ToHtmlStringRGB(punctuationColor);
                    break;

                case TokenType.EndOfLine: 
                    builder.Append(EscapeRichText(token.Value));
                    currentPositionInLine = tokenStartColumnInLine + token.Value.Length;
                    continue;


                case TokenType.Unknown:
                    colorHex = ColorUtility.ToHtmlStringRGB(unknownTokenColor);
                    useUnderline = true;
                    break;

                default:
                    colorHex = ColorUtility.ToHtmlStringRGB(identifierColor);
                    break;
            }

            if (!string.IsNullOrEmpty(colorHex)) builder.Append($"<color=#{colorHex}>");
            if (useUnderline) builder.Append("<u>");

            builder.Append(EscapeRichText(token.Value));

            if (useUnderline) builder.Append("</u>");
            if (!string.IsNullOrEmpty(colorHex)) builder.Append("</color>");

            currentPositionInLine = tokenStartColumnInLine + token.Value.Length;
        }

        if (currentPositionInLine < originalLineContent.Length)
        {
            builder.Append(EscapeRichText(originalLineContent.Substring(currentPositionInLine)));
        }
    }
    private string GetCleanText(string richText)
    {
        if (string.IsNullOrEmpty(richText)) return "";
    
        string pattern = @"<color=[^>]*>|</color>|<u>|</u>";
        return Regex.Replace(richText, pattern, "");
    }
    private string EscapeRichText(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return input.Replace("<", "&lt;").Replace(">", "&gt;");
    }
    
}
