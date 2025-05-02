using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TextureToTile : MonoBehaviour
{
    public GameObject walle;
    [Header("Configuración")]
    public float cellSize = 0.5f;

    public Texture2D sourceTexture;
    public LevelManager levelManager;

    [Header("Tilemap")]
    public Tilemap tilemap;
    Dictionary<(int x, int y), int> tilePosition = new Dictionary<(int x, int y), int>();

    [Header("Mapeo de colores")]
    public List<TileBase> tiles = new List<TileBase>();
    private List<Color> colorOfTilesBlue = new List<Color>();
    private List<Color> colorOfTilesBlack = new List<Color>();
    public List<ColorToTile> colorTileMappings = new List<ColorToTile>();

    void Awake(){
        levelManager=GetComponent<LevelManager>();
        ColorOfTilesBlue();
        ColorOfTilesBlack();
        SetColorTiles();
    }
    void Start()
    {
        sourceTexture = levelManager.levelToLoad;
        GenerateColliderFromTexture();
        GenerateGridFromTexture();
        UpdateAllTiles(); 
        GenerateWalle();
    }
    
    public void GenerateColliderFromTexture()
    {
        float width = sourceTexture.width * cellSize;
        float height = sourceTexture.height * cellSize;
        Vector2[] points = new Vector2[5];
        points[0] = new Vector2(0, 0);         
        points[1] = new Vector2(width, 0);      
        points[2] = new Vector2(width, height); 
        points[3] = new Vector2(0, height);     
        points[4] = new Vector2(0, 0);

        EdgeCollider2D edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        
        edgeCollider.points = points;
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
            tilemap.RefreshTile(pos);
        }
    }

    TileBase GetTileFromColor(Color color,int x,int y)
    {
        foreach (ColorToTile mapping in colorTileMappings)
        {
            if (ColorApproximately(mapping.color, color))
            {
                tilePosition[(x,y)] = mapping.background;
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
        int x = 0;
        int y = 0;
        (x,y) = levelManager.walleSpawn;
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
    public int background;
    public ColorToTile(Color Color,TileBase Tile,int Background){
        color=Color;
        tile=Tile;
        background=Background;
    }
}