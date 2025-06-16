using System;
using System.Collections.Generic;

namespace Interprete
{
    public class SemanticAnalyzer : IAstVisitor<ValueType>
    {
        public readonly List<string> errors = new List<string>();
        private readonly SymbolTable _symbols = new SymbolTable();

        public void Analyze(ProgramNode program)
        {
            try
            {
                foreach (StatementNode statement in program.Statements)
                {
                    if (statement is LabelNode labelNode)
                    {
                        labelNode.Accept(this);
                    }
                }
            }
            catch(SemanticException ex) { errors.Add(ex.Message); }

            foreach (StatementNode statement in program.Statements)
            {
                if (statement is LabelNode) continue;

                try
                {
                    statement.Accept(this);
                }
                catch (SemanticException ex) { errors.Add(ex.Message); }
                catch (Exception ex) { errors.Add($"Unexpected Analyzer Error: {ex.GetType().Name} - {ex.Message}"); }
            }
        }

        public ValueType VisitProgramNode(ProgramNode node)
        {
            return ValueType.Void;
        }

        public ValueType VisitLabelNode(LabelNode node)
        {
            _symbols.DefineLabel(node.LabelToken);
            return ValueType.Void;
        }
        
        public ValueType VisitAssignmentNode(AssignmentNode node)
        {
            string variableName = node.VariableNameToken.Value;
            ValueType valueType = node.ValueExpression.Accept(this);

            if (valueType == ValueType.Void)
            {
                throw new SemanticException($"Cannot assign a void value to a variable.", node.VariableNameToken);
            }
            
            if (_symbols.IsVariableDefined(variableName))
            {
                ValueType existingType = _symbols.GetVariableType(node.VariableNameToken);
                if (existingType != valueType)
                {
                    throw new SemanticException($"Cannot assign a value of type {valueType} to variable '{variableName}' which is of type {existingType}.", node.VariableNameToken);
                }
            }
            else
            {
                _symbols.DefineVariable(variableName, valueType, node.VariableNameToken);
            }

            return ValueType.Void;
        }

        public ValueType VisitGoToNode(GoToNode node)
        {
            _symbols.CheckLabelExists(node.TargetLabelToken);

            ValueType conditionType = node.Condition.Accept(this);
            if (conditionType != ValueType.Boolean)
            {
                throw new SemanticException("GoTo condition must evaluate to a boolean.", node.TargetLabelToken);
            }
            return ValueType.Void;
        }

        public ValueType VisitLiteralNode<T>(LiteralNode<T> node)
        {
            if (node.Value is int) return ValueType.Number;
            if (node.Value is string) return ValueType.String;
            if (node.Value is bool) return ValueType.Boolean;
            throw new SemanticException($"Unknown literal type: {node.Value.GetType().Name}", node.Token);
        }
        
        public ValueType VisitVariableNode(VariableNode node)
        {
            return _symbols.GetVariableType(node.NameToken);
        }

        public ValueType VisitUnaryOpNode(UnaryOpNode node)
        {
            ValueType operandType = node.Right.Accept(this);

            switch (node.OperatorToken.Type)
            {
                case TokenType.MinusOperator:
                    if (operandType != ValueType.Number)
                        throw new SemanticException($"Unary '-' operator can only be applied to numbers, not {operandType}.", node.OperatorToken);
                    return ValueType.Number;
                case TokenType.NotOperator:
                    if (operandType != ValueType.Boolean)
                        throw new SemanticException($"'!' operator can only be applied to booleans, not {operandType}.", node.OperatorToken);
                    return ValueType.Boolean;
            }
            throw new SemanticException($"Invalid unary operator '{node.OperatorToken.Value}'.", node.OperatorToken);
        }
        
        public ValueType VisitBinaryOpNode(BinaryOpNode node)
        {
            ValueType leftType = node.Left.Accept(this);
            ValueType rightType = node.Right.Accept(this);

            switch (node.OperatorToken.Type)
            {
                case TokenType.PlusOperator: case TokenType.MinusOperator: case TokenType.MultiplyOperator:
                case TokenType.DivideOperator: case TokenType.ModuloOperator: case TokenType.PowerOperator:
                    if (leftType != ValueType.Number || rightType != ValueType.Number)
                        throw new SemanticException($"Operator '{node.OperatorToken.Value}' can only be used with numbers.", node.OperatorToken);
                    return ValueType.Number;

                case TokenType.GreaterOperator: case TokenType.GreaterEqualOperator:
                case TokenType.LessOperator: case TokenType.LessEqualOperator:
                    if (leftType != ValueType.Number || rightType != ValueType.Number)
                        throw new SemanticException($"Operator '{node.OperatorToken.Value}' can only compare numbers.", node.OperatorToken);
                    return ValueType.Boolean;

                case TokenType.EqualOperator: case TokenType.NotEqualOperator:
                    if (leftType != rightType)
                         throw new SemanticException($"Cannot compare values of different types: {leftType} and {rightType}.", node.OperatorToken);
                    return ValueType.Boolean;
                
                case TokenType.AndOperator: case TokenType.OrOperator:
                    if (leftType != ValueType.Boolean || rightType != ValueType.Boolean)
                        throw new SemanticException($"Operator '{node.OperatorToken.Value}' can only be used with booleans.", node.OperatorToken);
                    return ValueType.Boolean;
            }
            throw new SemanticException($"Invalid binary operator '{node.OperatorToken.Value}'.", node.OperatorToken);
        }

        public ValueType VisitCommandNode(CommandNode node)
        {
            FunctionDefinition def = FunctionRegistry.Get(node.CommandToken.Type);
            CheckArgumentTypes(node.CommandToken, node.Arguments, def.ArgumentTypes);
            
            return ValueType.Void;
        }

        public ValueType VisitFunctionCallNode(FunctionCallNode node)
        {
            FunctionDefinition def = FunctionRegistry.Get(node.FunctionNameToken.Type);
            CheckArgumentTypes(node.FunctionNameToken, node.Arguments, def.ArgumentTypes);
            
            return def.ReturnType;
        }
        
        private void CheckArgumentCount(Token token, List<ExpressionNode> args, int expected)
        {
            if (args.Count != expected)
            {
                throw new SemanticException($"'{token.Value}' expects {expected} arguments, but got {args.Count}.", token);
            }
        }

        private void CheckArgumentTypes(Token token, List<ExpressionNode> args, params ValueType[] expectedTypes)
        {
            CheckArgumentCount(token, args, expectedTypes.Length);
            for (int i = 0; i < expectedTypes.Length; i++)
            {
                ValueType actualType = args[i].Accept(this);
                if (actualType != expectedTypes[i])
                {
                    throw new SemanticException($"Argument {i + 1} for '{token.Value}' should be of type {expectedTypes[i]}, but got {actualType}.", token);
                }
            }
        }
    }
}
