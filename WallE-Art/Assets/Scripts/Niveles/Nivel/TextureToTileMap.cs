using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TextureToTile : MonoBehaviour
{
    [Header("Referencias de Objetos")]
    [SerializeField] public GameObject wallePrefab; 
    [SerializeField] public bool walleInstantiated = false; 

    [Header("Configuración General")]
    [SerializeField] private float cellSize = 0.5f;
    [SerializeField] private Texture2D sourceTexture;
    [SerializeField] public PolygonCollider2D polyCollider;

    [Header("Colliders")]
    [SerializeField] private GameObject cineMachineConfiner;

    [Header("Tilemap Principal")]
    [SerializeField] private Tilemap terrainTilemap;

    private Dictionary<(int x, int y), int> tileBackgroundInfo = new Dictionary<(int x, int y), int>();
    
    private List<(int x, int y)> positionsRequiringBackgroundFill = new List<(int x, int y)>();
    
    private HashSet<(int x, int y)> prefabInstantiatedPositions = new HashSet<(int x, int y)>();

    [Header("Mapeo de Colores a Tiles de Terreno")]
    [SerializeField] private List<ColorToTile> colorToTerrainTileMappings = new List<ColorToTile>();

    [Header("Mapeo de Colores a Prefabs")]
    [SerializeField] private List<ColorToPrefabMapping> colorToPrefabMappings = new List<ColorToPrefabMapping>();

    [Header("Tiles de Fondo Específicos")]
    [SerializeField] private TileBase blueBackgroundTile;
    [SerializeField] private TileBase blackBackgroundTile;

    void Awake()
    {
        sourceTexture = LevelLoader.Instance.level;
        //ClearLevelState();
        GenerateColliderFromTexture();
        ProcessTextureAndPlaceElements();
        FillBackgroundTileAreas();
        terrainTilemap.RefreshAllTiles();
        //wallePrefab.GetComponent<Player>().DetectEnemies();
    }

    /* void ClearLevelState()
    {
        terrainTilemap.ClearAllTiles();
        tileBackgroundInfo.Clear();
        positionsRequiringBackgroundFill.Clear();
        prefabInstantiatedPositions.Clear();

        // Destruir prefabs generados previamente que son hijos de este objeto
        foreach (Transform child in transform) {
            // Considera usar un tag específico si tienes otros hijos que no deben ser destruidos
            Destroy(child.gameObject);
        }
    } */
    
    public void GenerateColliderFromTexture()
    {
        float width = sourceTexture.width * cellSize;
        float height = sourceTexture.height * cellSize;

        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(0, 0);          
        points[1] = new Vector2(width, 0);      
        points[2] = new Vector2(width, height); 
        points[3] = new Vector2(0, height);     
        polyCollider = cineMachineConfiner.AddComponent<PolygonCollider2D>();
        polyCollider.isTrigger = true;
        polyCollider.SetPath(0, points); 
        
        Vector2[] points2 = new Vector2[5];
        points2[0] = new Vector2(0, 0);         
        points2[1] = new Vector2(width, 0);      
        points2[2] = new Vector2(width, height); 
        points2[3] = new Vector2(0, height);     
        points2[4] = new Vector2(0, 0);
        EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.points = points2;

    }

    void ProcessTextureAndPlaceElements()
    { 
        int walleX = LevelLoader.Instance.wallePos.Item1;
        int walleY = LevelLoader.Instance.wallePos.Item2;

        List<(int x, int y)> wallePosList = new List<(int x, int y)>();
        wallePosList.Add((walleX+1,walleY));
        wallePosList.Add((walleX,walleY+1));
        wallePosList.Add((walleX+1,walleY+1));

        InstantiateSpecialPrefab(wallePrefab, walleX, walleY,wallePosList);      

        for (int y = 0; y < sourceTexture.height; y++)
        {
            for (int x = 0; x < sourceTexture.width; x++)
            {
                if (prefabInstantiatedPositions.Contains((x,y)))
                {
                    continue;
                }

                Color pixelColor = sourceTexture.GetPixel(x, y);
                bool processedPixel = false;

                if (!prefabInstantiatedPositions.Contains((x,y)))
                {
                    foreach (ColorToPrefabMapping prefabMapping in colorToPrefabMappings)
                    {
                        if (ColorApproximately(prefabMapping.color, pixelColor))
                        {
                            InstantiatePrefab(prefabMapping.prefab, x, y);
                            processedPixel = true;
                            break; 
                        }
                    }
                }

                if (!processedPixel && !prefabInstantiatedPositions.Contains((x,y)))
                {
                    foreach (ColorToTile terrainMapping in colorToTerrainTileMappings)
                    {
                        if (ColorApproximately(terrainMapping.color, pixelColor))
                        {
                            PlaceTerrainTile(terrainMapping.tile, x, y, terrainMapping.backgroundType);
                            processedPixel = true;
                            break;
                        }
                    }
                }

                // Si el píxel no es un prefab ni un tile de terreno,
                
                /* if (!processedPixel && !prefabInstantiatedPositions.Contains((x,y)))
                {
                    MarkAsRequiringBackgroundFill(x, y);
                } */
            }
        }
    }

    void InstantiatePrefab(GameObject prefab, int x, int y)
    {
        Vector3 position = new Vector3(x * cellSize + (cellSize * 0.5f), y * cellSize + (cellSize * 0.5f), 0);
        Instantiate(prefab, position, Quaternion.identity, this.transform);
        
        prefabInstantiatedPositions.Add((x, y));
        tileBackgroundInfo[(x, y)] = -1;
        if (!positionsRequiringBackgroundFill.Contains((x, y)))
        {
            positionsRequiringBackgroundFill.Add((x, y));
        }
    }
    void InstantiateSpecialPrefab(GameObject prefab, int x, int y,List<(int,int)> positionsToAdd)
    {
        foreach ((int x,int y) item in positionsToAdd)
        {
            tileBackgroundInfo[(item.x, item.y)] = -1;
            if (!positionsRequiringBackgroundFill.Contains((item.x, item.y))) {
                positionsRequiringBackgroundFill.Add((item.x, item.y));
            }
        }
        if(!walleInstantiated){
            Vector3 position = new Vector3(x * cellSize + (cellSize * 0.5f), y * cellSize + (cellSize * 0.5f), 0);
            GameObject walleInstance = Instantiate(prefab, position, Quaternion.identity, this.transform);
            wallePrefab = walleInstance;

            prefabInstantiatedPositions.Add((x, y));
            tileBackgroundInfo[(x, y)] = -1;
            if (!positionsRequiringBackgroundFill.Contains((x, y)))
            {
                positionsRequiringBackgroundFill.Add((x, y));
            }
            walleInstantiated = true;
            return;
        }
        InstantiatePrefab(prefab, x, y);
    }

    void PlaceTerrainTile(TileBase tile, int x, int y, int backgroundType)
    {
        Vector3Int tilePos = new Vector3Int(x, y, 0);
        terrainTilemap.SetTile(tilePos, tile);
        tileBackgroundInfo[(x, y)] = backgroundType; 
        if(backgroundType == -1){
            if (!positionsRequiringBackgroundFill.Contains((x, y)))
            {
                positionsRequiringBackgroundFill.Add((x, y));
            }
        }
    }

    void MarkAsRequiringBackgroundFill(int x, int y)
    {
        tileBackgroundInfo[(x, y)] = -1; 
        if (!positionsRequiringBackgroundFill.Contains((x, y)))
        {
            positionsRequiringBackgroundFill.Add((x, y));
        }
    }
    
    void FillBackgroundTileAreas()
    {
        foreach ((int x, int y) coord in positionsRequiringBackgroundFill)
        {
            AssignBackgroundTileForPosition(coord.x, coord.y);
        }
    }

    void AssignBackgroundTileForPosition(int startX, int startY)
    {
        Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
        HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
        (int, int)[] dirs = { 
            (0, 1), (1, 1), (1, 0), (1, -1), 
            (0, -1), (-1, -1), (-1, 0), (-1, 1),
        };

        queue.Enqueue((startX, startY));
        visited.Add((startX, startY));

        int determinedBackgroundType = -1; 

        int searchLimit = sourceTexture.width * sourceTexture.height; 
        int iterations = 0;

        while (queue.Count > 0 && iterations < searchLimit)
        {
            iterations++;
            (int curX, int curY) = queue.Dequeue();

            if (tileBackgroundInfo.TryGetValue((curX, curY), out int neighborBgType) && neighborBgType != -1)
            {
                determinedBackgroundType = neighborBgType;
                break; 
            }

            foreach ((int dx, int dy) in dirs)
            {
                int nx = curX + dx;
                int ny = curY + dy;

                if (nx >= 0 && nx < sourceTexture.width && ny >= 0 && ny < sourceTexture.height && !visited.Contains((nx, ny)))
                {
                    visited.Add((nx, ny));
                    queue.Enqueue((nx, ny));
                }
            }
        }

        if (determinedBackgroundType == -1)
        {
            determinedBackgroundType = 1;
        }
        
        TileBase backgroundTileToSet = (determinedBackgroundType == 0) ? blueBackgroundTile : blackBackgroundTile;
        Vector3Int originalPos = new Vector3Int(startX, startY, 0);
        terrainTilemap.SetTile(originalPos, backgroundTileToSet);
        
        tileBackgroundInfo[(startX, startY)] = determinedBackgroundType;
    }

    bool ColorApproximately(Color a, Color b, float tolerance = 0.01f)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}

[System.Serializable]
public class ColorToPrefabMapping
{
    public string description;
    public string colorName;
    public Color color;
    public GameObject prefab;
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
}