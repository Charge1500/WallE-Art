using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelManager : MonoBehaviour
{
    [SerializeField] public Texture2D levelToLoad;
    [SerializeField] public (int,int) walleSpawn;
    [Header("UI")]
    [SerializeField] private Button[] buttons;
    [Header("Configuración")]
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;

    void Awake(){
        levelToLoad = LevelLoader.Instance.level;
        walleSpawn = LevelLoader.Instance.wallePos;
    }
    void Start()
    {
        buttons[0].onClick.AddListener(Resume);
        buttons[1].onClick.AddListener(Restart);
        buttons[2].onClick.AddListener(Menu);
        buttons[3].onClick.AddListener(Quit);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }
    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart(){
        SceneManager.LoadScene("Nivel");
    }
    public void Menu(){
        SceneManager.LoadScene("Menu");
    }
    public void Quit(){
        Application.Quit();
    }

    
}