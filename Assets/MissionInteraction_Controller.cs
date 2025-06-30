using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionInteraction_Controller : MonoBehaviour
{
    public MissionInfo mission;

    public void AddMissionToObjectives()
    {
        if (PlayerObjectiveTracker.instance == null)
            return;

        PlayerObjectiveTracker.instance.AddNewMission(mission);
    }
}
