using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TextureToTile : MonoBehaviour
{
    [Header("Configuración")]
    public Texture2D sourceTexture;
    public float tileSize = 1f;

    [Header("Tilemap")]
    public Tilemap tilemap;

    [Header("Mapeo de colores")]
    public List<ColorToTile> colorTileMappings = new List<ColorToTile>();

    void Start()
    {
        GenerateGridFromTexture();
        UpdateAllTiles(); 
    }

    void GenerateGridFromTexture()
    {
        tilemap.ClearAllTiles();

        for (int y = 0; y < sourceTexture.height; y++)
        {
            for (int x = 0; x < sourceTexture.width; x++)
            {
                Color pixelColor = sourceTexture.GetPixel(x, y);
                TileBase tile = GetTileFromColor(pixelColor);

                if (tile != null)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    tilemap.SetTile(tilePosition, tile); 
                }
            }
        }
    }


    void UpdateAllTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile is RuleTile ruleTile)
            {
                tilemap.RefreshTile(pos);
            }
        }
    }

    TileBase GetTileFromColor(Color color)
    {
        foreach (ColorToTile mapping in colorTileMappings)
        {
            if (ColorApproximately(mapping.color, color))
            {
                return mapping.tile; // Ahora usa TileBase (RuleTile)
            }
        }
        return null;
    }

    bool ColorApproximately(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}

[System.Serializable]
public class ColorToTile
{
    public Color color;
    public TileBase tile;
}