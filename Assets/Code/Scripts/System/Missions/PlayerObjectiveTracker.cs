using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                        missionAsset.InitializeAllObjectives();
                    }
                }
            }
            return;
        }

        GameObject mainUserInterfaceRoot = GameObject.Find("MainUserInterfaceRoot");
        if (mainUserInterfaceRoot == null)
        {
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
            Debug.LogWarning("WorldSaveGameManager or currentCharacterData is not available. Skipping mission load and initializing from defaults.");
            InitializeMissionsFromDatabaseDefaults();
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
                            AddMissionToMissionList(missionAsset, false);
                        }
                    }
                }
            }
        }

        if (objectiveDatabase != null && objectiveDatabase.missionList != null)
        {
            foreach (var dbMission in objectiveDatabase.missionList)
            {
                if (dbMission != null && !objectiveList.Exists(m => m.MissionName == dbMission.MissionName) &&
                    (currentMission == null || currentMission.MissionName != dbMission.MissionName))
                {
                    dbMission.InitializeAllObjectives();
                }
            }
        }

        foreach (MissionObjectiveUpdater objectiveUpdater in FindObjectsByType<MissionObjectiveUpdater>(FindObjectsSortMode.None))
        {
            if (objectiveDatabase != null && objectiveDatabase.GetMissionByName(objectiveUpdater.MissionInfo.MissionName) != null)
            {
                MissionInfo.MissionObjective objective = objectiveDatabase.GetMissionByName(objectiveUpdater.MissionInfo.MissionName)
                    .objectives.FirstOrDefault(obj => obj.ObjectiveID == objectiveUpdater.ObjectiveId);
                if (objective != null && objective.isCompleted)
                {
                    objectiveUpdater.gameObject.SetActive(false);
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

    private void InitializeMissionsFromDatabaseDefaults()
    {
        if (objectiveDatabase != null && objectiveDatabase.missionList != null)
        {
            foreach (var missionAsset in objectiveDatabase.missionList)
            {
                if (missionAsset != null)
                {
                    missionAsset.InitializeAllObjectives();
                }
            }
        }
        objectiveList.Clear();
        currentMission = null;
        UpdateActiveMission(null);
    }

    private void LoadObjectiveStates(MissionInfo missionAsset, SerializableMission serializedMissionData)
    {
        if (missionAsset == null || serializedMissionData == null || missionAsset.objectives == null || serializedMissionData.objectiveStates == null)
            return;

        Dictionary<int, MissionInfo.MissionObjective> runtimeObjectivesMap = new Dictionary<int, MissionInfo.MissionObjective>();
        foreach (var runtimeObj in missionAsset.objectives)
        {
            if (runtimeObj == null) continue;
            runtimeObj.InitializeRequirements();
            if (!runtimeObjectivesMap.ContainsKey(runtimeObj.ObjectiveID))
            {
                runtimeObjectivesMap.Add(runtimeObj.ObjectiveID, runtimeObj);
            }
        }

        foreach (SerializableMission.SerializableObjectiveState savedObjectiveState in serializedMissionData.objectiveStates)
        {
            if (savedObjectiveState == null) continue;
            if (runtimeObjectivesMap.TryGetValue(savedObjectiveState.ObjectiveID, out MissionInfo.MissionObjective runtimeObjective))
            {
                if (runtimeObjective != null)
                {
                    runtimeObjective.isCompleted = savedObjectiveState.isCompleted;
                    if (runtimeObjective.Requirements != null && savedObjectiveState.requirementProgresses != null)
                    {
                        for (int i = 0; i < runtimeObjective.Requirements.Count && i < savedObjectiveState.requirementProgresses.Count; i++)
                        {
                            if (runtimeObjective.Requirements[i] != null && savedObjectiveState.requirementProgresses[i] != null)
                            {
                                runtimeObjective.Requirements[i].LoadProgress(savedObjectiveState.requirementProgresses[i]);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ReportEnemyKilled(Enums.EnemyType typeOfEnemyKilled)
    {
        if (currentMission == null || currentMission.isFinished) return;

        EnemyKilledEvent killEvent = new EnemyKilledEvent(typeOfEnemyKilled);

        if (currentMission.requireObjectiveOrder)
        {
            var activeObjective = currentMission.objectives.FirstOrDefault(o => !o.isCompleted);
            if (activeObjective != null)
            {
                activeObjective.ProcessEvent(killEvent, this);
            }
        }
        else
        {
            foreach (var objective in currentMission.objectives)
            {
                if (!objective.isCompleted)
                {
                    objective.ProcessEvent(killEvent, this);
                }
            }
        }
    }

    public void CheckObjectiveCompletion(MissionInfo.MissionObjective objective)
    {
        if (objective == null || objective.isCompleted) return;

        if (currentMission != null && currentMission.requireObjectiveOrder)
        {
            int objectiveIndex = currentMission.objectives.IndexOf(objective);
            if (objectiveIndex > 0)
            {
                if (!currentMission.objectives[objectiveIndex - 1].isCompleted)
                {
                    return;
                }
            }
        }

        if (objective.AreAllRequirementsMet())
        {
            objective.isCompleted = true;
            Debug.Log($"Objective '{objective.ObjectiveName}' completed in mission '{currentMission?.MissionName}'.");
            if (mainUserInterfaceController != null) mainUserInterfaceController.SetCurrentMission(currentMission);
            if (playerInventoryInterface != null) playerInventoryInterface.SetCurrentObjective(currentMission);
        }
    }

    public void AddNewMission(MissionInfo mission)
    {
        if (mission == null)
        {
            Debug.LogWarning("Tried to add a null mission!");
            return;
        }

        bool alreadyExists = objectiveList.Exists(obj => obj != null && obj.MissionName == mission.MissionName);
        if (alreadyExists)
        {
            Debug.LogWarning($"Mission '{mission.MissionName}' is already in the objective list.");
        }
        else
        {
            mission.InitializeAllObjectives();
            objectiveList.Add(mission);
        }

        currentMission = mission;
        UpdateActiveMission(currentMission);
        Debug.Log($"Added/Set current mission: {mission.MissionName}");
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
            baseIndex = objectiveList.FindIndex(m => m != null && m.MissionName == missionToChangeFrom.MissionName);
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
            baseIndex = objectiveList.FindIndex(m => m != null && m.MissionName == missionToChangeFrom.MissionName);
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

    public bool AreAllCurrentMissionObjectivesComplete()
    {
        if (currentMission == null)
        {
            return false;
        }

        if (currentMission.objectives == null || currentMission.objectives.Count == 0)
        {
            return true;
        }

        return currentMission.objectives.All(obj => obj.isCompleted);
    }

    public void FinishCurrentMission()
    {
        if (currentMission == null)
            return;

        if (AreAllCurrentMissionObjectivesComplete())
        {
            currentMission.isFinished = true;
            Debug.Log($"Mission '{currentMission.MissionName}' completed.");
            ActivateNextMission();
        }
        else
        {
            Debug.Log("Not all objectives for the current mission are completed. Mission cannot be finished yet.");
        }
    }

    public void AddMissionToMissionList(MissionInfo mission, bool initialize = true)
    {
        if (mission == null)
        {
            Debug.LogWarning("Tried to add a null mission to list!");
            return;
        }
        bool alreadyExists = objectiveList.Exists(obj => obj != null && obj.MissionName == mission.MissionName);
        if (!alreadyExists)
        {
            if (initialize) mission.InitializeAllObjectives();
            objectiveList.Add(mission);
        }
    }

    public void ChangeMissionObjectiveStatus(int objectiveID, bool status)
    {
        if (currentMission == null)
        {
            Debug.LogWarning("No active mission, cannot change objective status.");
            return;
        }

        if (status && currentMission.requireObjectiveOrder)
        {
            int objectiveIndex = currentMission.objectives.FindIndex(obj => obj.ObjectiveID == objectiveID);

            if (objectiveIndex == -1)
            {
                Debug.LogWarning($"Objective with ID {objectiveID} not found in current mission.");
                return;
            }

            for (int i = 0; i < objectiveIndex; i++)
            {
                if (!currentMission.objectives[i].isCompleted)
                {
                    Debug.Log($"Cannot complete objective '{currentMission.objectives[objectiveIndex].ObjectiveName}' because a previous objective is not yet complete.");
                    return;
                }
            }
        }

        MissionInfo.MissionObjective objective = currentMission.objectives.Find(obj => obj.ObjectiveID == objectiveID);
        if (objective != null)
        {
            objective.isCompleted = status;
            if (!status)
            {
                objective.InitializeRequirements();
            }
            if (mainUserInterfaceController != null) mainUserInterfaceController.SetCurrentMission(currentMission);
            if (playerInventoryInterface != null) playerInventoryInterface.SetCurrentObjective(currentMission);
        }
    }
}