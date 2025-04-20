using UnityEngine;
public class LevelManager : MonoBehaviour
{
    public Texture2D levelToLoad;
    private void Start()
    {
        levelToLoad = LevelLoader.Instance.level;
    }

}