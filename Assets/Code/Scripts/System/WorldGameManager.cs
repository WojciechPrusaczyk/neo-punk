using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldGameManager : MonoBehaviour
{
    public static WorldGameManager instance;
    // This script will store the most important references
    [Header("Entities")]
    public Player player;

    string prefix = "WORLD GAME MANAGER || ";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FindData());
    }

    private void FindDataInScene()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            player = null;
            return;
        }

        player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError($"{prefix} Player not found in the scene!");
        }
    }
    private IEnumerator FindData()
    {
        yield return new WaitForEndOfFrame();
        FindDataInScene();
    }
}
