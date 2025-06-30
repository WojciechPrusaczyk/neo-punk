using System.Collections.Generic;
using System.Linq;
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

        [SerializeReference]
        public List<ObjectiveRequirementBase> Requirements = new List<ObjectiveRequirementBase>();

        public bool AreAllRequirementsMet()
        {
            if (Requirements == null || Requirements.Count == 0)
            {
                return true;
            }
            return Requirements.All(r => r.IsMet);
        }

        public void InitializeRequirements()
        {
            if (Requirements == null) return;
            foreach (var req in Requirements)
            {
                req.Initialize();
            }
        }

        public void ProcessEvent(ObjectiveEventBase gameEvent, PlayerObjectiveTracker tracker)
        {
            if (isCompleted || Requirements == null) return;

            foreach (var req in Requirements)
            {
                req.OnEvent(gameEvent, this, tracker);
            }
        }

        public List<string> GetRequirementProgressStrings()
        {
            if (Requirements == null) return new List<string>();
            return Requirements.Select(r => $"{r.Description}: {r.ProgressString}").ToList();
        }
    }

    [Header("Must be unique")]
    public string MissionName;

    [Header("If it was finished")]
    public bool isFinished;

    [TextArea(3, 10)]
    public string MissionDescription;

    [Header("Mission Objectives")]
    public List<MissionObjective> objectives = new List<MissionObjective>();

    public bool requireObjectiveOrder = false;

    public void InitializeAllObjectives()
    {
        isFinished = false;
        foreach (var obj in objectives)
        {
            obj.isCompleted = false;
            obj.InitializeRequirements();
        }
    }
}