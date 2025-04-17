using System;
using UnityEngine;

namespace Interprete{
    public enum TokenType
    {
        //Keywords
        SpawnKeyword, ColorKeyword, SizeKeyword, DrawLineKeyword, DrawCircleKeyword, DrawRectangleKeyword, FillKeyword,

        //Function Keywords
        GetActualXKeyword, GetActualYKeyword, GetCanvasSizeKeyword, GetColorCountKeyword, IsBrushColorKeyword, IsBrushSizeKeyword, IsCanvasColorKeyword,

        //Control Flow
        GoToKeyword,

        //Literals
        NumberLiteral,StringLiteral,     

        //Identifiers
        Identifier,// Variable,Label

        //Operators 
        PlusOperator,MinusOperator,MultiplyOperator,DivideOperator,PowerOperator,ModuloOperator,AssignmentOperator,
        // Comparison Operators
        EqualOperator,NotEqualOperator,GreaterOperator,LessOperator,GreaterEqualOperator,LessEqualOperator,    
        // Logical Operators
        AndOperator,OrOperator,NotOperator,

        //Punctuation
        LeftParen, RightParen,Comma,LeftBracket, RightBracket,

        //Special Tokens
        EndOfLine,EndOfFile,        

        //Error Token
        Unknown
    }

    public struct Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public int Line { get; }
        public int Column { get; }

        public Token(TokenType type, string value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }

        public override string ToString()
        {
            return $"[{Line}:{Column}] {Type}: '{Value}'";
        }
    }
}