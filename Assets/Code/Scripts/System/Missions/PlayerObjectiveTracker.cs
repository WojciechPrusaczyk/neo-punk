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
            // Resetowanie misji przy powrocie do menu głównego
            // Save przechowuje wszystkie nasze dane, więc reset to dobre rozwiązanie
            // żeby nie przechowywać stanu w innych scenach
            objectiveList.Clear();
            currentMission = null;
            mainUserInterfaceController = null;
            playerInventoryInterface = null;

            foreach (var missionAsset in objectiveDatabase.missionList)
            {
                missionAsset.isFinished = false;
                foreach (var objective in missionAsset.objectives)
                {
                    objective.isCompleted = false;
                }
            }
            return;
        }


        GameObject mainUserInterfaceRoot = GameObject.Find("MainUserInterfaceRoot");
        if (mainUserInterfaceRoot == null)
        {
            Debug.LogError("Couldn't find MainUserInterfaceRoot in scene!");
            return;
        }

        mainUserInterfaceController = mainUserInterfaceRoot.GetComponentInChildren<MainUserInterfaceController>();
        playerInventoryInterface = mainUserInterfaceRoot.GetComponentInChildren<PlayerInventoryInterface>();

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

        // Czyszczenie misji pomiędzy scenami
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

                        if (missionAsset == currentMission)
                        {
                            SetCurrentMission(missionAsset);
                        }
                        else
                        {
                            AddMissionToMissionList(missionAsset);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Nie można znaleźć misji o nazwie: {serializableMissionData.missionName} podczas ładowania listy. Może brakować jej w ObjectiveDatabase.");
                    }
                }
            }
        }
    }
    private void LoadObjectiveStates(MissionInfo missionAsset, SerializableMission serializedMissionData)
    {
        if (missionAsset == null)
            return;

        if (serializedMissionData == null)
            return;

        if (serializedMissionData.objectiveStates == null)
            return;

        Dictionary<int, MissionInfo.MissionObjective> runtimeObjectivesMap = new Dictionary<int, MissionInfo.MissionObjective>();
        foreach (var runtimeObj in missionAsset.objectives)
        {
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
            if (runtimeObjectivesMap.TryGetValue(savedObjectiveState.ObjectiveID, out MissionInfo.MissionObjective runtimeObjective))
            {
                runtimeObjective.isCompleted = savedObjectiveState.isCompleted;
            }
            else
            {
                Debug.LogWarning($"Objective with ID {savedObjectiveState.ObjectiveID} not found in runtime objectives for mission '{missionAsset.MissionName}'. This may cause issues.");
            }
        }
    }

    public void SetCurrentMission(MissionInfo mission)
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
            mainUserInterfaceController.SetCurrentMission(mission);
            playerInventoryInterface.SetCurrentObjective(mission);

            Debug.Log($"Dodano nową misję: {mission.MissionName}");
        }
    }

    public void AddMissionToMissionList(MissionInfo mission)
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
            Debug.Log($"Dodano nową misję do listy: {mission.MissionName}");
        }
    }
}
