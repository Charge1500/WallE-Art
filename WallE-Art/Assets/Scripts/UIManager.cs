using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro; 
using Interprete;
public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RawImage canvasDisplayImage; 
    [SerializeField] private TMP_InputField sizeInput;    
    [SerializeField] private TMP_InputField codeEditorInput;
    [SerializeField] private TMP_InputField statusTextEditor;
    [SerializeField] private Button resizeButton;     
    [SerializeField] private Button loadButton;       
    [SerializeField] private Button saveButton;       
    [SerializeField] private Button executeButton;    
    [SerializeField] private Button menuButton;    
    [SerializeField] private Button cleanButton;    
    [SerializeField] private Toggle showCodeEditor;    
    [SerializeField] private Toggle showErrorArea;    
    [SerializeField] private Toggle showBook;    
    [SerializeField] private TMP_Text statusText;

    [Header("UI Elements")]
    [SerializeField] private GameObject codeEditor;      
    [SerializeField] private GameObject errorArea;      
    [SerializeField] private GameObject book;      

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
        cleanButton.onClick.AddListener(Clean);
        showCodeEditor.onValueChanged.AddListener(ActiveCodeEditor);
        showErrorArea.onValueChanged.AddListener(ActivateErrorArea);
        showBook.onValueChanged.AddListener(ActiveBook);

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
        string sourceCode = codeEditorInput.text;
        ExecuteCode(sourceCode);
    }
    public void Menu(){
        SceneManager.LoadScene("Menu");
    }
    public void Clean(){
        statusTextEditor.text="";
    }

    public void ActiveCodeEditor(bool activate){
        codeEditor.SetActive(activate);
    }
    public void ActivateErrorArea(bool activate){
        errorArea.SetActive(activate);
    }
    public void ActiveBook(bool activate){
        book.SetActive(activate);
    }
    public void ShowStatus(string message)
    {
        statusTextEditor.text += "\n" + "<color=white>"+message+"</color>";
        statusText.color = Color.white;  
        
    }

    public void ShowError(string message)
    {
        statusTextEditor.text += "\n" + "<color=red>"+message+"</color>";
        statusText.color = Color.red; 
    }

    private void ExecuteCode(string sourceCode)
    {
        Clean(); 
        ShowStatus("--- Starting Execution ---");

        ShowStatus("--- Lexer Phase ---");
        Lexer lexer = new Lexer();
        List<Token> tokens = lexer.Tokenize(sourceCode);

        if (tokens != null)
        {
            foreach (Token token in tokens)
            {
                ShowStatus(token.ToString());
            }
        }
        else
        {
            ShowStatus("(No tokens generated)");
        }

        if(lexer.errors != null){
            foreach (string error in lexer.errors)
            {
                ShowError(error);
            }
        }

        ShowStatus("--- Parser Phase ---");
        Parser parser = new Parser(tokens); 
        ProgramNode astRoot = parser.Parse(); 

        if (parser.errors != null)
        {
            foreach (string error in parser.errors)
            {
                ShowError(error);
            }
        }

        if (astRoot.Statements != null)
        {
            astRoot.Statements.ToString();
        }
        else
        {
            ShowStatus("Parser finished, but no AST generated (or empty program).");
        }
    }

}