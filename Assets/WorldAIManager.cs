using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public WorldAIManager instance;

    public List<GameObject> enemies;

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

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            enemies.Clear();
            return;
        }

        if (SceneManager.GetActiveScene().isLoaded)
        {
            InitializeAIForScene(SceneManager.GetActiveScene());
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeAIForScene(scene);
    }

    private void InitializeAIForScene(Scene scene)
    {
        // Each EnemyAI is children of main enemy GameObject
        enemies.Clear();
        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            GameObject enemyParent = enemy.gameObject.transform.parent != null ? enemy.gameObject.transform.parent.gameObject : enemy.gameObject;
            if (!enemies.Contains(enemyParent))
            {
                enemies.Add(enemyParent);
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
