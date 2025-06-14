using UnityEngine;
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
