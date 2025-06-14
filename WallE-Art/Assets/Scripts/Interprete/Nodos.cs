using System.Collections.Generic;
using System.Linq;

namespace Interprete
{
    public interface IAstVisitor<T>
    {

        T VisitLiteralNode<TValue>(LiteralNode<TValue> node); 
        T VisitVariableNode(VariableNode node);
        T VisitUnaryOpNode(UnaryOpNode node);
        T VisitBinaryOpNode(BinaryOpNode node);
        T VisitFunctionCallNode(FunctionCallNode node);
        T VisitCommandNode(CommandNode node);

        
        T VisitProgramNode(ProgramNode node);
        T VisitAssignmentNode(AssignmentNode node);
        T VisitGoToNode(GoToNode node);
        T VisitLabelNode(LabelNode node);
    }



    public abstract class AstNode
    {
        public abstract T Accept<T>(IAstVisitor<T> visitor);
    }

    public abstract class StatementNode : AstNode { }

    public abstract class ExpressionNode : AstNode { }

    public class LiteralNode<TValue> : ExpressionNode
    {
        public TValue Value { get; }
        public Token Token { get; }

        public LiteralNode(TValue value, Token token)
        {
            Value = value;
            Token = token;
        }
        public override string ToString() => $"{Value}";

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitLiteralNode(this); 
        }
    }

    public class VariableNode : ExpressionNode
    {
        public Token NameToken { get; }

        public VariableNode(Token nameToken)
        {
            NameToken = nameToken;
        }
        public override string ToString() => $"{NameToken.Value}";

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitVariableNode(this);
        }
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

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitUnaryOpNode(this);
        }
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

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitBinaryOpNode(this);
        }
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

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitFunctionCallNode(this);
        }
    }

    public class ProgramNode : AstNode
    {
        public List<StatementNode> Statements { get; } = new List<StatementNode>();
        public override string ToString() => $"Program({Statements.Count} statements)";

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitProgramNode(this);
        }
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

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitAssignmentNode(this);
        }
    }

    public class LabelNode : StatementNode
    {
        public Token LabelToken { get; }
        public LabelNode(Token labelToken)
        {
            LabelToken = labelToken;
        }
        public override string ToString() => $"Label: {LabelToken.Value}";

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitLabelNode(this); 
        }
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

        public override T Accept<T>(IAstVisitor<T> visitor)
        {
            return visitor.VisitGoToNode(this);
        }
    }

    public class CommandNode : StatementNode
    {
        public Token CommandToken { get; }
        public List<ExpressionNode> Arguments { get; }

        public CommandNode(Token commandToken, List<ExpressionNode> arguments)
        {
            CommandToken = commandToken;
            Arguments = arguments;
        }
        public override string ToString() => $"{CommandToken.Value}({string.Join(", ", Arguments)})";
        public override T Accept<T>(IAstVisitor<T> visitor) { return visitor.VisitCommandNode(this); }
    }
}