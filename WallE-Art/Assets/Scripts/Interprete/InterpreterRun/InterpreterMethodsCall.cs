using System;
using System.Collections.Generic;
using UnityEngine;
using Interprete;
public partial class Interpreter
{
    public object VisitLiteralNode<TValue>(LiteralNode<TValue> node)
    {
        return node.Value;
    }

    public object VisitVariableNode(VariableNode node)
    {
        return scope.GetVariable(node.NameToken);
    }

    public object VisitUnaryOpNode(UnaryOpNode node)
    {
        object right = Evaluate(node.Right);

        switch (node.OperatorToken.Type)
        {
            case TokenType.MinusOperator:
                int operand = InterpreterHelpers.ExpectNumber(right, node.OperatorToken, "Operand for unary '-'");
                return -operand;
            case TokenType.NotOperator:
                bool boolOperand = InterpreterHelpers.ExpectBoolean(right, node.OperatorToken, "Operand for '!'");
                return !boolOperand;
            default:
                throw new RuntimeException($"Invalid unary operator '{node.OperatorToken.Value}'.", node.OperatorToken);
        }
    }

    public object VisitBinaryOpNode(BinaryOpNode node)
    {
        object left = Evaluate(node.Left);
        object right = Evaluate(node.Right);

        if (node.OperatorToken.Type == TokenType.PlusOperator ||
            node.OperatorToken.Type == TokenType.MinusOperator ||
            node.OperatorToken.Type == TokenType.MultiplyOperator ||
            node.OperatorToken.Type == TokenType.DivideOperator ||
            node.OperatorToken.Type == TokenType.ModuloOperator ||
            node.OperatorToken.Type == TokenType.PowerOperator)
        {
            int leftNum = InterpreterHelpers.ExpectNumber(left, node.OperatorToken, "Left operand");
            int rightNum = InterpreterHelpers.ExpectNumber(right, node.OperatorToken, "Right operand");

            switch (node.OperatorToken.Type)
            {
                case TokenType.PlusOperator: return leftNum + rightNum;
                case TokenType.MinusOperator: return leftNum - rightNum;
                case TokenType.MultiplyOperator: return leftNum * rightNum;
                case TokenType.DivideOperator:
                    if (rightNum == 0) throw new RuntimeException("Division by zero.", node.OperatorToken);
                    return leftNum / rightNum;
                case TokenType.ModuloOperator:
                    if (rightNum == 0) throw new RuntimeException("Modulo by zero.", node.OperatorToken);
                    return leftNum % rightNum;
                case TokenType.PowerOperator:
                     try { 
                        return Convert.ToInt32(Math.Pow(leftNum, rightNum));
                     } catch (OverflowException) {
                        throw new RuntimeException($"Result of {leftNum} ** {rightNum} is too large for an integer.", node.OperatorToken);
                     }
                default: break;
            }
        }

        if (node.OperatorToken.Type == TokenType.GreaterOperator ||
            node.OperatorToken.Type == TokenType.GreaterEqualOperator ||
            node.OperatorToken.Type == TokenType.LessOperator ||
            node.OperatorToken.Type == TokenType.LessEqualOperator ||
            node.OperatorToken.Type == TokenType.EqualOperator ||
            node.OperatorToken.Type == TokenType.NotEqualOperator)
        {
            if (left is int lNum && right is int rNum)
            {
                switch (node.OperatorToken.Type) {
                    case TokenType.GreaterOperator: return lNum > rNum;
                    case TokenType.GreaterEqualOperator: return lNum >= rNum;
                    case TokenType.LessOperator: return lNum < rNum;
                    case TokenType.LessEqualOperator: return lNum <= rNum;
                    case TokenType.EqualOperator: return lNum == rNum;
                    case TokenType.NotEqualOperator: return lNum != rNum;
                }
            }
            else if (left is bool lBool && right is bool rBool)
            {
                switch (node.OperatorToken.Type) {
                    case TokenType.EqualOperator: return lBool == rBool;
                    case TokenType.NotEqualOperator: return lBool != rBool;
                    default: throw new RuntimeException($"Operator '{node.OperatorToken.Value}' cannot compare booleans.", node.OperatorToken);
                }
            }
             else if (left is string lStr && right is string rStr)
            {
                switch (node.OperatorToken.Type) {
                    case TokenType.EqualOperator: return lStr.Equals(rStr, StringComparison.OrdinalIgnoreCase);
                    case TokenType.NotEqualOperator: return !lStr.Equals(rStr, StringComparison.OrdinalIgnoreCase);
                    default: throw new RuntimeException($"Operator '{node.OperatorToken.Value}' cannot compare strings.", node.OperatorToken);
                 }
            }
            throw new RuntimeException($"Cannot compare values of types ({left?.GetType().Name ?? "null"} and {right?.GetType().Name ?? "null"}) with operator '{node.OperatorToken.Value}'.", node.OperatorToken);
        }

        if (node.OperatorToken.Type == TokenType.AndOperator ||
            node.OperatorToken.Type == TokenType.OrOperator)
        {
            bool leftBool = InterpreterHelpers.ExpectBoolean(left, node.OperatorToken, "Left operand");
            bool rightBool = InterpreterHelpers.ExpectBoolean(right, node.OperatorToken, "Right operand");

             switch (node.OperatorToken.Type)
            {
                case TokenType.AndOperator: return leftBool && rightBool; 
                case TokenType.OrOperator: return leftBool || rightBool; 
            }
        }

        throw new RuntimeException($"Invalid binary operator '{node.OperatorToken.Value}'.", node.OperatorToken);
    }

    public object VisitFunctionCallNode(FunctionCallNode node)
    {
        List<object> evaluatedArgs = new List<object>();
        foreach (var argExpr in node.Arguments)
        {
            evaluatedArgs.Add(Evaluate(argExpr));
        }

        switch (node.FunctionNameToken.Type)
        {
            case TokenType.GetActualXKeyword: return ExecuteGetActualX(node.FunctionNameToken, evaluatedArgs);
            case TokenType.GetActualYKeyword: return ExecuteGetActualY(node.FunctionNameToken, evaluatedArgs);
            case TokenType.GetCanvasSizeKeyword: return ExecuteGetCanvasSize(node.FunctionNameToken, evaluatedArgs);
            case TokenType.IsBrushColorKeyword: return ExecuteIsBrushColor(node.FunctionNameToken, evaluatedArgs);
            case TokenType.IsBrushSizeKeyword: return ExecuteIsBrushSize(node.FunctionNameToken, evaluatedArgs);
            case TokenType.IsCanvasColorKeyword: return ExecuteIsCanvasColor(node.FunctionNameToken, evaluatedArgs);
            case TokenType.GetColorCountKeyword: return ExecuteGetColorCount(node.FunctionNameToken, evaluatedArgs);
            default:
                throw new RuntimeException($"Unknown function '{node.FunctionNameToken.Value}'.", node.FunctionNameToken);
        }
    }

    private object ExecuteGetActualX(Token funcToken, List<object> args) {
        return _walleX;
    }
    private object ExecuteGetActualY(Token funcToken, List<object> args) {
        return _walleY;
    }
     private object ExecuteGetCanvasSize(Token funcToken, List<object> args) {
        return texture.width;
    }
     private object ExecuteIsBrushColor(Token funcToken, List<object> args) {
        string colorToCheck = InterpreterHelpers.ExpectString(args[0], funcToken, "Color name for IsBrushColor");
        if (!InterpreterHelpers.TryParseColor(colorToCheck, out Color targetColor)) {
            throw new RuntimeException($"Invalid color name '{colorToCheck}' in IsBrushColor.", funcToken);
        }
        bool isSameColor = InterpreterHelpers.ColorsApproximatelyEqual(_currentBrushColor, targetColor);
        return isSameColor ? 1 : 0;
    }
     private object ExecuteIsBrushSize(Token funcToken, List<object> args) {
        int sizeToCheck = InterpreterHelpers.ExpectNumber(args[0], funcToken, "Size for IsBrushSize");
        return (_currentBrushSize == sizeToCheck) ? 1 : 0;
    }
      private object ExecuteIsCanvasColor(Token funcToken, List<object> args) {
        string canvasColorName = InterpreterHelpers.ExpectString(args[0], funcToken, "Color name for IsCanvasColor");
        int verticalOffset = InterpreterHelpers.ExpectNumber(args[1], funcToken, "Vertical offset for IsCanvasColor");
        int horizontalOffset = InterpreterHelpers.ExpectNumber(args[2], funcToken, "Horizontal offset for IsCanvasColor");

        if (!InterpreterHelpers.TryParseColor(canvasColorName, out Color canvasTargetColor)) {
            throw new RuntimeException($"Invalid color name '{canvasColorName}' in IsCanvasColor.", funcToken);
        }

        int checkX = _walleX + horizontalOffset;
        int checkY = _walleY + verticalOffset;

        int canvasSize = texture.width;
        if (checkX < 0 || checkX >= canvasSize || checkY < 0 || checkY >= canvasSize) {
            return 0;
        }

        Color actualPixelColor = texture.GetPixel(checkX, checkY);
        bool isTargetColor = InterpreterHelpers.ColorsApproximatelyEqual(actualPixelColor, canvasTargetColor);
        return isTargetColor ? 1 : 0;
    }
     private object ExecuteGetColorCount(Token funcToken, List<object> args) {
        string countColorName = InterpreterHelpers.ExpectString(args[0], funcToken, "Color name for GetColorCount");
        int x1 = InterpreterHelpers.ExpectNumber(args[1], funcToken, "x1 for GetColorCount");
        int y1 = InterpreterHelpers.ExpectNumber(args[2], funcToken, "y1 for GetColorCount");
        int x2 = InterpreterHelpers.ExpectNumber(args[3], funcToken, "x2 for GetColorCount");
        int y2 = InterpreterHelpers.ExpectNumber(args[4], funcToken, "y2 for GetColorCount");

        if (!InterpreterHelpers.TryParseColor(countColorName, out Color countTargetColor)) {
            throw new RuntimeException($"Invalid color name '{countColorName}' in GetColorCount.", funcToken);
        }

        int cSize = texture.width;
        int startX = Math.Min(x1, x2); 
        int startY = Math.Min(y1, y2);
        int endX = Math.Max(x1, x2); 
        int endY = Math.Max(y1, y2);

        if (startX >= cSize || startY >= cSize || endX < 0 || endY < 0) return 0;

        int count = 0;
        for (int y = startY; y <= endY; y++) {
            for (int x = startX; x <= endX; x++) {
                if (InterpreterHelpers.ColorsApproximatelyEqual(texture.GetPixel(x, y), countTargetColor)) {
                    count++;
                }
            }
        }
        return count;
    }
} 
