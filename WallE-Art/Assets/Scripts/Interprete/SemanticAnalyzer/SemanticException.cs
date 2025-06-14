using System;

namespace Interprete
{
    public class SemanticException : System.Exception
    {
        public Token Token { get; }

        public SemanticException(string message, Token token)
            : base($"[Line {token.Line}:{token.Column}] Semantic Error: {message}")
        {
            Token = token;
        }

        public SemanticException(string message)
            : base($"Semantic Error: {message}")
        {
            Token = default;
        }
    }
}
