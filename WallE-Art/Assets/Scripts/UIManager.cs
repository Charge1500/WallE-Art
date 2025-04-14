using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RawImage canvasDisplayImage; 
    [SerializeField] private TMP_InputField sizeInput;    
    [SerializeField] private TMP_InputField codeEditorInput;
    [SerializeField] private Button resizeButton;     
    [SerializeField] private Button loadButton;       
    [SerializeField] private Button saveButton;       
    [SerializeField] private Button executeButton;    
    [SerializeField] private Button menuButton;    
    [SerializeField] private TMP_Text statusText;       

    [Header("Logic Controllers")]
    [SerializeField] private CanvasController canvasController; 
    [SerializeField] private FileManager fileManager;

    [Header("Configuration")]
    [SerializeField] private int defaultCanvasSize = 64;
    [SerializeField] private int minCanvasSize = 8;      
    [SerializeField] private int maxCanvasSize = 256;    

    void Start()
    {
        
        canvasController.InitializeCanvas(defaultCanvasSize);
        canvasDisplayImage.texture = canvasController.GetCanvasTexture();
        sizeInput.text = defaultCanvasSize.ToString();
            
        resizeButton.onClick.AddListener(OnResizeButtonPressed);
        loadButton.onClick.AddListener(OnLoadButtonPressed);
        saveButton.onClick.AddListener(OnSaveButtonPressed);
        executeButton.onClick.AddListener(OnExecuteButtonPressed);
        menuButton.onClick.AddListener(Menu);

        ShowStatus("Canvas inicializado.");
    }

    private void OnResizeButtonPressed()
    {
        if (int.TryParse(sizeInput.text, out int newSize))
        {
            if (newSize >= minCanvasSize && newSize <= maxCanvasSize)
            {
                canvasController.InitializeCanvas(newSize);
                canvasDisplayImage.texture = canvasController.GetCanvasTexture();
                ShowStatus($"Canvas redimensionado a {newSize}x{newSize}");
            }
            else
            {
                ShowError($"Error: El tamaño debe estar entre {minCanvasSize} y {maxCanvasSize}.");
            }
        }
        else
        {
            ShowError("Error: Entrada de tamaño inválida. Por favor, introduce un número.");
        }
    }
    private void OnLoadButtonPressed()
    {
        string loadedCode = fileManager.LoadScriptFromFile();
        if (loadedCode != null) {
            codeEditorInput.text = loadedCode;
            ShowStatus("Archivo cargado correctamente.");
        } else {
            ShowError("Fallo al cargar el archivo o cancelado.");
        }
    }

    private void OnSaveButtonPressed()
    {
        bool saved = fileManager.SaveScriptToFile();
        if (saved) {
           ShowStatus("Archivo guardado correctamente.");
        } else {
           ShowError("Fallo al guardar el archivo o cancelado.");
        }
    }

    private void OnExecuteButtonPressed()
    {
        
    }
    public void Menu(){
        SceneManager.LoadScene("Menu");
    }
    private void ShowStatus(string message)
    {
        statusText.text = message;
        statusText.color = Color.black;  
        
    }

    private void ShowError(string message)
    {
        statusText.text = message;
        statusText.color = Color.red; 
    }
}