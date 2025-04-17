using System.Collections.Generic;
using System.Linq; 

namespace Interprete{
    public abstract class AstNode
    {
        // public Token TokenInfo { get; protected set; } // Ejemplo
    }

    public abstract class StatementNode : AstNode { }

    public abstract class ExpressionNode : AstNode { }

    public class LiteralNode<T> : ExpressionNode
    {
        public T Value { get; }
        public Token Token { get; }

        public LiteralNode(T value, Token token)
        {
            Value = value;
            Token = token;
        }
        public override string ToString() => $"{Value}";
    }

    public class VariableNode : ExpressionNode
    {
        public Token NameToken { get; }

        public VariableNode(Token nameToken)
        {
            NameToken = nameToken;
        }
        public override string ToString() => $"{NameToken.Value}";
    }

    public class UnaryOpNode : ExpressionNode
    {
        public Token OperatorToken { get; }
        public ExpressionNode Right { get; }

        public UnaryOpNode(Token operatorToken, ExpressionNode right)
        {
            OperatorToken = operatorToken;
            Right = right;
        }
        public override string ToString() => $"({OperatorToken.Value}{Right})";
    }

    public class BinaryOpNode : ExpressionNode
    {
        public ExpressionNode Left { get; }
        public Token OperatorToken { get; }
        public ExpressionNode Right { get; }

        public BinaryOpNode(ExpressionNode left, Token operatorToken, ExpressionNode right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }
        public override string ToString() => $"({Left} {OperatorToken.Value} {Right})";
    }

    public class FunctionCallNode : ExpressionNode
    {
        public Token FunctionNameToken { get; }
        public List<ExpressionNode> Arguments { get; }

        public FunctionCallNode(Token functionNameToken, List<ExpressionNode> arguments)
        {
            FunctionNameToken = functionNameToken;
            Arguments = arguments;
        }
        public override string ToString() => $"{FunctionNameToken.Value}({string.Join(", ", Arguments)})";
    }

    public class ProgramNode : AstNode
    {
        public List<StatementNode> Statements { get; } = new List<StatementNode>();
        public override string ToString() => $"Program({Statements.Count} statements)";
    }

    public class AssignmentNode : StatementNode
    {
        public Token VariableNameToken { get; }
        public ExpressionNode ValueExpression { get; }

        public AssignmentNode(Token variableNameToken, ExpressionNode valueExpression)
        {
            VariableNameToken = variableNameToken;
            ValueExpression = valueExpression;
        }
        public override string ToString() => $"{VariableNameToken.Value} <- {ValueExpression}";
    }

    public class LabelNode : StatementNode
    {
        public Token LabelToken { get; } // El IDENTIFIER que actúa como etiqueta

        public LabelNode(Token labelToken)
        {
            LabelToken = labelToken;
        }
        public override string ToString() => $"Label: {LabelToken.Value}";
    }

    public class GoToNode : StatementNode
    {
        public Token TargetLabelToken { get; } 
        public ExpressionNode Condition { get; }

        public GoToNode(Token targetLabelToken, ExpressionNode condition)
        {
            TargetLabelToken = targetLabelToken;
            Condition = condition;
        }
        public override string ToString() => $"GoTo [{TargetLabelToken.Value}] ({Condition})";
    }

    public abstract class CommandNode : StatementNode
    {
        public Token CommandToken { get; }
        public List<ExpressionNode> Arguments { get; }

        protected CommandNode(Token commandToken, List<ExpressionNode> arguments)
        {
            CommandToken = commandToken;
            Arguments = arguments;
        }
        public override string ToString() => $"{CommandToken.Value}({string.Join(", ", Arguments)})";
    }

    public class SpawnNode : CommandNode
    {
        public SpawnNode(Token commandToken, List<ExpressionNode> arguments) : base(commandToken, arguments) { }
    }

    public class ColorNode : CommandNode
    {
        public ColorNode(Token commandToken, List<ExpressionNode> arguments) : base(commandToken, arguments) { }
    }

    public class SizeNode : CommandNode
    {
        public SizeNode(Token commandToken, List<ExpressionNode> arguments) : base(commandToken, arguments) { }
    }

    public class DrawLineNode : CommandNode
    {
        public DrawLineNode(Token commandToken, List<ExpressionNode> arguments) : base(commandToken, arguments) { }
    }

    public class DrawCircleNode : CommandNode
    {
        public DrawCircleNode(Token commandToken, List<ExpressionNode> arguments) : base(commandToken, arguments) { }
    }

    public class DrawRectangleNode : CommandNode
    {
        public DrawRectangleNode(Token commandToken, List<ExpressionNode> arguments) : base(commandToken, arguments) { }
    }

    public class FillNode : CommandNode
    {
        public FillNode(Token commandToken) : base(commandToken, new List<ExpressionNode>()) { }
        public override string ToString() => $"{CommandToken.Value}()"; 
    }
}