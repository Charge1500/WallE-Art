using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; 
using SFB;
using System.IO;
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
            if(!levelsButtonsObjects[arrayIndex].activeSelf)  levelsButtonsObjects[arrayIndex].SetActive(true);

            if(i >= levels.Count){
                levelsButtonsTMP[arrayIndex].text= "+";
                levelsButtons[arrayIndex].onClick.AddListener(AddLevelScreen);
                DesactivateRestingButtons(arrayIndex+1);
                break;
            }
            levelsButtonsTMP[arrayIndex].text = $"{i + 1}";
            levelsButtons[arrayIndex].onClick.AddListener(() => StartLevel(levelsButtonsTMP[arrayIndex].text));
            arrayIndex++;
        }

        nextLevelsButtonObject.SetActive(((levelsIndex + levelsButtons.Length) > levels.Count) ? false : true);
        previousLevelsButtonObject.SetActive(((levelsIndex - levelsButtons.Length) <= 0) ? false : true);
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

    }

    public void FindFile()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Cargar archivo", "", "pw", false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            code = File.ReadAllText(paths[0]);
            address.text = paths[0];
            info.text = "Archivo cargado con éxito";
        }
        else
        {
            info.text = "Carga cancelada";
        }
    }

    public void StartLevel(string level){
        int.TryParse(level, out int parsedLevel);
        int levelIndex = parsedLevel - 1;
        LevelLoader.Instance.SetLevel(levels[levelIndex]); 
        SceneManager.LoadScene("Nivel");
    }
    public void Editor(){
        SceneManager.LoadScene("Editor");
    }
    public void Regresar(){
        SceneManager.LoadScene("Menu");
    }

}
