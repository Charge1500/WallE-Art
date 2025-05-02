using System;
using UnityEngine; 
using Interprete;
public static class InterpreterHelpers
{
    public static int ExpectNumber(object value, Token contextToken, string description)
    {
        if (value is int i) return i;
        if (value is double d)
        {
            // Debug.LogWarning($"Implicit conversion from double ({d}) to int for {description} at line {contextToken.Line}.");
            try
            {
                return Convert.ToInt32(d);
            }
            catch (OverflowException)
            {
                throw new RuntimeException($"Number '{d}' is too large or small for an integer.", contextToken);
            }
        }
        throw new RuntimeException($"Expected {description} to be a number, but got {value?.GetType().Name ?? "null"}.", contextToken);
    }

    public static bool ExpectBoolean(object value, Token contextToken, string description)
    {
        if (value is bool b) return b;
        if (value is int i) return i != 0;
        throw new RuntimeException($"Expected {description} to be a boolean (true/false or 0/non-0), but got {value?.GetType().Name ?? "null"}.", contextToken);
    }

    public static string ExpectString(object value, Token contextToken, string description)
    {
        if (value is string s) return s;
        throw new RuntimeException($"Expected {description} to be a string (like \"Red\"), but got {value?.GetType().Name ?? "null"}.", contextToken);
    }

    public static bool TryParseColor(string colorName, out Color result)
    {
        if (string.IsNullOrWhiteSpace(colorName))
        {
            result = Color.clear;
            return false;
        }
        string lowerColor = colorName.ToLowerInvariant();
        switch (lowerColor)
        {
            case "blue": result = Color.blue; return true;
            case "darkblue": result = new Color(0.0f, 0.0f, 0.5f); return true; 
            case "darkred": result = new Color(0.5f, 0.0f, 0.0f); return true; 
            case "steelblue": result = new Color(0.1f, 0.2f, 0.3f); return true; 
            case "cyan": result = new Color(0f, 1.0f, 1.0f); return true; 
            case "purple": result = new Color(0.5f, 0.0f, 0.5f); return true; 
            case "green": result = Color.green; return true;
            case "red": result = Color.red; return true;
            case "yellow": result = Color.yellow; return true;
            case "orange": result = new Color(1.0f, 0.5f, 0.0f); return true; 
            case "brown": result = new Color(0.6f, 0.3f, 0.1f); return true; 
            case "coral": result = new Color(1.0f, 0.5f, 0.3f); return true; 
            case "burgundy": result = new Color(0.4f, 0.0f, 0.1f); return true; 
            case "gray": result = new Color(0.5f, 0.5f, 0.5f); return true; 
            case "black": result = Color.black; return true;
            case "turquoise": result = new Color(0.25f, 0.88f, 0.82f); return true; 
            case "lime": result = new Color(0.75f, 1.0f, 0.0f); return true; 
            case "white": result = Color.white; return true;
            case "pink": result = new Color(1.0f, 0.4f, 0.7f); return true;
            case "transparent": result = Color.clear; return true;
            default:
                result = Color.clear; return false;
        }
    }
    public static bool ColorsApproximatelyEqual(Color c1, Color c2, float tolerance = 0.01f)
    {
        return Mathf.Abs(c1.r - c2.r) < tolerance &&
               Mathf.Abs(c1.g - c2.g) < tolerance &&
               Mathf.Abs(c1.b - c2.b) < tolerance &&
               Mathf.Abs(c1.a - c2.a) < tolerance;
    }
}
