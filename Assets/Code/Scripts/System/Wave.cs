using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    public List<GameObject> enemies;
    public float duration;


    public void SpawnEnemies(List<Transform> spawnPoints)
    {
        StartCoroutine(SpawnEnemiesWithDelay(spawnPoints));
    }
    
    private IEnumerator SpawnEnemiesWithDelay(List<Transform> spawnPoints)
    {
        foreach (GameObject enemy in enemies)
        {
            GameObject newEnemy = Instantiate(enemy, spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
            newEnemy.GetComponentInChildren<EnemyAI>().state = EnemyAI.EnemyState.Chasing;

            yield return new WaitForSeconds(0.2f); // Delay of 0.1 seconds
        }
    }
}
