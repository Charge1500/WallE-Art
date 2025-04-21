using System.Collections.Generic;
using UnityEngine;
using Interprete;
public class Scope
{
    private readonly Dictionary<string, object> _variables = new Dictionary<string, object>();

    private readonly Dictionary<string, int> _labels = new Dictionary<string, int>();
    public void DefineVariable(string name, object value)
    {
        _variables[name] = value;
        // Debug.Log($"Variable defined/updated: {name} = {value}");
    }

    public object GetVariable(Token nameToken)
    {
        if (_variables.TryGetValue(nameToken.Value, out object value))
        {
            return value;
        }
        throw new RuntimeException($"Undefined variable '{nameToken.Value}'.", nameToken);
    }

    public bool IsVariableDefined(string name)
    {
        return _variables.ContainsKey(name);
    }
    public void DefineLabel(Token labelToken, int statementIndex)
    {
        if (_labels.ContainsKey(labelToken.Value))
        {
            throw new RuntimeException($"Duplicate label definition: '{labelToken.Value}'.", labelToken);
        }
        _labels[labelToken.Value] = statementIndex;
         // Debug.Log($"Label defined: {labelToken.Value} at index {statementIndex}");
    }

    public int ResolveLabel(Token labelToken)
    {
        if (_labels.TryGetValue(labelToken.Value, out int index))
        {
            return index;
        }
        throw new RuntimeException($"Undefined label: '{labelToken.Value}'.", labelToken);
    }
}
