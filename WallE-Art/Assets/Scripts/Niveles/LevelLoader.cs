using UnityEngine;
public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance; 
    public Texture2D level { get; private set; }
    public string editorText { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLevel(Texture2D nivel)
    {
        level = nivel;
    }
    public void SetEditorText(string text)
    {
        editorText = text;
    }
}