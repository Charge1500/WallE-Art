using UnityEngine;
using System.IO;
using SFB;
using TMPro;

public class FileManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    public string LoadScriptFromFile()
    {
        string path = OpenFilePanel();

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                string content = File.ReadAllText(path);
                Debug.Log($"File loaded successfully from: {path}");
                return content;
            }
            catch (IOException ex)
            {
                Debug.LogError($"Error loading file: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An unexpected error occurred while loading file: {ex.Message}");
            }
        }
        else
        {
            Debug.Log("File load cancelled or path was invalid.");
        }
        return null;
    }

    public bool SaveScriptToFile()
    {
        string path = SaveFilePanel();

        if (!string.IsNullOrEmpty(path))
        {
            try
            {
                File.WriteAllText(path, inputField.text);
                Debug.Log($"File saved successfully to: {path}");
                return true;
            }
            catch (IOException ex)
            {
                Debug.LogError($"Error saving file: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"An unexpected error occurred while saving file: {ex.Message}");
            }
        }
        else
        {
            Debug.Log("File save cancelled or path was invalid.");
        }
        return false;
    }

    private string OpenFilePanel()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Cargar archivo", "", "pw", false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string content = File.ReadAllText(paths[0]);
            inputField.text = content;
            Debug.Log("Archivo cargado desde: " + paths[0]);
            return paths[0];
        }
        else
        {
            Debug.Log("Carga cancelada.");
        }

        return null;
    }

    private string SaveFilePanel()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Guardar archivo", "", "archivo", "pw");

        if (!string.IsNullOrEmpty(path))
        {
            string content = inputField.text;
            File.WriteAllText(path, content);
            Debug.Log("Archivo guardado en: " + path);
            return path;
        }
        else
        {
            Debug.Log("Guardado cancelado.");
        }

        return null;
    }
}