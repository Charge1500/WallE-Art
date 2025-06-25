using UnityEngine;

public class CanvasController : MonoBehaviour 
{
    private Texture2D canvasTexture;
    private int currentSize;

    public void InitializeCanvas(int size)
    {
        if (canvasTexture != null)
        {
            Destroy(canvasTexture);
        }

        canvasTexture = new Texture2D(size, size);
        currentSize = size;


        // El filtrado de puntos previene el desenfoque al escalar
        canvasTexture.filterMode = FilterMode.Point;
        // Clamp wrap mode prevents texture edges from bleeding
        canvasTexture.wrapMode = TextureWrapMode.Clamp;

        Color[] initialPixels = new Color[currentSize * currentSize];

        for (int i = 0; i < initialPixels.Length; i++)
        {
            initialPixels[i] = Color.white;
        }

        canvasTexture.SetPixels(initialPixels);
        canvasTexture.Apply();
    }

    public Texture2D GetCanvasTexture()
    {
        return canvasTexture;
    }

    public int GetCurrentSize()
    {
        return currentSize;
    }
}