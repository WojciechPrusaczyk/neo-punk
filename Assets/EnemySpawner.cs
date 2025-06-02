using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject instantiatedEnemy;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            if (instantiatedEnemy != null)
            {
                Destroy(instantiatedEnemy);
            }

            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            instantiatedEnemy = enemy;

            if (WorldAIManager.instance != null)
                WorldAIManager.instance.AddEnemyToEnemiesList(enemy);
        }
        else
        {
            Debug.LogWarning("Enemy prefab is not assigned in the EnemySpawner.");
        }
    }
}
