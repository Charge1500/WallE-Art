using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Interprete;
public partial class Interpreter : IAstVisitor<object> 
{
    private int max_execution_steps = int.MaxValue;
    private int _executedSteps = 0;

    private Scope scope = new Scope();
    private Texture2D texture; 

    private int _walleX;
    private int _walleY;
    private Color _currentBrushColor = Color.clear; 
    private int _currentBrushSize = 1; 
    public List<string> errors = new List<string>();

    private int _programCounter = 0; 
    private bool _goToExecuted = false; 

    private ProgramNode _programAst;

    public Interpreter(Texture2D canvasController)
    {
        texture = canvasController;
    }

    public Texture2D Interpret(ProgramNode program)
    {
        _programAst = program ?? throw new ArgumentNullException(nameof(program));
        _programCounter = 0;
        _executedSteps = 0;
        _currentBrushColor = Color.clear; 
        _currentBrushSize = 1;
        scope = new Scope();
        

        try
        {
            FindLabels(program);

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
            //Debug.Log("Interpretation finished successfully.");
            texture.Apply();
            return texture;
        }
        catch (RuntimeException ex) 
        {
            errors.Add(ex.Message);
        }
         catch (Exception ex) 
        {
            errors.Add(ex.Message);
        }
        return null;
    }

    private void Execute(StatementNode node)
    {   
        node.Accept(this);
    }

    private object Evaluate(ExpressionNode node)
    {
        return node.Accept(this);
    }

    private void FindLabels(ProgramNode program)
    {
        for (int i = 0; i < program.Statements.Count; i++)
        {
            if (program.Statements[i] is LabelNode labelNode)
            {
                scope.DefineLabel(labelNode.LabelToken, i);
            }
        }
    }

    private void DrawPixelWithBrush(int cx, int cy)
    {
        if (_currentBrushColor == Color.clear) return;

        int canvasSize = texture.width;
        int halfBrush = _currentBrushSize / 2; 

        for (int offsetY = -halfBrush; offsetY <= halfBrush; offsetY++)
        {
            for (int offsetX = -halfBrush; offsetX <= halfBrush; offsetX++)
            {
                int px = cx + offsetX;
                int py = cy + offsetY;
                if (px >= 0 && px < canvasSize && py >= 0 && py < canvasSize)
                {
                    texture.SetPixel(px, py, _currentBrushColor);
                }
            }
        }
    }

}
