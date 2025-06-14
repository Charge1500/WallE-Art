using System;
using System.Collections.Generic;
using UnityEngine;
using Interprete;

public partial class Interpreter
{
    public object VisitLiteralNode<TValue>(LiteralNode<TValue> node) => node.Value;
    public object VisitVariableNode(VariableNode node) => _runtimeScope.GetVariable(node.NameToken);

    public object VisitUnaryOpNode(UnaryOpNode node)
    {
        object right = Evaluate(node.Right);
        switch (node.OperatorToken.Type)
        {
            case TokenType.MinusOperator: return -(int)right;
            case TokenType.NotOperator: return !(bool)right;
        }
        throw new RuntimeException("Unreachable: Invalid unary operator.", node.OperatorToken);
    }

    public object VisitBinaryOpNode(BinaryOpNode node)
    {
        object left = Evaluate(node.Left);
        object right = Evaluate(node.Right);
        
        switch (node.OperatorToken.Type)
        {
            case TokenType.PlusOperator: return (int)left + (int)right;
            case TokenType.MinusOperator: return (int)left - (int)right;
            case TokenType.MultiplyOperator: return (int)left * (int)right;
            case TokenType.DivideOperator:
                if ((int)right == 0) throw new RuntimeException("Division by zero.", node.OperatorToken);
                return (int)left / (int)right;
            case TokenType.ModuloOperator:
                if ((int)right == 0) throw new RuntimeException("Modulo by zero.", node.OperatorToken);
                return (int)left % (int)right;
            case TokenType.PowerOperator: return (int)Math.Pow((int)left, (int)right);

            case TokenType.GreaterOperator: return (int)left > (int)right;
            case TokenType.GreaterEqualOperator: return (int)left >= (int)right;
            case TokenType.LessOperator: return (int)left < (int)right;
            case TokenType.LessEqualOperator: return (int)left <= (int)right;

            case TokenType.EqualOperator: return Equals(left, right);
            case TokenType.NotEqualOperator: return !Equals(left, right);

            case TokenType.AndOperator: return (bool)left && (bool)right;
            case TokenType.OrOperator: return (bool)left || (bool)right;
        }
        throw new RuntimeException("Unreachable: Invalid binary operator.", node.OperatorToken);
    }
    
    public object VisitProgramNode(ProgramNode node) => null; 
    public object VisitLabelNode(LabelNode node) => null;

    public object VisitAssignmentNode(AssignmentNode node)
    {
        object value = Evaluate(node.ValueExpression);
        _runtimeScope.DefineVariable(node.VariableNameToken.Value, value);
        return null;
    }

    public object VisitGoToNode(GoToNode node)
    {
        if ((bool)Evaluate(node.Condition))
        {
            _programCounter = _labelPositions[node.TargetLabelToken.Value];
            _goToExecuted = true;
        }
        return null;
    }
    
    public object VisitCommandNode(CommandNode node)
    {
        List<object> evaluatedArgs = new List<object>();
        foreach (var argExpr in node.Arguments)
        {
            evaluatedArgs.Add(Evaluate(argExpr));
        }

        switch (node.CommandToken.Type)
        {
            case TokenType.SpawnKeyword:       ExecuteSpawn(node.CommandToken, evaluatedArgs); break;
            case TokenType.ColorKeyword:       ExecuteColor(node.CommandToken, evaluatedArgs); break;
            case TokenType.SizeKeyword:        ExecuteSize(node.CommandToken, evaluatedArgs); break;
            case TokenType.DrawLineKeyword:    ExecuteDrawLine(node.CommandToken, evaluatedArgs); break;
            case TokenType.DrawCircleKeyword:  ExecuteDrawCircle(node.CommandToken, evaluatedArgs); break;
            case TokenType.DrawRectangleKeyword: ExecuteDrawRectangle(node.CommandToken, evaluatedArgs); break;
            case TokenType.FillKeyword:        ExecuteFill(node.CommandToken, evaluatedArgs); break;
            default:
                throw new RuntimeException($"Unknown command '{node.CommandToken.Value}'.", node.CommandToken);
        }
        return null;
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
            case TokenType.GetActualXKeyword: return _walleX;
            case TokenType.GetActualYKeyword: return _walleY;
            case TokenType.GetCanvasSizeKeyword: return _texture.width;
            case TokenType.IsBrushColorKeyword:
                InterpreterHelpers.TryParseColor((string)evaluatedArgs[0], out var targetColor);
                return InterpreterHelpers.ColorsApproximatelyEqual(_currentBrushColor, targetColor);
            case TokenType.IsBrushSizeKeyword: return _currentBrushSize == (int)evaluatedArgs[0];
            case TokenType.IsCanvasColorKeyword:
                InterpreterHelpers.TryParseColor((string)evaluatedArgs[0], out var canvasTargetColor);
                int checkX = _walleX + (int)evaluatedArgs[2]; 
                int checkY = _walleY + (int)evaluatedArgs[1]; 
                if (checkX < 0 || checkX >= _texture.width || checkY < 0 || checkY >= _texture.height) return false;
                return InterpreterHelpers.ColorsApproximatelyEqual(_texture.GetPixel(checkX, checkY), canvasTargetColor);
            case TokenType.GetColorCountKeyword:
                return ExecuteGetColorCount(node.FunctionNameToken, evaluatedArgs);
            default:
                throw new RuntimeException($"Unknown function '{node.FunctionNameToken.Value}'.", node.FunctionNameToken);
        }
    }

    private void ExecuteSpawn(Token token, List<object> args)
    {
        _walleX = (int)args[0]; _walleY = (int)args[1];
        if (_walleX < 0 || _walleX >= _texture.width || _walleY < 0 || _walleY >= _texture.height)
            throw new RuntimeException($"Spawn position ({_walleX},{_walleY}) is outside canvas.", token);
    }
    
    private void ExecuteColor(Token token, List<object> args)
    {
        if (!InterpreterHelpers.TryParseColor((string)args[0], out _currentBrushColor))
            throw new RuntimeException($"Invalid color name: '{(string)args[0]}'.", token);
    }
    
    private void ExecuteSize(Token token, List<object> args) => _currentBrushSize = (int)args[0];
    
    private void ExecuteDrawLine(Token token, List<object> args)
    {
        int dirX = (int)args[0], dirY = (int)args[1], dist = (int)args[2];
        for (int i = 0; i < dist; i++) { DrawPixelWithBrush(_walleX + (dirX * i), _walleY + (dirY * i)); }
        _walleX += dirX * (dist > 0 ? dist - 1 : 0);
        _walleY += dirY * (dist > 0 ? dist - 1 : 0);
    }

    private void ExecuteDrawCircle(Token token, List<object> args)
    {
        int dirX = (int)args[0], dirY = (int)args[1], radius = (int)args[2];

        if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1) throw new RuntimeException("DrawCircle directions (dirX, dirY) must be -1, 0, or 1.", token);
        if (dirX == 0 && dirY == 0) throw new RuntimeException("DrawCircle direction cannot be (0, 0).", token);
        if (radius <= 0) throw new RuntimeException("DrawCircle radius must be positive.", token);

        int centerX = _walleX + dirX * radius;
        int centerY = _walleY + dirY * radius;
        int canvasSize = _texture.width;

        if (centerX < 0 || centerX >= canvasSize || centerY < 0 || centerY >= canvasSize) {
            throw new RuntimeException($"Wall-E position is outside canvas bounds ({_walleX},{_walleY}) after DrawCircle.", token);
        }

        int x = radius -1;
        int y = 0;
        int dx = 1;
        int dy = 1;
        int err = dx - (radius * 2);

        while (x >= y)
        {
            DrawPixelWithBrush(centerX + x, centerY + y); DrawPixelWithBrush(centerX + y, centerY + x);
            DrawPixelWithBrush(centerX - y, centerY + x); DrawPixelWithBrush(centerX - x, centerY + y);
            DrawPixelWithBrush(centerX - x, centerY - y); DrawPixelWithBrush(centerX - y, centerY - x);
            DrawPixelWithBrush(centerX + y, centerY - x); DrawPixelWithBrush(centerX + x, centerY - y);

            if (err <= 0) { y++; err += dy; dy += 2; }
            if (err > 0) { x--; dx += 2; err += dx - (radius * 2); }
        }

        _walleX = centerX;
        _walleY = centerY;
    }
    
    private void ExecuteDrawRectangle(Token token, List<object> args)
    {
        int dirX = (int)args[0], dirY = (int)args[1], distance = (int)args[2], width = (int)args[3], height = (int)args[4];

        if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1) throw new RuntimeException("DrawRectangle directions (dirX, dirY) must be -1, 0, or 1.", token);
        if (distance < 0) throw new RuntimeException("DrawRectangle distance cannot be negative.", token);
        if (width <= 0) throw new RuntimeException("DrawRectangle width must be positive.", token);
        if (height <= 0) throw new RuntimeException("DrawRectangle height must be positive.", token);
    
        int centerX = _walleX + dirX * distance;
        int centerY = _walleY + dirY * distance;
        int canvasSize = _texture.width;

        if (centerX < 0 || centerX >= canvasSize || centerY < 0 || centerY >= canvasSize) {
            throw new RuntimeException($"Wall-E position is outside canvas bounds ({_walleX},{_walleY}) after DrawRectangle.", token);
        }

        int halfWidth = width / 2;
        int halfHeight = height / 2;
        int x1 = centerX - halfWidth;
        int y1 = centerY - halfHeight;
        int x2 = x1 + width - 1;
        int y2 = y1 + height - 1;

        for (int x = x1; x <= x2; x++) { DrawPixelWithBrush(x, y1); DrawPixelWithBrush(x, y2); }
        for (int y = y1 + 1; y < y2; y++) { DrawPixelWithBrush(x1, y); DrawPixelWithBrush(x2, y); }

        _walleX = centerX;
        _walleY = centerY;
    }

    private void ExecuteFill(Token token, List<object> args)
    {
        Color targetColor = _texture.GetPixel(_walleX, _walleY);
        if (InterpreterHelpers.ColorsApproximatelyEqual(_currentBrushColor, targetColor) || _currentBrushColor == Color.clear) return;
        Fill(_walleX, _walleY, targetColor);
    }

    public void Fill(int startX, int startY, Color targetColor) {
        int w = _texture.width, h = _texture.height;
        var toProcess = new Stack<(int x, int y)>();
        var visited = new HashSet<(int, int)>();
        toProcess.Push((startX, startY));
        while (toProcess.Count > 0)
        {
            var (x, y) = toProcess.Pop();
            if (x < 0 || x >= w || y < 0 || y >= h || visited.Contains((x, y))) continue;
            
            Color c = _texture.GetPixel(x, y);
            if (!InterpreterHelpers.ColorsApproximatelyEqual(c, targetColor)) continue;
            
            _texture.SetPixel(x, y, _currentBrushColor);
            visited.Add((x, y));
            
            toProcess.Push((x + 1, y)); toProcess.Push((x - 1, y));
            toProcess.Push((x, y + 1)); toProcess.Push((x, y - 1));
        }
    }

    private int ExecuteGetColorCount(Token funcToken, List<object> args)
    {
        string countColorName = (string)args[0];
        int x1 = (int)args[1], y1 = (int)args[2],x2 = (int)args[3],y2 = (int)args[4];

        if (!InterpreterHelpers.TryParseColor(countColorName, out Color countTargetColor)) {
            throw new RuntimeException($"Invalid color name '{countColorName}' in GetColorCount.", funcToken);
        }

        int cSize = _texture.width;
        int startX = Math.Min(x1, x2); 
        int startY = Math.Min(y1, y2);
        int endX = Math.Max(x1, x2); 
        int endY = Math.Max(y1, y2);

        if (startX >= cSize || startY >= cSize || endX < 0 || endY < 0) return 0;

        int count = 0;
        for (int y = startY; y <= endY; y++) {
            for (int x = startX; x <= endX; x++) {
                if (InterpreterHelpers.ColorsApproximatelyEqual(_texture.GetPixel(x, y), countTargetColor)) {
                    count++;
                }
            }
        }
        return count;
    }
}
