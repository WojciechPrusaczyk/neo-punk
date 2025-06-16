using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerObjectiveTracker : MonoBehaviour
{
    public static PlayerObjectiveTracker instance;

    public ObjectiveDatabase objectiveDatabase;

    public List<MissionInfo> objectiveList = new List<MissionInfo>();
    public MissionInfo currentMission;
    private MainUserInterfaceController mainUserInterfaceController;
    private PlayerInventoryInterface playerInventoryInterface;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (objectiveDatabase == null)
                objectiveDatabase = GetComponent<ObjectiveDatabase>();

            if (objectiveDatabase == null)
                Debug.LogError("ObjectiveDatabase is not assigned in PlayerObjectiveTracker!");

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            objectiveList.Clear();
            currentMission = null;
            mainUserInterfaceController = null;
            playerInventoryInterface = null;

            if (objectiveDatabase != null && objectiveDatabase.missionList != null)
            {
                foreach (var missionAsset in objectiveDatabase.missionList)
                {
                    if (missionAsset != null)
                    {
                        missionAsset.isFinished = false;
                        if (missionAsset.objectives != null)
                        {
                            foreach (var objective in missionAsset.objectives)
                            {
                                if (objective != null) objective.isCompleted = false;
                            }
                        }
                    }
                }
            }
            return;
        }


        GameObject mainUserInterfaceRoot = GameObject.Find("MainUserInterfaceRoot");
        if (mainUserInterfaceRoot == null)
        {
            Debug.LogError("Couldn't find MainUserInterfaceRoot in scene!");
            mainUserInterfaceController = null;
            playerInventoryInterface = null;
        }
        else
        {
            mainUserInterfaceController = mainUserInterfaceRoot.GetComponentInChildren<MainUserInterfaceController>();
            playerInventoryInterface = mainUserInterfaceRoot.GetComponentInChildren<PlayerInventoryInterface>();
        }


        if (objectiveDatabase == null)
        {
            Debug.LogError("ObjectiveDatabase is null in PlayerObjectiveTracker!");
            return;
        }
        if (WorldSaveGameManager.instance == null || WorldSaveGameManager.instance.currentCharacterData == null)
        {
            Debug.LogWarning("WorldSaveGameManager or currentCharacterData is not available. Skipping mission load.");
            return;
        }

        CharacterSaveData saveData = WorldSaveGameManager.instance.currentCharacterData;

        objectiveList.Clear();
        currentMission = null;

        if (saveData.currentMission != null && !string.IsNullOrEmpty(saveData.currentMission.missionName))
        {
            MissionInfo missionAsset = objectiveDatabase.GetMissionFromSerializedData(saveData.currentMission);
            if (missionAsset != null)
            {
                missionAsset.isFinished = saveData.currentMission.ifFinished;
                LoadObjectiveStates(missionAsset, saveData.currentMission);
                currentMission = missionAsset;
            }
            else
            {
                Debug.LogWarning($"Could not load current mission: {saveData.currentMission.missionName}. It might be missing from the ObjectiveDatabase.");
            }
        }

        if (saveData.serializableMission != null && saveData.serializableMission.Length > 0)
        {
            foreach (var serializableMissionData in saveData.serializableMission)
            {
                if (serializableMissionData != null && !string.IsNullOrEmpty(serializableMissionData.missionName))
                {
                    MissionInfo missionAsset = objectiveDatabase.GetMissionFromSerializedData(serializableMissionData);
                    if (missionAsset != null)
                    {
                        missionAsset.isFinished = serializableMissionData.ifFinished;
                        LoadObjectiveStates(missionAsset, serializableMissionData);

                        bool alreadyInList = objectiveList.Exists(m => m != null && m.MissionName == missionAsset.MissionName);
                        if (!alreadyInList)
                        {
                            AddMissionToMissionList(missionAsset);
                        }
                    }
                }
            }
        }
        if (currentMission != null)
        {
            UpdateActiveMission(currentMission);
        }
        else if (objectiveList.Count > 0)
        {
            ActivateNextMission();
        }
        else
        {
            UpdateActiveMission(null);
        }
    }
    private void LoadObjectiveStates(MissionInfo missionAsset, SerializableMission serializedMissionData)
    {
        if (missionAsset == null || serializedMissionData == null || missionAsset.objectives == null || serializedMissionData.objectiveStates == null)
            return;

        Dictionary<int, MissionInfo.MissionObjective> runtimeObjectivesMap = new Dictionary<int, MissionInfo.MissionObjective>();
        foreach (var runtimeObj in missionAsset.objectives)
        {
            if (runtimeObj == null) continue;
            if (!runtimeObjectivesMap.ContainsKey(runtimeObj.ObjectiveID))
            {
                runtimeObjectivesMap.Add(runtimeObj.ObjectiveID, runtimeObj);
            }
            else
            {
                Debug.LogWarning($"Objective with ID {runtimeObj.ObjectiveID} already exists in the runtime map for mission '{missionAsset.MissionName}'. This may cause issues.");
            }
        }

        foreach (SerializableMission.SerializableObjectiveState savedObjectiveState in serializedMissionData.objectiveStates)
        {
            if (savedObjectiveState == null) continue;
            if (runtimeObjectivesMap.TryGetValue(savedObjectiveState.ObjectiveID, out MissionInfo.MissionObjective runtimeObjective))
            {
                if (runtimeObjective != null) runtimeObjective.isCompleted = savedObjectiveState.isCompleted;
            }
            else
            {
                Debug.LogWarning($"Objective with ID {savedObjectiveState.ObjectiveID} not found in runtime objectives for mission '{missionAsset.MissionName}'. This may cause issues.");
            }
        }
    }

    public void AddNewMission(MissionInfo mission)
    {
        if (mission == null)
        {
            Debug.LogWarning("Próbowano dodać pustą misję!");
            return;
        }

        bool alreadyExists = objectiveList.Exists(obj => obj != null && obj.MissionName == mission.MissionName);

        if (alreadyExists)
        {
            Debug.LogWarning($"Misja '{mission.MissionName}' jest już na liście celów.");
        }
        else
        {
            objectiveList.Add(mission);
            currentMission = mission;
            if (mainUserInterfaceController != null) mainUserInterfaceController.SetCurrentMission(mission);
            if (playerInventoryInterface != null) playerInventoryInterface.SetCurrentObjective(mission);
            Debug.Log($"Dodano nową misję: {mission.MissionName}");
        }
    }

    private void UpdateActiveMission(MissionInfo newMission)
    {
        this.currentMission = newMission;

        if (mainUserInterfaceController != null)
        {
            mainUserInterfaceController.SetCurrentMission(this.currentMission);
        }
        if (playerInventoryInterface != null)
        {
            playerInventoryInterface.SetCurrentObjective(this.currentMission);
        }
    }

    private int FindNextAvailableMissionIndexFrom(int effectiveStartIndex, bool searchForward)
    {
        if (objectiveList.Count == 0) return -1;

        for (int i = 0; i < objectiveList.Count; i++)
        {
            int currentIndexToTest;
            if (searchForward)
            {
                currentIndexToTest = (effectiveStartIndex + i) % objectiveList.Count;
            }
            else
            {
                currentIndexToTest = (effectiveStartIndex - i + objectiveList.Count * (i + 1)) % objectiveList.Count;
            }

            if (objectiveList[currentIndexToTest] != null && !objectiveList[currentIndexToTest].isFinished)
            {
                return currentIndexToTest;
            }
        }
        return -1;
    }

    private void SetPreviousMissionActive(MissionInfo missionToChangeFrom)
    {
        if (objectiveList.Count == 0)
        {
            UpdateActiveMission(null);
            return;
        }

        int baseIndex = -1;
        if (missionToChangeFrom != null)
        {
            baseIndex = objectiveList.IndexOf(missionToChangeFrom);
        }

        int effectiveStartIndexForSearch;
        if (baseIndex == -1)
        {
            effectiveStartIndexForSearch = objectiveList.Count - 1;
        }
        else
        {
            effectiveStartIndexForSearch = (baseIndex - 1 + objectiveList.Count) % objectiveList.Count;
        }

        int newMissionActualIndex = FindNextAvailableMissionIndexFrom(effectiveStartIndexForSearch, false);

        if (newMissionActualIndex != -1)
        {
            UpdateActiveMission(objectiveList[newMissionActualIndex]);
        }
        else
        {
            UpdateActiveMission(null);
        }
    }

    private void SetNextMissionActive(MissionInfo missionToChangeFrom)
    {
        if (objectiveList.Count == 0)
        {
            UpdateActiveMission(null);
            return;
        }

        int baseIndex = -1;
        if (missionToChangeFrom != null)
        {
            baseIndex = objectiveList.IndexOf(missionToChangeFrom);
        }

        int effectiveStartIndexForSearch;
        if (baseIndex == -1)
        {
            effectiveStartIndexForSearch = 0;
        }
        else
        {
            effectiveStartIndexForSearch = (baseIndex + 1) % objectiveList.Count;
        }

        int newMissionActualIndex = FindNextAvailableMissionIndexFrom(effectiveStartIndexForSearch, true);

        if (newMissionActualIndex != -1)
        {
            UpdateActiveMission(objectiveList[newMissionActualIndex]);
        }
        else
        {
            UpdateActiveMission(null);
        }
    }

    public void ActivateNextMission()
    {
        if (this.currentMission == null && objectiveList.Count > 0)
        {
            SetNextMissionActive(null);
            return;
        }
        SetNextMissionActive(this.currentMission);
    }
    public void ActivatePreviousMission()
    {
        if (this.currentMission == null && objectiveList.Count > 0)
        {
            SetPreviousMissionActive(null);
            return;
        }
        SetPreviousMissionActive(this.currentMission);
    }

    public void FinishCurrentMission()
    {
        if (currentMission == null)
            return;

        bool allObjectivesCompleted = true;
        if (currentMission.objectives != null)
        {
            foreach (var objective in currentMission.objectives)
            {
                if (objective == null) continue;
                if (!objective.isCompleted)
                {
                    allObjectivesCompleted = false;
                    break;
                }
            }
        }

        if (allObjectivesCompleted)
        {
            currentMission.isFinished = true;
            Debug.Log($"Ukończono misję {currentMission.MissionName}");
            ActivateNextMission();
        }
        else
        {
            Debug.Log("Nie wszystkie cele misji zostały ukończone. Misja nie może zostać zakończona.");
        }
    }

    public void AddMissionToMissionList(MissionInfo mission)
    {
        if (mission == null)
        {
            Debug.LogWarning("Próbowano dodać pustą misję!");
            return;
        }
        bool alreadyExists = objectiveList.Exists(obj => obj != null && obj.MissionName == mission.MissionName);
        if (alreadyExists)
        {
        }
        else
        {
            objectiveList.Add(mission);
        }
    }
}