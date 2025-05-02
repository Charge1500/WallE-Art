using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; 
using SFB;
using System.IO;
using Interprete;
public class JugarManager : MonoBehaviour
{
    [Header("Level Buttons")]
    [SerializeField] private GameObject[] levelsButtonsObjects;
    [SerializeField] private Button[] levelsButtons;
    [SerializeField] private TMP_Text[] levelsButtonsTMP;
    [SerializeField] private List<Texture2D> levels;
    [SerializeField] private int levelsIndex = 0;

    [Header("Auxiliar Level Buttons")]
    [SerializeField] private Button nextLevelsButton;
    [SerializeField] private GameObject nextLevelsButtonObject;
    [SerializeField] private Button previousLevelsButton;
    [SerializeField] private GameObject previousLevelsButtonObject;

    [Header("Create Level")]
    [SerializeField] private CanvasController canvasController; 
    [SerializeField] private GameObject addLevelPanel;
    [SerializeField] private Button cancelLevelPanel;
    [SerializeField] private Button addLevel;
    [SerializeField] private Button findFile;
    [SerializeField] private Button editor;
    [SerializeField] private TMP_InputField address;
    [SerializeField] private TMP_InputField info;
    [SerializeField] private string code;
    [SerializeField] private TMP_InputField size;

    void Start(){
        nextLevelsButton.onClick.AddListener(Next);
        previousLevelsButton.onClick.AddListener(Previous);
        cancelLevelPanel.onClick.AddListener(Cancel);
        addLevel.onClick.AddListener(AddLevel);
        findFile.onClick.AddListener(FindFile);
        editor.onClick.AddListener(Editor);
        UpdateLevelButtons();
    }

    public void Next(){
        levelsIndex += levelsButtons.Length;
        UpdateLevelButtons();
    }

    public void Previous(){
        levelsIndex -= levelsButtons.Length;
        UpdateLevelButtons();
    }

    public void UpdateLevelButtons(){
        int arrayIndex = 0;
        for (int i = levelsIndex; i < levelsIndex + levelsButtons.Length; i++)
        {
            levelsButtons[arrayIndex].onClick.RemoveAllListeners();
            if(!levelsButtonsObjects[arrayIndex].activeSelf)  levelsButtonsObjects[arrayIndex].SetActive(true);

            if(i >= levels.Count){
                levelsButtonsTMP[arrayIndex].text= "+";
                levelsButtons[arrayIndex].onClick.AddListener(AddLevelScreen);
                DesactivateRestingButtons(arrayIndex+1);
                break;
            }
            levelsButtonsTMP[arrayIndex].text = $"{i + 1}";
            for (int buttonIndex = 0; buttonIndex < levelsButtons.Length; buttonIndex++)
            {   
                int capturedButtonIndex = buttonIndex;
                levelsButtons[buttonIndex].onClick.AddListener(() => StartLevel(capturedButtonIndex));   
            }
            arrayIndex++;
        }

        nextLevelsButtonObject.SetActive(((levelsIndex + levelsButtons.Length) > levels.Count) ? false : true);
        previousLevelsButtonObject.SetActive(((levelsIndex - levelsButtons.Length) < 0) ? false : true);
    }

    public void DesactivateRestingButtons(int n){
        for (int i = n; i < levelsButtonsObjects.Length; i++)
        {
            levelsButtonsObjects[i].SetActive(false);
        }
    }

    public void AddLevelScreen(){
        addLevelPanel.SetActive(true);
    }
    
    public void Cancel(){
        addLevelPanel.SetActive(false);
    }

    public void AddLevel(){
        Clean();
        if(!string.IsNullOrEmpty(code)){ 
            Lexer lexer = new Lexer();
            List<Token> tokens = lexer.Tokenize(code);

            Parser parser = new Parser(tokens);
            ProgramNode astRoot = parser.Parse();

            if (astRoot == null)
            {
                ShowError("Parsing failed. Check editor for details."); 
                return;
            }
            
            if(!int.TryParse(size.text, out int sizeValue) || sizeValue < 8 || sizeValue > 256) 
            {
                ShowError("Invalid Canvas Size (must be between 8 and 256).");
                return;
            }

            canvasController.InitializeCanvas(sizeValue);
            Texture2D newTexture = canvasController.GetCanvasTexture();
            Interpreter interpreter = new Interpreter(newTexture);
            Texture2D texture = interpreter.Interpret(astRoot); 
            
            if(interpreter.errors.Count!=0){ShowError("Execution failed. Check editor for details.");return;}
            if(!Validate(texture)){ShowError("The texture must have a yellow\nsquare 2x2 and at least one pink pixel");return;}

            levels.Add(texture); 
            UpdateLevelButtons();
            addLevelPanel.SetActive(false);

        } else {
            ShowError("No code to process.");
        }
    }

    public void FindFile()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Cargar archivo", "", "pw", false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            code = File.ReadAllText(paths[0]);
            LevelLoader.Instance.SetEditorText(code);
            address.text = paths[0];
            info.text = "Archivo cargado con éxito";
        }
        else
        {
            info.text = "Carga cancelada";
        }
    }

    public void StartLevel(int buttonIndex){
        int levelIndex = levelsIndex + buttonIndex;
        LevelLoader.Instance.SetLevel(levels[levelIndex]); 
        SceneManager.LoadScene("Nivel");
    }
    public void Editor(){
        SceneManager.LoadScene("Editor");
    }
    public void Regresar(){
        SceneManager.LoadScene("Menu");
    }

    public void ShowStatus(string message)
    {
        info.text += "\n" + "<color=white>"+message+"</color>";
    }

    public void ShowError(string message)
    {
        info.text += "\n" + "<color=red>"+message+"</color>"; 
    }
    public void Clean(){
        info.text="";
    }

    public bool Validate(Texture2D tex)
    {
        Color pink = new Color(1.0f, 0.4f, 0.7f);
        Color yellow  = Color.yellow;

        bool square = false;
        bool foundPink = false;
        int yellowCount = 0;

        for (int y = 0; y < tex.width; y++)
        {
            for (int x = 0; x < tex.width; x++)
            {
                Color c = tex.GetPixel(x, y);
                if (ColorsApprox(c,pink)) foundPink = true;

                if (c==yellow && x < tex.width-1 && y < tex.width-1)
                {
                    yellowCount++;
                    if(yellowCount==1) {
                        square = tex.GetPixel(x+1, y)==yellow && tex.GetPixel(x+1, y+1)==yellow && tex.GetPixel(x, y+1)==yellow;
                        if(square){
                            LevelLoader.Instance.SetWallePos((x,y)); 
                        }
                    }
                    if(yellowCount>4) return false;
                }
                
            }
        }
        if (!foundPink) return false;
        if(square) return true;
        return false;
    }
    bool ColorsApprox(Color a, Color b, float tol = 0.01f) {
    return Mathf.Abs(a.r - b.r) < tol
        && Mathf.Abs(a.g - b.g) < tol
        && Mathf.Abs(a.b - b.b) < tol;
}
}
