using System;
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
    
    public InvasionInterface invasionInterface;


    private void Update()
    {
        if (trialStarted)
        {
            int s = waves[currentWave - 1].gameObject.transform.childCount;
            string enemiesText = $"Enemies left: {waves[currentWave - 1].gameObject.transform.childCount} / {waves[currentWave - 1].enemies.Count}";
            invasionInterface.EnemiesState.text = enemiesText;
        }
    }

    public void StartTrial()
    {
        if (!trialStarted)
        {
            invasionInterface.gameObject.SetActive(true);
            UpdateWaveState();
            
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
            UpdateWaveState();
            yield return new WaitForSeconds(durationBetweenWaves);
        }
        trialFinished = true;
        invasionInterface.gameObject.SetActive(false);
    }
    
    public void UpdateWaveState()
    {
        string waveText =  $"Wave {currentWave}/{waves.Count}";
        invasionInterface.WaveState.text = waveText;
    }
}