using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wave : MonoBehaviour
{
    public bool waveStarted;
    public bool waveEnded;
    public List<GameObject> enemies;


    public void SpawnEnemies(List<Transform> spawnPoints)
    {
        StartCoroutine(SpawnEnemiesWithDelay(spawnPoints));
    }
    
    private IEnumerator SpawnEnemiesWithDelay(List<Transform> spawnPoints)
    {
        waveStarted = true;
        foreach (GameObject enemy in enemies)
        {
            GameObject newEnemy = Instantiate(enemy, spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
            newEnemy.transform.parent = transform;
            newEnemy.GetComponentInChildren<EnemyAI>().state = EnemyAI.EnemyState.Chasing;

            yield return new WaitForSeconds(0.2f); // Delay of 0.1 seconds
        }
    }

    public void Update()
    {
        if (!waveStarted)
            return;
        if (transform.childCount == 0)
        {
            waveEnded = true;
        }
    }
}
