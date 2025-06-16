using UnityEngine;

namespace Interprete
{
    public class InterpreterHelpers
    {
        public static bool TryParseColor(string colorName, LevelThemeData levelTheme, out Color result)
        {
            if (string.IsNullOrWhiteSpace(colorName)){
                result = Color.clear;
                return false;
            }
            

            string lowerColorName = colorName.ToLowerInvariant();

            foreach (ColorToTile terrainMapping in levelTheme.colorToTerrainTileMappings)
            {
                if (terrainMapping.colorName.ToLowerInvariant() == lowerColorName)
                {
                    result = terrainMapping.color;
                    return true;
                }
            }

            foreach (ColorToPrefabMapping prefabMapping in levelTheme.colorToPrefabMappings)
            {
                if (prefabMapping.colorName.ToLowerInvariant() == lowerColorName)
                {
                    result = prefabMapping.color;
                    return true;
                }
            }

            switch (lowerColorName)
            {
                case "transparent": result = Color.clear; return true;
                case "blue": result = Color.blue; return true;
                case "darkblue": result = new Color(0.0f, 0.0f, 0.5f); return true;
                case "darkred": result = new Color(0.5f, 0.0f, 0.0f); return true;
                case "purple": result = new Color(0.5f, 0.0f, 0.5f); return true;
                case "green": result = Color.green; return true;
                case "red": result = Color.red; return true;
                case "yellow": result = Color.yellow; return true;
                case "orange": result = new Color(1.0f, 0.5f, 0.0f); return true;
                case "brown": result = new Color(0.6f, 0.3f, 0.1f); return true;
                case "gray": result = new Color(0.5f, 0.5f, 0.5f); return true;
                case "black": result = Color.black; return true;
                case "white": result = Color.white; return true;
                case "pink": result = new Color(1.0f, 0.4f, 0.7f); return true;
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
}
