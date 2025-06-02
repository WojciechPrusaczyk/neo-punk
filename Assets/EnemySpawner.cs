using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    private void Awake()
    {
        // Ensure the spawner is inactive at the start
        gameObject.SetActive(false);

        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        if (gameObject.activeInHierarchy == false)
            gameObject.SetActive(true);

        if (enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Enemy prefab is not assigned in the EnemySpawner.");
        }
    }
}
