using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TextureToTile : MonoBehaviour
{
    [SerializeField] public GameObject walle;
    [Header("Configuración")]
    [SerializeField] private float cellSize = 0.5f;

    [SerializeField] private Texture2D sourceTexture;
    [SerializeField] public PolygonCollider2D  polyCollider;
    [SerializeField] private GameObject  cineMachineConfiner;

    [Header("Tilemap")]
    [SerializeField] Tilemap tilemap;
    [SerializeField] Dictionary<(int x, int y), int> tilePosition = new Dictionary<(int x, int y), int>();
    [SerializeField] private List<(int x, int y)> keysToProcess= new List<(int x, int y)>();
    [Header("Mapeo de colores")]
    [SerializeField] private List<TileBase> tiles = new List<TileBase>();
    private List<Color> colorOfTilesBlue = new List<Color>();
    private List<Color> colorOfTilesBlack = new List<Color>();
    [SerializeField] private List<ColorToTile> colorTileMappings = new List<ColorToTile>();
    [SerializeField] private TileBase blueBackgroundTile;
    [SerializeField] private TileBase blackBackgroundTile;

    void Awake(){
        sourceTexture = LevelLoader.Instance.level;
        ColorOfTilesBlue();
        ColorOfTilesBlack();
        SetColorTiles();
        blueBackgroundTile=tiles[0];
        blackBackgroundTile=tiles[tiles.Count-1];
        GenerateWalle();
    }
    void Start()
    {
        GenerateColliderFromTexture();
        GenerateGridFromTexture();
        GenerateObjectsTiles();
        UpdateAllTiles(); 
    }
    
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

    public void GenerateGridFromTexture()
    {
        tilemap.ClearAllTiles();

        for (int y = 0; y < sourceTexture.height; y++)
        {
            for (int x = 0; x < sourceTexture.width; x++)
            {
                Color pixelColor = sourceTexture.GetPixel(x, y);
                TileBase tile = GetTileFromColor(pixelColor,x,y);

                if (tile != null)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, 0);
                    tilemap.SetTile(tilePos, tile); 
                    continue;
                }
                tilePosition[(x,y)] = -1;
                keysToProcess.Add((x,y));
            }
        }
    }

    public void GenerateObjectsTiles(){
        foreach ((int x,int y) coordenada in keysToProcess)
        {
            if(tilePosition[coordenada]==-1) AssignBackgroundToObject(coordenada.x,coordenada.y);
        }
    }
    public void AssignBackgroundToObject(int startX, int startY)
    {
        Queue<(int x, int y)> queue = new Queue<(int x, int y)>();
        HashSet<(int x, int y)> visited = new HashSet<(int x, int y)>();
        (int, int)[] dirs = { (0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1), (-1, 0), (-1, 1) };

        queue.Enqueue((startX, startY));
        visited.Add((startX, startY));

        bool foundBackground = false;
        Vector3Int originalPos = new Vector3Int(startX, startY, 0);

        while (queue.Count > 0)
        {
            (int x,int y) current = queue.Dequeue();
            int cx = current.x;
            int cy = current.y;

            if (tilePosition.TryGetValue((cx, cy), out int backgroundType) && backgroundType != -1)
            {
                TileBase backgroundTileToSet = (backgroundType == 0) ? blueBackgroundTile : blackBackgroundTile;

                tilemap.SetTile(originalPos, backgroundTileToSet);

                tilePosition[(startX, startY)] = backgroundType;

                foundBackground = true;
                break;
            }

            foreach ((int dx, int dy) in dirs)
            {
                int nx = cx + dx;
                int ny = cy + dy;

                if (nx >= 0 && nx < sourceTexture.width && ny >= 0 && ny < sourceTexture.height && !visited.Contains((nx, ny)))
                {
                    visited.Add((nx, ny));
                    queue.Enqueue((nx, ny));
                }
            }
        }

        if (!foundBackground)
        {
            tilemap.SetTile(originalPos, blackBackgroundTile);
            tilePosition[(startX, startY)] = 1;
        }
    }
    void UpdateAllTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            tilemap.RefreshTile(pos);
        }
    }

    TileBase GetTileFromColor(Color color,int x,int y)
    {
        foreach (ColorToTile mapping in colorTileMappings)
        {
            if (ColorApproximately(mapping.color, color))
            {
                tilePosition[(x,y)] = mapping.backgroundType;
                return mapping.tile;
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

    public void GenerateWalle(){
        int x = LevelLoader.Instance.wallePos.Item1;
        int y = LevelLoader.Instance.wallePos.Item2;
        GameObject walleInstance = Instantiate(walle, new Vector3(x *0.5f + 0.5f, y*0.5f + 0.5f, 0), Quaternion.identity);
        walle = walleInstance;
    }
    public void SetColorTiles(){
        int i=0;
        foreach (Color color in colorOfTilesBlue)
        {
            colorTileMappings.Add(new ColorToTile(color,tiles[i],0));    
            i++;
        }
        foreach (Color color in colorOfTilesBlack)
        {
            colorTileMappings.Add(new ColorToTile(color,tiles[i],1));    
            i++;
        }
        
    }
    public void ColorOfTilesBlue(){
        //-----------------BlueBackGround------------------
        colorOfTilesBlue.Add(Color.white);
        colorOfTilesBlue.Add(Color.green);
        colorOfTilesBlue.Add(new Color(1.0f, 0.5f, 0.3f));//coral
        colorOfTilesBlue.Add(new Color(0.75f, 1.0f, 0.0f));//lime
        colorOfTilesBlue.Add(new Color(1.0f, 0.5f, 0.0f));//orange
        colorOfTilesBlue.Add(new Color(0.4f, 0.0f, 0.1f));//burgundy
        colorOfTilesBlue.Add(new Color(1.0f, 0.4f, 0.7f));//pink
    }
    public void ColorOfTilesBlack(){
        //-----------------BlackBackGround------------------
        colorOfTilesBlack.Add(new Color(0.5f, 0.0f, 0.0f));//darkred
        colorOfTilesBlack.Add(new Color(0.5f, 0.5f, 0.5f));//gray
        colorOfTilesBlack.Add(Color.red);
        colorOfTilesBlack.Add(new Color(0.5f, 0.0f, 0.5f));//purple
        colorOfTilesBlack.Add(new Color(0.25f, 0.88f, 0.82f));//turquoise
        colorOfTilesBlack.Add(new Color(0f, 1.0f, 1.0f));//cyan
        colorOfTilesBlack.Add(Color.blue);
        colorOfTilesBlack.Add(new Color(0.0f, 0.0f, 0.5f));//darkblue
        colorOfTilesBlack.Add(new Color(0.1f, 0.2f, 0.3f));//steelblue
        colorOfTilesBlack.Add(new Color(0.6f, 0.3f, 0.1f));//brown
        colorOfTilesBlack.Add(Color.black);
    }
}

[System.Serializable]
public class ColorToTile
{
    public Color color;
    public TileBase tile;
    public int backgroundType;
    public ColorToTile(Color Color,TileBase Tile,int Background){
        color=Color;
        tile=Tile;
        backgroundType=Background;
    }
}