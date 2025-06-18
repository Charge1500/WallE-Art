using System;
using System.Collections.Generic;
using System.Linq;

namespace Interprete
{
    public static class FunctionRegistry
    {
        private static readonly Dictionary<string, FunctionDefinition> ByName;
        private static readonly Dictionary<TokenType, FunctionDefinition> ByToken;

        static FunctionRegistry()
        {
            ByName = new Dictionary<string, FunctionDefinition>(/*StringComparer.OrdinalIgnoreCase*/);
            ByToken = new Dictionary<TokenType, FunctionDefinition>();

            // Comandos (devuelven Void)
            Define("Spawn", TokenType.SpawnKeyword, FunctionCategory.Command, ValueType.Void, ValueType.Number, ValueType.Number);
            Define("MoveTo", TokenType.MoveToKeyword, FunctionCategory.Command, ValueType.Void, ValueType.Number, ValueType.Number);
            Define("Color", TokenType.ColorKeyword, FunctionCategory.Command, ValueType.Void, ValueType.String);
            Define("Size", TokenType.SizeKeyword, FunctionCategory.Command, ValueType.Void, ValueType.Number);
            Define("DrawLine", TokenType.DrawLineKeyword, FunctionCategory.Command, ValueType.Void, ValueType.Number, ValueType.Number, ValueType.Number);
            Define("DrawCircle", TokenType.DrawCircleKeyword, FunctionCategory.Command, ValueType.Void, ValueType.Number, ValueType.Number, ValueType.Number);
            Define("DrawRectangle", TokenType.DrawRectangleKeyword, FunctionCategory.Command, ValueType.Void, ValueType.Number, ValueType.Number, ValueType.Number, ValueType.Number, ValueType.Number);
            Define("Fill", TokenType.FillKeyword, FunctionCategory.Command, ValueType.Void);

            // Funciones (devuelven un valor)
            Define("GetActualX", TokenType.GetActualXKeyword, FunctionCategory.Function, ValueType.Number);
            Define("GetActualY", TokenType.GetActualYKeyword, FunctionCategory.Function, ValueType.Number);
            Define("GetCanvasSize", TokenType.GetCanvasSizeKeyword, FunctionCategory.Function, ValueType.Number);
            Define("IsBrushColor", TokenType.IsBrushColorKeyword, FunctionCategory.Function, ValueType.Boolean, ValueType.String);
            Define("IsBrushSize", TokenType.IsBrushSizeKeyword, FunctionCategory.Function, ValueType.Boolean, ValueType.Number);
            Define("IsCanvasColor", TokenType.IsCanvasColorKeyword, FunctionCategory.Function, ValueType.Boolean, ValueType.String, ValueType.Number, ValueType.Number);
            Define("GetColorCount", TokenType.GetColorCountKeyword, FunctionCategory.Function, ValueType.Number, ValueType.String, ValueType.Number, ValueType.Number, ValueType.Number, ValueType.Number);
        }

        private static void Define(string name, TokenType tokenType, FunctionCategory category, ValueType returnType, params ValueType[] argTypes)
        {
            FunctionDefinition def = new FunctionDefinition(name, tokenType, category, returnType, argTypes);
            ByName[name] = def;
            ByToken[tokenType] = def;
        }

        public static FunctionDefinition Get(string name) => ByName.TryGetValue(name, out var def) ? def : null;
        public static FunctionDefinition Get(TokenType tokenType) => ByToken.TryGetValue(tokenType, out var def) ? def : null;

        public static bool IsFunction(TokenType tokenType) => ByToken.TryGetValue(tokenType, out var def) && def.Category == FunctionCategory.Function;
        public static bool IsCommand(TokenType tokenType) => ByToken.TryGetValue(tokenType, out var def) && def.Category == FunctionCategory.Command;
        
        public static IEnumerable<FunctionDefinition> AllDefinitions => ByName.Values;
    }
}
