using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSceneManager : MonoBehaviour
{
    public static WorldSceneManager instance;
    public bool changeScene = false;
    public int sceneIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    private void Update()
    {
        if (changeScene)
        {
            changeScene = false;
            LoadScene(sceneIndex);
        }
    }
}
