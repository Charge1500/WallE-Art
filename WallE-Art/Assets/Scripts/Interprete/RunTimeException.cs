using System;
using Interprete;
public class RuntimeException : System.Exception
{
    public Token Token { get; }

    public RuntimeException(string message, Token token)
        : base($"[Line {token.Line}:{token.Column}] Runtime Error: {message}")
    {
        Token = token;
    }
    public RuntimeException(string message)
        : base($"Runtime Error: {message}")
    {
        Token = default;
    }
}
