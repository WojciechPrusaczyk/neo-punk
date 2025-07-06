using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    public string mainMenuSceneName;

    public void GotToScene()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
