using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    public List<EnemySpawner> enemySpawners;
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        enemies.Clear();
        enemySpawners.Clear();

        if (SceneManager.GetActiveScene().name == "MainMenu")
            return;

        InitializeAIForScene(scene);
    }

    private void InitializeAIForScene(Scene scene)
    {
        RecalculateLists(true, true);
        RespawnAllEnemies();
    }

    public void RecalculateLists(bool _enemies = false, bool _spawners = false)
    {
        if (_enemies)
        {
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
        if (_spawners)
        {
            enemySpawners.Clear();
            foreach (EnemySpawner spawner in FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
            {
                if (!enemySpawners.Contains(spawner))
                {
                    enemySpawners.Add(spawner);
                }
            }
        }
    }

    public void RespawnAllEnemies()
    {
        enemies.Clear();

        foreach (EnemySpawner spawner in enemySpawners)
        {
            if (spawner != null)
            {
                spawner.SpawnEnemy();
            }
        }
    }

    public void AddEnemyToEnemiesList(GameObject enemy)
    {
        if (enemy != null && !enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void RemoveEnemyFromEnemiesList(GameObject enemy)
    {
        if (enemy != null && enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
