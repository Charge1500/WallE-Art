using System;
using System.Collections.Generic;
using UnityEngine;
using Interprete;

public partial class Interpreter : IAstVisitor<object>
{
    private readonly int max_execution_steps = 1000000;
    private int _executedSteps = 0;

    private Scope _runtimeScope = new Scope();
    private readonly Texture2D _texture;

    private int _walleX;
    private int _walleY;
    private Color _currentBrushColor = Color.clear;
    private int _currentBrushSize = 1;
    public List<string> errors = new List<string>();

    private int _programCounter = 0;
    private bool _goToExecuted = false;
    private readonly Dictionary<string, int> _labelPositions = new Dictionary<string, int>();


    public Interpreter(Texture2D canvasTexture)
    {
        _texture = canvasTexture;
    }

    public Texture2D Interpret(ProgramNode program)
    {
        _programCounter = 0;
        _executedSteps = 0;
        _currentBrushColor = Color.clear;
        _currentBrushSize = 1;
        _runtimeScope = new Scope();
        _labelPositions.Clear();
        errors.Clear();

        try
        {
            FindLabelPositions(program);

            while (_programCounter < program.Statements.Count)
            {
                _executedSteps++;
                if (_executedSteps > max_execution_steps)
                {
                    throw new RuntimeException($"Execution aborted: Maximum step limit ({max_execution_steps}) exceeded. Possible infinite loop.");
                }

                _goToExecuted = false;
                StatementNode currentStatement = program.Statements[_programCounter];
                
                Execute(currentStatement);

                if (!_goToExecuted)
                {
                    _programCounter++;
                }
            }
            _texture.Apply();
            return _texture;
        }
        catch (RuntimeException ex)
        {
            errors.Add(ex.Message);
        }
        catch (Exception ex)
        {
            errors.Add($"[FATAL] Unexpected Interpreter Error: {ex.GetType().Name} - {ex.Message}");
        }
        return null;
    }

    private void Execute(StatementNode node) => node.Accept(this);
    private object Evaluate(ExpressionNode node) => node.Accept(this);

    private void FindLabelPositions(ProgramNode program)
    {
        for (int i = 0; i < program.Statements.Count; i++)
        {
            if (program.Statements[i] is LabelNode labelNode)
            {
                _labelPositions[labelNode.LabelToken.Value] = i;
            }
        }
    }
    
    private void DrawPixelWithBrush(int cx, int cy)
    {
        if (_currentBrushColor == Color.clear) return;
        int canvasSize = _texture.width;
        int halfBrush = _currentBrushSize / 2;
        for (int offsetY = -halfBrush; offsetY <= halfBrush; offsetY++)
        {
            for (int offsetX = -halfBrush; offsetX <= halfBrush; offsetX++)
            {
                int px = cx + offsetX;
                int py = cy + offsetY;
                if (px >= 0 && px < canvasSize && py >= 0 && py < canvasSize)
                {
                    _texture.SetPixel(px, py, _currentBrushColor);
                }
            }
        }
    }
}
