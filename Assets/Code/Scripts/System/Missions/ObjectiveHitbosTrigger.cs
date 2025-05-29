using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveHitbosTrigger : MonoBehaviour
{
    public MissionInfo missionInfo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerObjectiveTracker.instance == null)
            {
                Debug.Log("PlayerObjectiveTracker instance is null!");
                return;
            }

            PlayerObjectiveTracker.instance.SetCurrentMission(missionInfo);
            gameObject.SetActive(false);
        }
    }
}
