using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableMission : ISerializationCallbackReceiver
{
    [System.Serializable]
    public class SerializableObjectiveState
    {
        public int ObjectiveID;
        public bool isCompleted;
        [SerializeReference]
        public List<ObjectiveRequirementSaveData> requirementProgresses = new List<ObjectiveRequirementSaveData>();
    }

    [SerializeField] public string missionName;
    [SerializeField] public bool ifFinished;
    [SerializeField] public List<SerializableObjectiveState> objectiveStates = new List<SerializableObjectiveState>();

    public MissionInfo GetMissionInfo()
    {
        if (PlayerObjectiveTracker.instance == null || PlayerObjectiveTracker.instance.objectiveDatabase == null)
            return null;

        MissionInfo missionInfo = PlayerObjectiveTracker.instance.objectiveDatabase.GetMissionFromSerializedData(this);
        return missionInfo;
    }

    public void OnAfterDeserialize()
    {
    }

    public void OnBeforeSerialize()
    {
    }
}