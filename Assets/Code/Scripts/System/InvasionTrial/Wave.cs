using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wave : MonoBehaviour
{
    public bool waveStarted;
    public bool waveEnded;
    public GameObject flame;

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
            Vector2 position = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
            GameObject flameOBJ = Instantiate(flame, position, Quaternion.identity);
            
            position.y += 2f;
            GameObject newEnemy = Instantiate(enemy, position, Quaternion.identity);
            newEnemy.transform.parent = transform;
            newEnemy.GetComponentInChildren<EnemyAI>().enabled = false;
            newEnemy.GetComponentInChildren<EnemyAI>().state = EnemyAI.EnemyState.Chasing;

            newEnemy.SetActive(true);

            yield return new WaitUntil(() => flameOBJ.GetComponentInChildren<Flame>().ended);

            Destroy(flameOBJ);
            newEnemy.GetComponentInChildren<EnemyAI>().enabled = true;

            yield return new WaitForSeconds(0.1f); // Delay of 0.1 seconds
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
