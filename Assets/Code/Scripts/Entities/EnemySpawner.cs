using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject instantiatedEnemy;

    public Action<GameObject> OnSpawn;

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

            if (SpawnCheckIfBoss())
            {
                
                return;
            }
            
            Debug.Log("spawning enemy" + enemyPrefab);
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            CircleCollider2D cc = enemy.GetComponent<CircleCollider2D>();
            
            if (cc != null) cc.enabled = true;
            instantiatedEnemy = enemy;
            
            OnSpawn?.Invoke(instantiatedEnemy);
            if (WorldAIManager.instance != null)
                WorldAIManager.instance.AddEnemyToEnemiesList(enemy);
        }
        else
        {
            Debug.LogWarning("Enemy prefab is not assigned in the EnemySpawner.");
        }
    }

    private bool SpawnCheckIfBoss()
    {
        if (enemyPrefab.TryGetComponent<BossData>(out BossData bossData))
        {
            if (EventFlagsSystem.instance.IsEventDone(bossData.bossFlag))
            {
                return true;
            }
            return false;
        }
        return false;
    }
}
