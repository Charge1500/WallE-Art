using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelTheme", menuName = "Pixel Wall-E/Level Theme Data")]
public class LevelThemeData : ScriptableObject
{
    [Header("Mapeo de Colores a Tiles de Terreno")]
    [Tooltip("Lista de colores y los tiles de terreno que les corresponden.")]
    public List<ColorToTile> colorToTerrainTileMappings;

    [Header("Mapeo de Colores a Prefabs")]
    [Tooltip("Lista de colores y los prefabs que les corresponden.")]
    public List<ColorToPrefabMapping> colorToPrefabMappings;
}

[System.Serializable]
public class ColorToPrefabMapping
{
    public string description;
    public string colorName;
    public Color color;
    public GameObject prefab;
    public Sprite[] displaySprites;
}

//tipo de área de fondo (0 para azul, 1 para negro)
[System.Serializable]
public class ColorToTile
{
    public string description;
    public string colorName;
    public Color color;
    public TileBase tile;
    public int backgroundType; 
    public Sprite[] displaySprites;
}