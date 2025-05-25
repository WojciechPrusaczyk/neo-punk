using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wave : MonoBehaviour
{
    [Header("Wave State")]
    public bool waveStarted;
    public bool waveEnded;

    [SerializeField] private GameObject flame;
    [SerializeField] public List<GameObject> enemies = new();

    public int aliveCount;
    [SerializeField] private float spawnInterval = 0.1f;
    
    public InvasionTrial invasionTrial;

    /* -------------------------------------------------------------------- */
    public void SpawnEnemies(List<Transform> spawnPoints)
    {
        if (waveStarted) return;

        waveStarted = true;
        aliveCount = enemies.Count;

        for (int i = 0; i < enemies.Count; i++)
        {
            GameObject prefab = enemies[i];
            float delay       = i * spawnInterval;
            StartCoroutine(SpawnSingleEnemy(prefab, spawnPoints, delay));
        }
    }

    private IEnumerator SpawnSingleEnemy(GameObject prefab,
        List<Transform> spawnPoints,
        float initialDelay)
    {
        if (initialDelay > 0f)
            yield return new WaitForSeconds(initialDelay);
        
        Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        GameObject fx = Instantiate(flame, pos, Quaternion.identity);
        
        float fxDuration = 0f;
        Animator anim = fx.GetComponentInChildren<Animator>();
        
        fxDuration = anim.GetCurrentAnimatorStateInfo(0).length / anim.speed;

        GameObject enemy = Instantiate(prefab, pos, Quaternion.identity, transform);

        EnemyAI ai = enemy.GetComponentInChildren<EnemyAI>();
        ai.enabled = false;
        ai.state   = EnemyAI.EnemyState.Chasing;

        if (fxDuration > 0f)
            yield return new WaitForSeconds(fxDuration);

        Destroy(fx);
        ai.enabled = true;

        var status = enemy.GetComponentInChildren<EntityStatus>();
        status.OnEntityDeath += DecreaseEnemyNumber;
    }

    private void DecreaseEnemyNumber()
    {
        aliveCount--;
        if (aliveCount <= 0)
            waveEnded = true;
        invasionTrial.UpdateWaveStateUI();

    }

    private void Update()
    {
        if (waveStarted && !waveEnded && transform.childCount == 0)
            waveEnded = true;
    }
}
