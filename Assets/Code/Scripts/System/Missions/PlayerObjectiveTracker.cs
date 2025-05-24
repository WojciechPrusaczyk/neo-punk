using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectiveTracker : MonoBehaviour
{
    public List<MissionInfo> objectiveList = new List<MissionInfo>();
    public MissionInfo currentMission;
    private MainUserInterfaceController mainUserInterfaceController;
    private PlayerInventoryInterface playerInventoryInterface;

    private void Awake()
    {
        var mainUserInterfaceRoot = GameObject.Find("MainUserInterfaceRoot").gameObject;

        mainUserInterfaceController = mainUserInterfaceRoot.GetComponentInChildren<MainUserInterfaceController>();
        playerInventoryInterface = mainUserInterfaceRoot.GetComponentInChildren<PlayerInventoryInterface>();
    }

    public void SetCurrentObjective(MissionInfo mission)
    {
        if (mission == null)
        {
            Debug.LogWarning("Próbowano dodać pustą misję!");
            return;
        }

        bool alreadyExists = objectiveList.Exists(obj => obj.MissionName == mission.MissionName);

        if (alreadyExists)
        {
            Debug.LogWarning($"Misja '{mission.MissionName}' jest już na liście celów.");
        }
        else
        {
            objectiveList.Add(mission);
            currentMission = mission;
            playerInventoryInterface.SetCurrentObjective(mission);

            Debug.Log($"Dodano nową misję: {mission.MissionName}");
        }
    }
}
