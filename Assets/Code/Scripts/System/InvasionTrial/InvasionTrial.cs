using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class InvasionTrial : MonoBehaviour
{
    [Header("Status Flags")]
    public bool trialStarted;
    public bool trialFinished;

    [Header("Stats")]
    public float damageTaken;

    public float trialTime = 0;

    [Header("Setup References")]
    public List<Transform> spawnPoints = new();
    public List<Wave> waves = new();
    public float durationBetweenWaves   = 1f;
    public InvasionInterface invasionInterface;

    private int _currentWaveIndex = -1;

    private EventFlagsSystem _eventFlags;

    public Wave currentWave;
    public List<Reward> rewards = new List<Reward>();
    private float force = 6f;

    public List<int> medalTimes;
    public Light2D lightAfterOpening;



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


    private void Update()
    {
        if (trialStarted && !trialFinished)
        {
            trialTime += Time.deltaTime;
        }
    }

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
            GiveRewards();
        }
    }

    public void UpdateWaveStateUI()
    {
        invasionInterface.WaveState.text = $"Wave {_currentWaveIndex + 1}/{waves.Count}";
        invasionInterface.EnemiesState.text = $"Enemies left: {currentWave.aliveCount} / {currentWave.enemies.Count}";
        Debug.Log(invasionInterface.EnemiesState.text);

    }
    
    public void GiveRewards()
    {
        if(medalTimes == null)
            return;
        if(medalTimes[0]>trialTime)
        {
            
            StartCoroutine(PlayAnimation(rewards[0]));
        }
        else if(medalTimes[1]>trialTime)
        {
            StartCoroutine(PlayAnimation(rewards[1]));
        }
        else if(medalTimes[2]>trialTime)
        {
            StartCoroutine(PlayAnimation(rewards[2]));
        }
    }
    IEnumerator PlayAnimation(Reward reward)
    {
        yield return new WaitForSeconds(1.5f);
        var verPostion = gameObject.transform.position;
        var newpos = new Vector3(verPostion.x, verPostion.y+1, verPostion.z);
        if (reward.items.Count != 0)
        {
            foreach (var item in reward.items)
            {
                GameObject itemGiven = Instantiate(item, newpos, Quaternion.identity);
                var rigidBody = itemGiven.GetComponent<Rigidbody2D>();
                rigidBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            }
        }

        EntityStatus entityStatus = WorldGameManager.instance.player.GetComponent<EntityStatus>();
        
        entityStatus.AddGold(reward.goldAmount);
        if(lightAfterOpening)
            lightAfterOpening.enabled = true;
    }
}
