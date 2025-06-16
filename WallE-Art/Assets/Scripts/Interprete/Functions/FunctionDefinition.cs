namespace Interprete
{
    public enum FunctionCategory { Command, Function }

    public class FunctionDefinition
    {
        public string Name { get; }
        public TokenType KeywordToken { get; }
        public FunctionCategory Category { get; }
        public ValueType ReturnType { get; }
        public ValueType[] ArgumentTypes { get; }

        public int Arity => ArgumentTypes.Length;

        public FunctionDefinition(string name, TokenType keywordToken, FunctionCategory category, ValueType returnType, params ValueType[] argumentTypes)
        {
            Name = name;
            KeywordToken = keywordToken;
            Category = category;
            ReturnType = returnType;
            ArgumentTypes = argumentTypes ?? new ValueType[0];
        }
    }
}