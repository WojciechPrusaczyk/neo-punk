using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InvasionTrial : MonoBehaviour
{
    [Header("Status Flags")]
    public bool trialStarted;
    public bool trialFinished;

    [Header("Stats")]
    public float damageTaken;

    [Header("Setup References")]
    public List<Transform> spawnPoints = new();
    public List<Wave> waves = new();
    public float durationBetweenWaves   = 1f;
    public InvasionInterface invasionInterface;

    private int _currentWaveIndex = -1;

    private EventFlagsSystem _eventFlags;

    public Wave currentWave;

    private void Awake()
    {
        GameObject eventsPage = GameObject.Find("EventsFlags");
        if (eventsPage == null)
            Debug.LogError("EventsFlags GameObject missing in scene! (needed by InvasionTrial)");
        else
            _eventFlags = eventsPage.GetComponent<EventFlagsSystem>();
    }

    #region Public API
    public void StartTrial()
    {
        if (trialStarted) return;

        trialStarted = true;
        invasionInterface.gameObject.SetActive(true);
        StartCoroutine(RunTrial());
    }
    #endregion

    private IEnumerator RunTrial()
    {
        for (_currentWaveIndex = 0; _currentWaveIndex < waves.Count; _currentWaveIndex++)
        {
            Wave wave = waves[_currentWaveIndex];
            currentWave = wave;
            UpdateWaveStateUI();

            wave.SpawnEnemies(spawnPoints);
            
            invasionInterface.EnemiesState.text = $"Enemies left: {currentWave.aliveCount} / {currentWave.enemies.Count}";

            yield return new WaitUntil(() => wave.aliveCount == 0);

            yield return new WaitForSeconds(durationBetweenWaves);
        }

        trialFinished = true;
        invasionInterface.gameObject.SetActive(false);

        if (_eventFlags && !_eventFlags.IsEventDone("doneFirstArena"))
        {
            _eventFlags.FinishEvent("doneFirstArena");
            Debug.Log("Invasion Trial Done");
        }
    }

    public void UpdateWaveStateUI()
    {
        invasionInterface.WaveState.text = $"Wave {_currentWaveIndex + 1}/{waves.Count}";
        invasionInterface.EnemiesState.text = $"Enemies left: {currentWave.aliveCount} / {currentWave.enemies.Count}";
        Debug.Log(invasionInterface.EnemiesState.text);

    }
}
