using System;
using System.Collections.Generic;
using UnityEngine;
using Interprete;
public partial class Interpreter
{
    public object VisitProgramNode(ProgramNode node)
    {
        Debug.LogWarning("VisitProgramNode called directly - usually handled by Interpret loop.");
        foreach (var statement in node.Statements) { Execute(statement); }
        return null;
    }

    public object VisitAssignmentNode(AssignmentNode node)
    {
        object value = Evaluate(node.ValueExpression); 

        if (!(value is int || value is bool)) {
            if (value is double d) { 
                value = Convert.ToInt32(d); 
            }
        
            if (!(value is int || value is bool)) {
                throw new RuntimeException($"Variable '{node.VariableNameToken.Value}' can only hold numbers or booleans, but received type {value?.GetType().Name ?? "null"}.", node.VariableNameToken);
            }
        }

        scope.DefineVariable(node.VariableNameToken.Value, value);
        return null;
    }

    public object VisitGoToNode(GoToNode node)
    {
        object conditionValue = Evaluate(node.Condition);
        bool jump = InterpreterHelpers.ExpectBoolean(conditionValue, node.TargetLabelToken, "GoTo condition");

        if (jump)
        {
            int targetIndex = scope.ResolveLabel(node.TargetLabelToken);
            _programCounter = targetIndex;
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
            case TokenType.SpawnKeyword: return ExecuteSpawn(node.CommandToken, evaluatedArgs);
            case TokenType.ColorKeyword: return ExecuteColor(node.CommandToken, evaluatedArgs);
            case TokenType.SizeKeyword: return ExecuteSize(node.CommandToken, evaluatedArgs);
            case TokenType.DrawLineKeyword: return ExecuteDrawLine(node.CommandToken, evaluatedArgs);
            case TokenType.DrawCircleKeyword: return ExecuteDrawCircle(node.CommandToken, evaluatedArgs);
            case TokenType.DrawRectangleKeyword: return ExecuteDrawRectangle(node.CommandToken, evaluatedArgs);
            case TokenType.FillKeyword: return ExecuteFill(node.CommandToken, evaluatedArgs);
            default:
                throw new RuntimeException($"Unknown function '{node.CommandToken.Value}'.", node.CommandToken);
        }
    }

    public object ExecuteSpawn(Token commandToken, List<object> args)
    {
        int x = InterpreterHelpers.ExpectNumber(args[0], commandToken, "X coordinate for Spawn");
        int y = InterpreterHelpers.ExpectNumber(args[1], commandToken, "Y coordinate for Spawn");

        int canvasSize = texture.width;
        if (x < 0 || x >= canvasSize || y < 0 || y >= canvasSize)
        {
            throw new RuntimeException($"Spawn position ({x},{y}) is outside the canvas bounds (0 to {canvasSize - 1}).", commandToken);
        }
        _walleX = x;
        _walleY = y;

        // Debug.Log($"Wall-E Spawned at ({_walleX}, {_walleY})");
        return null;
    }

     public object ExecuteColor(Token commandToken, List<object> args)
    {
        object colorArg = args[0];
        string colorName = InterpreterHelpers.ExpectString(colorArg, commandToken, "Color name");

        if (!InterpreterHelpers.TryParseColor(colorName, out Color newColor)) {
            Token errorToken = commandToken;
            throw new RuntimeException($"Invalid color name: '{colorName}'.", errorToken);
        }
        _currentBrushColor = newColor;
        // Debug.Log($"Brush color set to: {colorName}");
        return null;
    }

    public object ExecuteSize(Token commandToken, List<object> args)
    {
        object sizeArg = args[0];
        int k = InterpreterHelpers.ExpectNumber(sizeArg, commandToken, "Brush size");

        if (k <= 0) {
            Token errorToken = commandToken;
            throw new RuntimeException("Brush size must be positive.", errorToken);
        }

        _currentBrushSize = (k % 2 == 0) ? k - 1 : k;
        // Debug.Log($"Brush size set to: {_currentBrushSize}");
        return null;
    }

     public object ExecuteDrawLine(Token commandToken, List<object> args)
    {
        int dirX = InterpreterHelpers.ExpectNumber(args[0], commandToken, "dirX for DrawLine");
        int dirY = InterpreterHelpers.ExpectNumber(args[1], commandToken, "dirY for DrawLine");
        int distance = InterpreterHelpers.ExpectNumber(args[2], commandToken, "distance for DrawLine");

        if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1) throw new RuntimeException("DrawLine directions (dirX, dirY) must be -1, 0, or 1.", commandToken);
        if (dirX == 0 && dirY == 0) throw new RuntimeException("DrawLine direction cannot be (0, 0).", commandToken);
        if (distance <= 0) throw new RuntimeException("DrawLine distance must be positive.", commandToken);

        int startX = _walleX;
        int startY = _walleY;
        int currentX = startX;
        int currentY = startY;
        int canvasSize = texture.width;

        for (int i = 0; i < distance; i++)
        {
            if (currentX < 0 || currentX >= canvasSize || currentY < 0 || currentY >= canvasSize) {
                throw new RuntimeException($"Wall-E position is outside canvas bounds ({currentX - dirX},{currentY - dirY}) after DrawLine.", commandToken);
            }
            DrawPixelWithBrush(currentX, currentY);
            currentX += dirX;
            currentY += dirY;
        }

        _walleX = currentX - dirX;
        _walleY = currentY - dirY;

        return null;
    }

     public object ExecuteDrawCircle(Token commandToken, List<object> args)
    {
        int dirX = InterpreterHelpers.ExpectNumber(args[0], commandToken, "dirX for DrawCircle");
        int dirY = InterpreterHelpers.ExpectNumber(args[1], commandToken, "dirY for DrawCircle");
        int radius = InterpreterHelpers.ExpectNumber(args[2], commandToken, "radius for DrawCircle");

        if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1) throw new RuntimeException("DrawCircle directions (dirX, dirY) must be -1, 0, or 1.", commandToken);
        if (dirX == 0 && dirY == 0) throw new RuntimeException("DrawCircle direction cannot be (0, 0).", commandToken);
        if (radius <= 0) throw new RuntimeException("DrawCircle radius must be positive.", commandToken);

        int centerX = _walleX + dirX * radius;
        int centerY = _walleY + dirY * radius;
        int canvasSize = texture.width;

        if (centerX < 0 || centerX >= canvasSize || centerY < 0 || centerY >= canvasSize) {
            throw new RuntimeException($"Wall-E position is outside canvas bounds ({_walleX},{_walleY}) after DrawCircle.", commandToken);
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

        return null;
    }

     public object ExecuteDrawRectangle(Token commandToken, List<object> args)
    {
        int dirX = InterpreterHelpers.ExpectNumber(args[0], commandToken, "dirX for DrawRectangle");
        int dirY = InterpreterHelpers.ExpectNumber(args[1], commandToken, "dirY for DrawRectangle");
        int distance = InterpreterHelpers.ExpectNumber(args[2], commandToken, "distance for DrawRectangle");
        int width = InterpreterHelpers.ExpectNumber(args[3], commandToken, "width for DrawRectangle");
        int height = InterpreterHelpers.ExpectNumber(args[4], commandToken, "height for DrawRectangle");

        if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1) throw new RuntimeException("DrawRectangle directions (dirX, dirY) must be -1, 0, or 1.", commandToken);
        if (distance < 0) throw new RuntimeException("DrawRectangle distance cannot be negative.", commandToken);
        if (width <= 0) throw new RuntimeException("DrawRectangle width must be positive.", commandToken);
        if (height <= 0) throw new RuntimeException("DrawRectangle height must be positive.", commandToken);
    
        int centerX = _walleX + dirX * distance;
        int centerY = _walleY + dirY * distance;
        int canvasSize = texture.width;

        if (centerX < 0 || centerX >= canvasSize || centerY < 0 || centerY >= canvasSize) {
            throw new RuntimeException($"Wall-E position is outside canvas bounds ({_walleX},{_walleY}) after DrawRectangle.", commandToken);
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

        return null;
    }

    public object ExecuteFill(Token commandToken, List<object> args)
    {
        Color targetColor = texture.GetPixel(_walleX, _walleY);

        if (InterpreterHelpers.ColorsApproximatelyEqual(_currentBrushColor, targetColor) || _currentBrushColor == Color.clear)
        {
            return null;
        } 
        FloodFillIterative(_walleX,_walleY,targetColor);

        return null;
    }
    public void FloodFillIterative(int startX, int startY, Color targetColor) {
    int w = texture.width, h = texture.height;
    var toProcess = new Stack<(int x,int y)>();
    var visited   = new HashSet<(int,int)>();
    toProcess.Push((startX, startY));

    while (toProcess.Count > 0) {
            var (x,y) = toProcess.Pop();
            if (x < 0 || x >= w || y < 0 || y >= h) continue;
            if (visited.Contains((x,y))) continue;

            Color c = texture.GetPixel(x,y);
            if (!InterpreterHelpers.ColorsApproximatelyEqual(c, targetColor))
                continue;

            texture.SetPixel(x, y, _currentBrushColor);
            visited.Add((x,y));

            toProcess.Push((x+1, y));
            toProcess.Push((x-1, y));
            toProcess.Push((x, y+1));
            toProcess.Push((x, y-1));
        }
    }
}
