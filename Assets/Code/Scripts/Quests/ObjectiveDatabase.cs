using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveDatabase : MonoBehaviour
{
    public List<MissionInfo> missionList = new List<MissionInfo>();

    public MissionInfo GetMissionByName(string missionName)
    {
        foreach (var mission in missionList)
        {
            if (mission.MissionName == missionName)
            {
                return mission;
            }
        }
        Debug.LogWarning($"Mission '{missionName}' not found in the database.");
        return null;
    }

    public MissionInfo GetMissionFromSerializedData(SerializableMission serializableMission)
    {
        if (serializableMission == null || string.IsNullOrEmpty(serializableMission.missionName))
        {
            Debug.LogWarning("Attempted to get mission from null or unnamed SerializableMission.");
            return null;
        }

        MissionInfo missionAsset = GetMissionByName(serializableMission.missionName);

        if (missionAsset == null)
        {
            Debug.LogWarning($"Mission asset '{serializableMission.missionName}' not found in the database. " + "Check if the mission name in save data matches an existing MissionInfo asset name " + "and that the asset is added to ObjectiveDatabase.missionList.");
            return null;
        }

        return missionAsset;
    }
}
