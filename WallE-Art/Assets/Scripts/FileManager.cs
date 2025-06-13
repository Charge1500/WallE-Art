using UnityEngine;
using System.IO;
using SFB;
using TMPro;

public class FileManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_InputField compiler;

    public string LoadScriptFromFile()
    {
        string path = OpenFilePanel();

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                string content = File.ReadAllText(path);
                return content;
            }
            catch (IOException ex)
            {
                compiler.text = $"Error loading file: {ex.Message}";
            }
            catch (System.Exception ex)
            {
                compiler.text = $"An unexpected error occurred while loading file: {ex.Message}";
            }
        }
        return null;
    }

    public bool SaveScriptToFile()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Guardar archivo", "", "archivo", "pw");

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                File.WriteAllText(path, inputField.text);
                return true;
            }
            catch (IOException ex)
            {
                compiler.text = $"Error saving file: {ex.Message}";
            }
            catch (System.Exception ex)
            {
                compiler.text = $"An unexpected error occurred while saving file: {ex.Message}";
            }
        }
        return false;
    }
    
     private string OpenFilePanel()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Cargar archivo", "", "pw", false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string content = File.ReadAllText(paths[0]);
            return paths[0];
        }

        return null;
    }

    public bool SaveTextureAsJPG(Texture2D texture)
    {

        byte[] bytes = texture.EncodeToJPG();
        string path = StandaloneFileBrowser.SaveFilePanel("Guardar imagen", "", "imagen", "jpg");

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                File.WriteAllBytes(path, bytes);
                return true;
            }
            catch (IOException ex)
            {
                compiler.text = $"Error saving image: {ex.Message}";
            }
            catch (System.Exception ex)
            {
                compiler.text = $"An unexpected error occurred while saving image: {ex.Message}";
            }
        }

        return false;
    }
}