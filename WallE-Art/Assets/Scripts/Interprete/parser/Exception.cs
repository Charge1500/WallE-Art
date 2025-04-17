using System;

namespace Interprete{
    public class ParseException : System.Exception
    {
        public Token Token { get; }

        public ParseException(string message, Token token)
            : base($"[Line {token.Line}:{token.Column}] Parse Error: {message} (Found token: {token.Type} '{token.Value}')")
        {
            Token = token;
        }
        public ParseException(string message)
            : base($"Parse Error: {message}")
        {
            Token = default;
        }
    }
}