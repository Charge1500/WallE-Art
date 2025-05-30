using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
public class LevelManager : MonoBehaviour
{
    [SerializeField] public Texture2D levelToLoad;
    [SerializeField] public (int,int) walleSpawn;
    [Header("UI")]
    [SerializeField] private Button[] buttons;
    [SerializeField] public GameObject[] screens;
    [Header("Configuración")]
    [SerializeField] public CinemachineCamera cinemachineCamera;
    [SerializeField] private CinemachineConfiner2D confiner;
    [SerializeField] private TextureToTile textureToTile;
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;

    void Awake(){
        textureToTile = GetComponent<TextureToTile>();
        levelToLoad = LevelLoader.Instance.level;
        walleSpawn = LevelLoader.Instance.wallePos;
    }
    void Start()
    {
        //pause buttons
        buttons[0].onClick.AddListener(Resume);
        buttons[1].onClick.AddListener(Restart);
        buttons[2].onClick.AddListener(Menu);
        buttons[3].onClick.AddListener(Quit);
        //game over screen buttons
        buttons[4].onClick.AddListener(Restart);
        buttons[5].onClick.AddListener(Menu);

        cinemachineCamera.Follow = textureToTile.wallePrefab.transform;
        confiner.BoundingShape2D = textureToTile.polyCollider;
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
        Time.timeScale = 1f;
        SceneManager.LoadScene("Nivel");
    }
    public void Menu(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void Quit(){
        Application.Quit();
    }

    
}