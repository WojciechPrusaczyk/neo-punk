using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvasionTrial : MonoBehaviour
{
    public bool trialStarted = false;
    public bool trialFinished = false;

    public List<Transform> SpawnPoints;
    [SerializeField]
    public List<Wave> waves;
    public int currentWave = 0;
    
    public float durationBetweenWaves = 1f;

    public void StartTrial()
    {
        if (!trialStarted)
        {
            trialStarted = true;
            currentWave = 1;
            StartCoroutine(HandleWaves());
        }
    }

    private IEnumerator HandleWaves()
    {
        foreach (Wave wave in waves)
        {
            wave.SpawnEnemies(spawnPoints: SpawnPoints);
            while (!wave.waveEnded)
            {
                yield return null;
            }
            currentWave++;
            yield return new WaitForSeconds(durationBetweenWaves);
        }
        trialFinished = true;
    }
}