using System.Collections.Generic;

namespace Interprete
{
    public class SymbolTable
    {
        private readonly Dictionary<string, ValueType> _variableTypes = new Dictionary<string, ValueType>();
        private readonly Dictionary<string, Token> _labels = new Dictionary<string, Token>();

        public void DefineVariable(string name, ValueType type, Token token)
        {
            _variableTypes[name] = type;
        }

        public ValueType GetVariableType(Token nameToken)
        {
            if (_variableTypes.TryGetValue(nameToken.Value, out ValueType type))
            {
                return type;
            }
            throw new CodeException(TypeError.Semantic,$"Variable '{nameToken.Value}' is not defined in the current scope.", nameToken);
        }

        public bool IsVariableDefined(string name)
        {
            return _variableTypes.ContainsKey(name);
        }

        public void DefineLabel(Token labelToken)
        {
            if (_labels.ContainsKey(labelToken.Value))
            {
                throw new CodeException(TypeError.Semantic,$"Duplicate label definition: '{labelToken.Value}'.", labelToken);
            }
            _labels.Add(labelToken.Value, labelToken);
        }

        public void CheckLabelExists(Token labelToken)
        {
            if (!_labels.ContainsKey(labelToken.Value))
            {
                throw new CodeException(TypeError.Semantic,$"Undefined label: '{labelToken.Value}'.", labelToken);
            }
        }
    }
}
