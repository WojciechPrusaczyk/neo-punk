using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMissionInfo", menuName = "NeoPunk/MissionInfo")]
public class MissionInfo : ScriptableObject
{
    [System.Serializable]
    public class MissionObjective
    {
        public int ObjectiveID;
        public string ObjectiveName;
        public bool isCompleted;
    }

    [Header("Must be unique")]
    public string MissionName;

    [Header("If it was finished")]
    public bool isFinished;

    [TextArea(3, 10)]
    public string MissionDescription;

    [Header("Mission Objectives")]
    public List<MissionObjective> objectives = new List<MissionObjective>();
}