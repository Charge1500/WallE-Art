using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interprete{
    public class Lexer 
    {
         public List<string> errors = new List<string>();
        private string _sourceCode;
        private int _position;
        private int _currentLine;
        private int _currentColumn;
        private List<Token> _tokens;

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase) // Ignorar mayúsculas/minúsculas? La spec no lo dice, asumamos case-sensitive por ahora. Si no, usar OrdinalIgnoreCase.
        {
            {"Spawn", TokenType.SpawnKeyword},{"Color", TokenType.ColorKeyword},{"Size", TokenType.SizeKeyword},{"DrawLine", TokenType.DrawLineKeyword},
            {"DrawCircle", TokenType.DrawCircleKeyword},{"DrawRectangle", TokenType.DrawRectangleKeyword},{"Fill", TokenType.FillKeyword},
            {"GetActualX", TokenType.GetActualXKeyword},{"GetActualY", TokenType.GetActualYKeyword},{"GetCanvasSize", TokenType.GetCanvasSizeKeyword},
            {"GetColorCount", TokenType.GetColorCountKeyword},{"IsBrushColor", TokenType.IsBrushColorKeyword},{"IsBrushSize", TokenType.IsBrushSizeKeyword},
            {"IsCanvasColor", TokenType.IsCanvasColorKeyword},{"GoTo", TokenType.GoToKeyword}
        };


        public List<Token> Tokenize(string sourceCode)
        {
            _sourceCode = sourceCode ?? string.Empty;
            _position = 0;
            _currentLine = 1;
            _currentColumn = 1;
            _tokens = new List<Token>();

            while (_position < _sourceCode.Length)
            {
                char currentChar = CurrentChar();

                if (char.IsWhiteSpace(currentChar))
                {
                    HandleWhitespace();
                }
                else if (char.IsDigit(currentChar))
                {
                    _tokens.Add(RecognizeNumber());
                }
                else if (currentChar == '"')
                {
                    _tokens.Add(RecognizeStringLiteral());
                }
                else if (char.IsLetter(currentChar))
                {
                    _tokens.Add(RecognizeIdentifierOrKeyword());
                }
                else if (IsOperatorOrPunctuationStart(currentChar))
                {
                    _tokens.Add(RecognizeOperatorOrPunctuation());
                }
                else
                {
                    _tokens.Add(new Token(TokenType.Unknown, currentChar.ToString(), _currentLine, _currentColumn));
                    errors.Add($"Lexer Error: Unrecognized character '{currentChar}' at Line {_currentLine}, Column {_currentColumn}");
                    Advance();
                }
            }

            _tokens.Add(new Token(TokenType.EndOfFile, string.Empty, _currentLine, _currentColumn));
            return _tokens;
        }

        private void Advance()
        {
            if (_position < _sourceCode.Length)
            {
                _currentColumn++;
                _position++;
            }
        }

        private char Peek()
        {
            if (_position + 1 >= _sourceCode.Length) return '\0';
            return _sourceCode[_position + 1];
        }

        private char PeekNext()
        {
            if (_position + 2 >= _sourceCode.Length) return '\0';
            return _sourceCode[_position + 2];
        }


        private char CurrentChar()
        {
            if (_position >= _sourceCode.Length) return '\0';
            return _sourceCode[_position];
        }

        private void HandleWhitespace()
        {
            char currentChar = CurrentChar();
            int startColumn = _currentColumn;

            if (currentChar == '\n')
            {
                _tokens.Add(new Token(TokenType.EndOfLine, "\\n", _currentLine, startColumn));
                Advance();
                _currentLine++;
                _currentColumn = 1;
            }
            else if (currentChar == '\r')
            {
                if (Peek() == '\n')
                {
                    _tokens.Add(new Token(TokenType.EndOfLine, "\\r\\n", _currentLine, startColumn));
                    Advance();
                    Advance();
                }
                else
                {
                    _tokens.Add(new Token(TokenType.EndOfLine, "\\r", _currentLine, startColumn));
                    Advance(); 
                }
                _currentLine++;
                _currentColumn = 1;
            }
            else
            {
                Advance();
            }
        }

        private Token RecognizeNumber()
        {
            int startPos = _position;
            int startColumn = _currentColumn;
            while (_position < _sourceCode.Length && char.IsDigit(CurrentChar()))
            {
                Advance();
            }
            string numberValue = _sourceCode.Substring(startPos, _position - startPos);
            return new Token(TokenType.NumberLiteral, numberValue, _currentLine, startColumn);
        }

        private Token RecognizeStringLiteral()
        {
            int startLine = _currentLine;
            int startColumn = _currentColumn;
            Advance();

            int stringStartPos = _position;
            while (_position < _sourceCode.Length && CurrentChar() != '"')
            {
                if (CurrentChar() == '\n' || CurrentChar() == '\r')
                {
                    errors.Add($"Lexer Error: Newline in string literal is not allowed. Started at Line {startLine}, Column {startColumn}");
                    string partialValue = _sourceCode.Substring(stringStartPos, _position - stringStartPos);
                    return new Token(TokenType.Unknown, "\"" + partialValue + "...", startLine, startColumn);
                }
                Advance();
            }

            if (_position >= _sourceCode.Length) 
            {
                errors.Add($"Lexer Error: Unterminated string literal at end of file. Started at Line {startLine}, Column {startColumn}");
                string partialValue = _sourceCode.Substring(stringStartPos, _position - stringStartPos);
                return new Token(TokenType.Unknown, "\"" + partialValue, startLine, startColumn);
            }
            else
            {
                string stringValue = _sourceCode.Substring(stringStartPos, _position - stringStartPos);
                Advance();
                return new Token(TokenType.StringLiteral, stringValue, startLine, startColumn);
            }
        }

        private Token RecognizeIdentifierOrKeyword()
        {
            int startPos = _position;
            int startColumn = _currentColumn;

            while (_position < _sourceCode.Length && (char.IsLetterOrDigit(CurrentChar()) || CurrentChar() == '_' || CurrentChar() == '-'))
            {
                Advance();
            }
            string identifier = _sourceCode.Substring(startPos, _position - startPos);

            if (Keywords.TryGetValue(identifier, out TokenType keywordType))
            {
                return new Token(keywordType, identifier, _currentLine, startColumn);
            }
            else
            {
                return new Token(TokenType.Identifier, identifier, _currentLine, startColumn);
            }
        }


        private Token RecognizeOperatorOrPunctuation()
        {
            int startColumn = _currentColumn;
            char firstChar = CurrentChar();
            char secondChar = Peek();

            string twoChars = $"{firstChar}{secondChar}";
            TokenType type = TokenType.Unknown;

            switch (twoChars)
            {
                case "<-": type = TokenType.AssignmentOperator; break;
                case "**": type = TokenType.PowerOperator; break;
                case "==": type = TokenType.EqualOperator; break;
                case "!=": type = TokenType.NotEqualOperator; break;
                case ">=": type = TokenType.GreaterEqualOperator; break;
                case "<=": type = TokenType.LessEqualOperator; break;
                case "&&": type = TokenType.AndOperator; break;
                case "||": type = TokenType.OrOperator; break;
            }

            if (type != TokenType.Unknown)
            {
                Advance(); 
                Advance(); 
                return new Token(type, twoChars, _currentLine, startColumn);
            }

            Advance();
            string oneChar = firstChar.ToString();
            switch (firstChar)
            {
                case '+': type = TokenType.PlusOperator; break;
                case '-': type = TokenType.MinusOperator; break; 
                case '*': type = TokenType.MultiplyOperator; break;
                case '/': type = TokenType.DivideOperator; break; 
                case '%': type = TokenType.ModuloOperator; break;
                case '>': type = TokenType.GreaterOperator; break;
                case '<': type = TokenType.LessOperator; break; 
                case '!': type = TokenType.NotOperator; break; 
                case '(': type = TokenType.LeftParen; break;
                case ')': type = TokenType.RightParen; break;
                case ',': type = TokenType.Comma; break;
                case '[': type = TokenType.LeftBracket; break;
                case ']': type = TokenType.RightBracket; break;

                default:
                    errors.Add($"Lexer Error: Unrecognized operator/punctuation start '{firstChar}' at Line {_currentLine}, Column {startColumn}");
                    return new Token(TokenType.Unknown, oneChar, _currentLine, startColumn);
            }
            return new Token(type, oneChar, _currentLine, startColumn);
        }

        private bool IsOperatorOrPunctuationStart(char c)
        { 
            return "+-*/%<>=!&|(),[]".IndexOf(c) != -1;
        }
    }
}