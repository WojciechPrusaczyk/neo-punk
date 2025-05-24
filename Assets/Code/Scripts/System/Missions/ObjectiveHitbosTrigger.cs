using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveHitbosTrigger : MonoBehaviour
{
    public MissionInfo missionInfo;
    private PlayerObjectiveTracker playerObjectiveTracker;

    private void Awake()
    {
        playerObjectiveTracker = GameObject.Find("MainUserInterfaceRoot").GetComponentInChildren<PlayerObjectiveTracker>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerObjectiveTracker.SetCurrentObjective(missionInfo);
            gameObject.SetActive(false);
        }
    }
}
