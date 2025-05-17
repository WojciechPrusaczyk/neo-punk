using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvasionTrial : MonoBehaviour
{
    public bool trialStarted = false;
    public bool trialFinished = false;

    public float damageTaken = 0;
    
    public List<Transform> SpawnPoints;
    [SerializeField]
    public List<Wave> waves;
    public int currentWave = 0;
    
    public float durationBetweenWaves = 1f;
    
    public InvasionInterface invasionInterface;

    /*
     * Event system
     */
    private GameObject EventsPage;
    private EventFlagsSystem _EventsFlagsSystem;

    private void Awake()
    {
        EventsPage = GameObject.Find("EventsFlags");
        _EventsFlagsSystem = EventsPage.GetComponent<EventFlagsSystem>();
    }

    private void Update()
    {
        if (trialStarted)
        {
            int index = currentWave - 1;

            if (index >= 0 && index < waves.Count)
            {
                int s = waves[index].gameObject.transform.childCount;
                string enemiesText = $"Enemies left: {s} / {waves[index].enemies.Count}";
                invasionInterface.EnemiesState.text = enemiesText;
            }
        }
    }

    public void StartTrial()
    {
        if (!trialStarted)
        {
            invasionInterface.gameObject.SetActive(true);
            currentWave = 1;
            trialStarted = true;
            
            UpdateWaveState();
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

        if (!_EventsFlagsSystem.IsEventDone("doneFirstArena"))
            _EventsFlagsSystem.FinishEvent("doneFirstArena");
    }
    
    public void UpdateWaveState()
    {
        string waveText =  $"Wave {currentWave}/{waves.Count}";
        invasionInterface.WaveState.text = waveText;
    }
}