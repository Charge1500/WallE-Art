using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interprete{
    public class Parser
    {
        private readonly List<Token> _tokens;
        private int _current = 0;
        private bool _spawn = false;
        public List<string> errors = new List<string>();

        public Parser(List<Token> tokens)
        {
            _tokens = tokens ?? new List<Token>();
            _current = 0;
        }

        public ProgramNode Parse()
        {
            ProgramNode program = new ProgramNode();
            while (!IsAtEnd())
            {
                try
                {
                    while(Check(TokenType.EndOfLine) && !IsAtEnd()) {
                        Advance();
                    }
                    if (IsAtEnd()) break;
                    if(!_spawn)CheckFirstStatementIsSpawn();
                    StatementNode statement = ParseStatement();

                    if (statement != null) program.Statements.Add(statement);

                    ConsumeEndOfLineOrFile();
                }
                catch (ParseException ex)
                {
                    errors.Add(ex.Message);
                    Synchronize();
                }
                catch (Exception ex)
                {
                    Token errorToken = IsAtEnd() ? Previous() : Peek();
                    errors.Add($"[Line {errorToken.Line}:{errorToken.Column}] Unexpected Parser Error: {ex.GetType().Name} - {ex.Message}");
                    break;
                }
            }

            return program;
        }

        private StatementNode ParseStatement()
        {
            Token currentToken = Peek();

            if (Check(TokenType.Identifier) && CheckNext(TokenType.AssignmentOperator))
            {
                return ParseAssignment();
            }
            else if (FunctionRegistry.IsCommand(currentToken.Type))
            {
                return ParseCommand();
            }
            else if (Check(TokenType.GoToKeyword))
            {
                return ParseGoTo();
            }
            else if (Check(TokenType.Identifier) && (CheckNext(TokenType.EndOfLine) || CheckNext(TokenType.EndOfFile)))
            {
                return ParseLabel();
            }
            else if (Check(TokenType.EndOfFile))
            {
                return null;
            }
            else
            {
                throw new ParseException($"Unexpected token. Expected a command, assignment, label, or GoTo.", currentToken);
            }
        }

        private AssignmentNode ParseAssignment()
        {
            Token name = Consume(TokenType.Identifier, "Expected variable name for assignment.");
            Consume(TokenType.AssignmentOperator, "Expected '<-' after variable name.");
            ExpressionNode value = ParseExpression();
            return new AssignmentNode(name, value);
        }

        private LabelNode ParseLabel()
        {
            Token label = Consume(TokenType.Identifier, "Expected label name.");
            return new LabelNode(label);
        }


        private GoToNode ParseGoTo()
        {
            Consume(TokenType.GoToKeyword, "Expected 'GoTo'.");
            Consume(TokenType.LeftBracket, "Expected '[' after GoTo.");
            Token label = Consume(TokenType.Identifier, "Expected label name inside brackets [].");
            Consume(TokenType.RightBracket, "Expected ']' after label name.");
            Consume(TokenType.LeftParen, "Expected '(' for GoTo condition.");
            ExpressionNode condition = ParseExpression();
            Consume(TokenType.RightParen, "Expected ')' after GoTo condition.");
            return new GoToNode(label, condition);
        }

        private CommandNode ParseCommand()
        {
            Token commandToken = Advance();
            (Token token,List<ExpressionNode> args) = ParseArgumentList(commandToken);

            return new CommandNode(token, args);
        }
        private FunctionCallNode ParseFunctionCall()
        {
            Token functionToken = Advance();
            (Token token,List<ExpressionNode> args) = ParseArgumentList(functionToken);

            return new FunctionCallNode(token, args);
        }

        public (Token, List<ExpressionNode>) ParseArgumentList(Token keywordToken){
            Consume(TokenType.LeftParen, $"Expected '(' after function name '{keywordToken.Value}'.");

            FunctionDefinition def = FunctionRegistry.Get(keywordToken.Type);

            int expectedArgs = def.Arity;
            List<ExpressionNode> args = new List<ExpressionNode>();

            if (!Check(TokenType.RightParen))
            {
                if (expectedArgs == 0)
                {
                    throw new ParseException($"Function '{keywordToken.Value}' does not take any arguments.", Peek());
                }
                do
                {
                    args.Add(ParseExpression());
                } while (Match(TokenType.Comma));
            }
            
            Consume(TokenType.RightParen, $"Expected ')' or ',' after argument list for function '{keywordToken.Value}'.");

            if (args.Count != expectedArgs)
            {
                throw new ParseException($"'{keywordToken.Value}' expects {expectedArgs} arguments, but got {args.Count}.", keywordToken);
            }

            return (keywordToken, args);
        }

        private ExpressionNode ParseExpression()
        {
            return ParseLogicalOr();
        }

        private ExpressionNode ParseLogicalOr()
        {
            ExpressionNode expr = ParseLogicalAnd();
            while (Match(TokenType.OrOperator))
            {
                Token op = Previous();
                ExpressionNode right = ParseLogicalAnd();
                expr = new BinaryOpNode(expr, op, right);
            }
            return expr;
        }

        private ExpressionNode ParseLogicalAnd()
        {
            ExpressionNode expr = ParseEquality();
            while (Match(TokenType.AndOperator))
            {
                Token op = Previous();
                ExpressionNode right = ParseEquality();
                expr = new BinaryOpNode(expr, op, right);
            }
            return expr;
        }
        private ExpressionNode ParseEquality()
        {
            ExpressionNode expr = ParseComparison();
            if (Match(TokenType.EqualOperator, TokenType.NotEqualOperator))
            {
                Token op = Previous();
                ExpressionNode right = ParseComparison();
                expr = new BinaryOpNode(expr, op, right);
            }
            return expr;
        }

        private ExpressionNode ParseComparison()
        {
            ExpressionNode expr = ParseTerm();
            if (Match(TokenType.GreaterOperator, TokenType.GreaterEqualOperator, TokenType.LessOperator, TokenType.LessEqualOperator))
            {
                Token op = Previous();
                ExpressionNode right = ParseTerm();
                expr = new BinaryOpNode(expr, op, right);
            }
            return expr;
        }

        private ExpressionNode ParseTerm()
        {
            ExpressionNode expr = ParseFactor();
            while (Match(TokenType.PlusOperator, TokenType.MinusOperator))
            {
                Token op = Previous();
                ExpressionNode right = ParseFactor();
                expr = new BinaryOpNode(expr, op, right);
            }
            return expr;
        }

        private ExpressionNode ParseFactor()
        {
            ExpressionNode expr = ParsePower();
            while (Match(TokenType.MultiplyOperator, TokenType.DivideOperator, TokenType.ModuloOperator))
            {
                Token op = Previous();
                ExpressionNode right = ParsePower();
                expr = new BinaryOpNode(expr, op, right);
            }
            return expr;
        }

        private ExpressionNode ParsePower()
        {
            ExpressionNode expr = ParseUnary();
            if (Match(TokenType.PowerOperator))
            {
                Token op = Previous();
                ExpressionNode right = ParsePower();
                expr = new BinaryOpNode(expr, op, right);
            }
            return expr;
        }

        private ExpressionNode ParseUnary()
        {
            if (Match(TokenType.NotOperator, TokenType.MinusOperator))
            {
                Token op = Previous();
                ExpressionNode right = ParseUnary();
                return new UnaryOpNode(op, right);
            }
            return ParsePrimary();
        }

        private ExpressionNode ParsePrimary()
        {
            if (Match(TokenType.NumberLiteral))
            {
                Token numToken = Previous();
                if (int.TryParse(numToken.Value, out int intValue))
                {
                    return new LiteralNode<int>(intValue, numToken);
                }
                else {
                    throw new ParseException($"Invalid integer number format: {numToken.Value}", numToken);
                }
            }

            if (Match(TokenType.StringLiteral))
            {
                Token strToken = Previous();
                return new LiteralNode<string>(strToken.Value, strToken);
            }

            if (Check(TokenType.Identifier))
            {
                Token identifierToken = Advance();
                return new VariableNode(identifierToken);
            }

            if (FunctionRegistry.IsFunction(Peek().Type))
            { 
                if (CheckNext(TokenType.LeftParen)) {
                    return ParseFunctionCall();
                } else {
                    throw new ParseException($"Expected '(' after function name '{Peek().Value}'.", Peek());
                }
            }

            if (Match(TokenType.LeftParen))
            {
                ExpressionNode expr = ParseExpression();
                Consume(TokenType.RightParen, "Expected ')' after expression in parentheses.");
                return expr;
            }
            throw new ParseException($"Expected expression (number, string, variable, function call, or parentheses).", Peek());
        }

        private bool IsAtEnd()
        {
            return Peek().Type == TokenType.EndOfFile;
        }

        private Token Peek()
        {
            if (_current >= _tokens.Count) return _tokens[_tokens.Count - 1];
            return _tokens[_current];
        }

        private Token PeekNext()
        {
            if (_current + 1 >= _tokens.Count) return _tokens[_tokens.Count - 1];
            return _tokens[_current + 1];
        }

        private Token Previous()
        {
            if (_current == 0)
            {
                if (_tokens.Count == 0) return new Token(TokenType.Unknown, "", 1, 1);
                
                return _tokens.FirstOrDefault(); 
            }
            return _tokens[_current - 1];
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private bool CheckNext(TokenType type)
        {
            if (IsAtEnd()) return false;
            if (_current + 1 >= _tokens.Count) return false; 
            return _tokens[_current + 1].Type == type;
        }

        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private Token Consume(TokenType type, string errorMessage)
        {
            if (Check(type)) return Advance();
            throw new ParseException(errorMessage, Peek());
        }

        private void ConsumeEndOfLineOrFile()
        {
            if (Check(TokenType.EndOfLine))
            {
                Advance();
                
                while (!IsAtEnd() && Check(TokenType.EndOfLine))
                {
                    Advance();
                }
            }
            else if (!IsAtEnd()) 
            {
                throw new ParseException("Expected newline after statement.", Peek());
            }
        }

        private void ValidateArgumentCount(Token commandOrFunction, List<ExpressionNode> args, int expectedCount)
        {
            if (args.Count != expectedCount)
            {
                throw new ParseException($"'{commandOrFunction.Value}' expects {expectedCount} arguments, but got {args.Count}.", commandOrFunction);
            }
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Peek().Type == TokenType.EndOfLine) {
                    Advance();
                    return; 
                }
                Advance();
            }
        }

        private void CheckFirstStatementIsSpawn()
        {
            _spawn = true;
            if (_tokens == null || _tokens.Count == 0 || _tokens[0].Type == TokenType.EndOfFile) {
                throw new ParseException("[Line 1:1] Parse Error: Source code cannot be empty. Must start with 'Spawn'.");
            }
            while(Check(TokenType.EndOfLine) && !IsAtEnd()) {
                Advance();
            }
            if (IsAtEnd()) throw new ParseException("Parse Error: Source code cannot be empty. Must start with 'Spawn'.");
            if(!Check(TokenType.SpawnKeyword)) throw new ParseException("Parse Error: Source must start with 'Spawn'.");
            return;       
        }
    }
}