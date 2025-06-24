using System;
using Interprete;
public class CodeException : System.Exception
{
    public Token Token { get; }

    public CodeException(TypeError error,string message, Token token)
        : base($"[Line {token.Line}:{token.Column}] {error} Error: {message}")
    {
        Token = token;
    }
    public CodeException(TypeError error,string message)
        : base($"{error} Error: {message}")
    {
        Token = default;
    }
}

public enum TypeError
{
    Parse,
    Semantic,
    Execution
}
