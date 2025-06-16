using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro; 
public class MenuManager : MonoBehaviour
{
    void Start(){
        MusicManager.Instance.PlayMusic(MusicManager.Instance.music[0]);
    }
    public void Salir(){
        Application.Quit();
    }

    public void Editor(){
        SceneManager.LoadScene("Editor");
    }

    public void Jugar(){
        SceneManager.LoadScene("Jugar");
    }
    
    public void Menu(){
        SceneManager.LoadScene("Menu");
    }

}
