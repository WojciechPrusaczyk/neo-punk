using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvasionTrial : MonoBehaviour
{
    public bool started = false;

    public List<Transform> SpawnPoints;
    [SerializeField]
    public List<Wave> waves;

    public void StartTrial()
    {
        if (!started)
        {
            started = true;
            StartCoroutine(HandleWaves());
        }
    }

    private IEnumerator HandleWaves()
    {
        foreach (Wave wave in waves)
        {
            wave.SpawnEnemies(spawnPoints: SpawnPoints);
            yield return new WaitForSeconds(wave.duration);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartTrial();
        }
    }
}