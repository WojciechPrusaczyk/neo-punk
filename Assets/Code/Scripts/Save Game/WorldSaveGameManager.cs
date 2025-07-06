using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    public Player player;

    [SerializeField] bool saveGame = false;
    [SerializeField] bool loadGame = false;
    [SerializeField] bool saveWithoutEncryption = false;

    [Header("Save Data Writer")]
    SaveFileDataWriter saveFileDataWriter;

    [Header("Current Character Data")]
    public CharacterSlots currentCharacterSlotBeingUsed;
    public CharacterSaveData currentCharacterData;
    private string saveFileName;
    private string settingsFileName;

    public SettingsSaveData settingsSaveData;

    [Header("Character Slots")]
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;

    private bool _pendingSaveAfterSceneLoad = false;
    private string _targetSceneForSave = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
        }
    }

    private void Start()
    {
        AttemptToCreateNewSettingsFile();
    }

    private void Update()
    {
        if (saveGame)
        {
            saveGame = false;
            SaveGame();
        }

        if (loadGame)
        {
            loadGame = false;
            LoadGame();
        }
    }

    public void NewGame()
    {
        _pendingSaveAfterSceneLoad = true;
        _targetSceneForSave = "InitialLevel";

        StartCoroutine(NewGameCoroutine());
    }

    private IEnumerator NewGameCoroutine()
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("InitialLevel");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();

        if (WorldAIManager.instance != null)
        {
            WorldAIManager.instance.InitializeAIForScene(SceneManager.GetActiveScene());
        }
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
            LoadAllCharacterProfiles();

        if (_pendingSaveAfterSceneLoad && scene.name == _targetSceneForSave)
        {
            _pendingSaveAfterSceneLoad = false;
            _targetSceneForSave = "";
            StartCoroutine(SaveGameAfterSceneLoadRoutine());
        }

        if (player == null && currentCharacterData != null && !string.IsNullOrEmpty(currentCharacterData.sceneName) && scene.name == currentCharacterData.sceneName)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    private IEnumerator SaveGameAfterSceneLoadRoutine()
    {
        yield return new WaitForEndOfFrame();
        SaveGame();
    }


    public void SaveGame()
    {
        if (player == null && SceneManager.GetActiveScene().name != "MainMenu") // Allow saving from main menu if needed, though player would be null
        {
            player = FindFirstObjectByType<Player>();
            if (player == null && SceneManager.GetActiveScene().name != "MainMenu") // Re-check after find
            {
                Debug.LogError("Player not found in current scene '" + SceneManager.GetActiveScene().name + "', cannot save game!");
                return;
            }
        }


        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;

        if (currentCharacterData == null)
        {
            currentCharacterData = new CharacterSaveData();
        }

        if (EventFlagsSystem.instance != null)
        {
            List<EventFlagsSystem.EventFlag> allDoneFlags = new List<EventFlagsSystem.EventFlag>();
            foreach (var flag in EventFlagsSystem.instance.eventFlags)
            {
                if (flag.isDone)
                {
                    allDoneFlags.Add(flag);
                }
            }
            currentCharacterData.completedEventFlags = allDoneFlags.ToArray();
        }
        else
        {
            if (currentCharacterData.completedEventFlags == null)
            {
                currentCharacterData.completedEventFlags = new EventFlagsSystem.EventFlag[0];
            }
        }

        List<SerializableMission> sMissions = new List<SerializableMission>();
        if (PlayerObjectiveTracker.instance != null && PlayerObjectiveTracker.instance.objectiveList != null)
        {
            foreach (MissionInfo liveMission in PlayerObjectiveTracker.instance.objectiveList)
            {
                if (liveMission == null) continue;
                sMissions.Add(CreateSerializableMissionFromMissionInfo(liveMission));
            }
        }
        currentCharacterData.serializableMission = sMissions.ToArray();

        if (PlayerObjectiveTracker.instance != null && PlayerObjectiveTracker.instance.currentMission != null)
        {
            currentCharacterData.currentMission = CreateSerializableMissionFromMissionInfo(PlayerObjectiveTracker.instance.currentMission);
        }
        else
        {
            currentCharacterData.currentMission = null;
        }

        if (player != null) // Only save player data if player exists
        {
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);
        }
        currentCharacterData.sceneName = SceneManager.GetActiveScene().name;
        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData, !saveWithoutEncryption);
        Debug.Log($"Game saved to slot: {currentCharacterSlotBeingUsed} (File: {saveFileName}) in scene {currentCharacterData.sceneName}");


    }

    private SerializableMission CreateSerializableMissionFromMissionInfo(MissionInfo missionInfo)
    {
        SerializableMission sm = new SerializableMission();
        sm.missionName = missionInfo.MissionName;
        sm.ifFinished = missionInfo.isFinished;
        sm.objectiveStates = new List<SerializableMission.SerializableObjectiveState>();

        foreach (MissionInfo.MissionObjective obj in missionInfo.objectives)
        {
            var objectiveState = new SerializableMission.SerializableObjectiveState
            {
                ObjectiveID = obj.ObjectiveID,
                isCompleted = obj.isCompleted,
                requirementProgresses = new List<ObjectiveRequirementSaveData>()
            };

            if (obj.Requirements != null)
            {
                foreach (var req in obj.Requirements)
                {
                    objectiveState.requirementProgresses.Add(req.GetSaveData());
                }
            }
            sm.objectiveStates.Add(objectiveState);
        }
        return sm;
    }

    public bool CheckIfSaveFileExists(int id)
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed((CharacterSlots)id);
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        return saveFileDataWriter.CheckToSeeIfFileExists();
    }

    public void LoadGame(float delay = 0f)
    {
        StartCoroutine(LoadGameCoroutine(delay));
    }

    public void DeleteGame(int id)
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed((CharacterSlots)id);

        saveFileDataWriter.DeleteSaveFile();
    }


    private IEnumerator LoadGameCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        if (currentCharacterData == null)
        {
            Debug.LogError($"Could not load save file: {saveFileName}.");
            yield break;
        }

        string sceneToLoad = currentCharacterData.sceneName;
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("No scene name in save file.");
            yield break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();

        player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
        }
        else if (sceneToLoad != "MainMenu")
        {
            Debug.LogError("Player not found in loaded scene '" + sceneToLoad + "'!");
        }


        if (currentCharacterData.completedEventFlags != null && EventFlagsSystem.instance != null)
        {
            foreach (var savedFlag in currentCharacterData.completedEventFlags)
            {
                if (savedFlag.isDone)
                {
                    EventFlagsSystem.instance.FinishEvent(savedFlag.name);
                }
            }
        }


        if (MusicManager.instance != null)
            MusicManager.instance.RestartSong();

        if (WorldAIManager.instance != null)
        {
            WorldAIManager.instance.InitializeAIForScene(SceneManager.GetActiveScene());
        }
    }

    public bool HasFreeCharacterSlot()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot1);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) return true;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot2);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) return true;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot3);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) return true;

        return false;
    }


    public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots characterSlot)
    {
        string fileName = "";
        switch (characterSlot)
        {
            case CharacterSlots.CharacterSlot1: fileName = "CharacterSlot1"; break;
            case CharacterSlots.CharacterSlot2: fileName = "CharacterSlot2"; break;
            case CharacterSlots.CharacterSlot3: fileName = "CharacterSlot3"; break;
            default:
                fileName = "CharacterSlot_Unknown";
                break;
        }
        return fileName;
    }

    public void AttemptToCreateNewGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot1);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlots.CharacterSlot1;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot2);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlots.CharacterSlot2;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot3);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlotBeingUsed = CharacterSlots.CharacterSlot3;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }
    }

    public void AttemptToCreateNewSettingsFile()
    {
        if (WorldSoundFXManager.instance == null)
        {
            return;
        }

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        settingsFileName = "Settings";
        saveFileDataWriter.saveFileName = settingsFileName;

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        if (WorldSoundFXManager.instance == null)
        {
            return;
        }

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        settingsFileName = "Settings";
        saveFileDataWriter.saveFileName = settingsFileName;

        settingsSaveData = new SettingsSaveData();
        settingsSaveData.masterVolume = WorldSoundFXManager.instance.masterVolume;
        settingsSaveData.sfxVolume = WorldSoundFXManager.instance.sfxVolume;
        settingsSaveData.musicVolume = WorldSoundFXManager.instance.musicVolume;
        settingsSaveData.dialogueVolume = WorldSoundFXManager.instance.dialogueVolume;
        settingsSaveData.tooltipsEnabled = UserInterfaceController.instance.tooltipsEnabled;
        saveFileDataWriter.CreateNewSettingsSaveFile(settingsSaveData);
    }

    public void LoadSettingsFile()
    {
        if (WorldSoundFXManager.instance == null)
        {
            return;
        }

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        settingsFileName = "Settings";
        saveFileDataWriter.saveFileName = settingsFileName;

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            AttemptToCreateNewSettingsFile();
            return;
        }

        settingsSaveData = saveFileDataWriter.LoadSettingsSaveFile();
        if (settingsSaveData == null)
        {
            AttemptToCreateNewSettingsFile();
            return;
        }
        WorldSoundFXManager.instance.masterVolume = settingsSaveData.masterVolume;
        WorldSoundFXManager.instance.sfxVolume = settingsSaveData.sfxVolume;
        WorldSoundFXManager.instance.musicVolume = settingsSaveData.musicVolume;
        WorldSoundFXManager.instance.dialogueVolume = settingsSaveData.dialogueVolume;
    }


    private void LoadAllCharacterProfiles()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot1);
        characterSlot01 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot2);
        characterSlot02 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot3);
        characterSlot03 = saveFileDataWriter.LoadSaveFile();
    }

    public SerializableMission GetSerializableMissionFromMissionInfo(MissionInfo mission)
    {
        if (mission == null)
        {
            return new SerializableMission { missionName = "" };
        }
        return CreateSerializableMissionFromMissionInfo(mission);
    }
}