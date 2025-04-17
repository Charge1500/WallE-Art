using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public void Salir(){
        Application.Quit();
    }

    public void Editor(){
        SceneManager.LoadScene("Editor");
    }

    public void Jugar(){
        SceneManager.LoadScene("Jugar");
    }
}
