using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;
using static SerializableMission;

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
        LoadAllCharacterProfiles();
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

        SceneManager.LoadScene(_targetSceneForSave);
        if (MusicManager.instance != null)
            MusicManager.instance.PlaySong(1);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }

        if (player == null)
        {
            Debug.LogError("Nie znaleziono gracza w bie¿¹cej scenie '" + SceneManager.GetActiveScene().name + "', nie mo¿na zapisaæ gry!");
            return;
        }

        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;

        if (currentCharacterData == null)
        {
            currentCharacterData = new CharacterSaveData();
            Debug.LogWarning("currentCharacterData was null during SaveGame. Initializing new CharacterSaveData.");
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
        foreach (MissionInfo liveMission in PlayerObjectiveTracker.instance.objectiveList)
        {
            SerializableMission sm = new SerializableMission();
            sm.missionName = liveMission.MissionName;
            sm.ifFinished = liveMission.isFinished;
            sm.objectiveStates = new List<SerializableObjectiveState>();
            foreach (MissionInfo.MissionObjective obj in liveMission.objectives)
            {
                sm.objectiveStates.Add(new SerializableObjectiveState {
                    ObjectiveID = obj.ObjectiveID,
                    isCompleted = obj.isCompleted
                });
            }
            sMissions.Add(sm);
        }
        currentCharacterData.serializableMission = sMissions.ToArray();

        if (PlayerObjectiveTracker.instance.currentMission != null)
        {
            MissionInfo liveCurrentMission = PlayerObjectiveTracker.instance.currentMission;
            SerializableMission scm = new SerializableMission();
            scm.missionName = liveCurrentMission.MissionName;
            scm.ifFinished = liveCurrentMission.isFinished;
            scm.objectiveStates = new List<SerializableObjectiveState>();
            foreach (MissionInfo.MissionObjective obj in liveCurrentMission.objectives)
            {
                scm.objectiveStates.Add(new SerializableObjectiveState {
                    ObjectiveID = obj.ObjectiveID,
                    isCompleted = obj.isCompleted
                });
            }
            currentCharacterData.currentMission = scm;
        }
        else
        {
            currentCharacterData.currentMission = null;
        }

        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);
        currentCharacterData.sceneName = SceneManager.GetActiveScene().name;
        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData, !saveWithoutEncryption);
        Debug.Log($"Zapisano grê do slota: {currentCharacterSlotBeingUsed} (Plik: {saveFileName}) w scenie {currentCharacterData.sceneName}");
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

    private IEnumerator LoadGameCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Loading Game Data...");

        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        if (currentCharacterData == null)
        {
            Debug.LogError($"Nie mo¿na za³adowaæ pliku: {saveFileName}.");
            yield break;
        }

        string sceneToLoad = currentCharacterData.sceneName;
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Brak nazwy sceny w pliku zapisu.");
            yield break;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return new WaitForEndOfFrame();

        player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("Brak gracza w za³adowanej scenie '" + sceneToLoad + "'!");
            yield break;
        }

        player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);

        if (currentCharacterData.completedEventFlags != null)
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
        Debug.Log("Game loaded successfully.");
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
                Debug.LogWarning($"Unknown character slot: {characterSlot}");
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
        Debug.Log("Nie ma wolnych slotów zapisu gry!");
    }

    public void AttemptToCreateNewSettingsFile()
    {
        if (WorldSoundFXManager.instance == null)
        {
            Debug.LogWarning("WorldSoundFXManager instance not found. Cannot create/save settings.");
            return;
        }

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        settingsFileName = "Settings";
        saveFileDataWriter.saveFileName = settingsFileName;

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            Debug.Log("Settings file not found. Creating new one with default values.");
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        if (WorldSoundFXManager.instance == null)
        {
            Debug.LogWarning("WorldSoundFXManager instance not found. Cannot save settings.");
            return;
        }

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        settingsFileName = "Settings";
        saveFileDataWriter.saveFileName = settingsFileName;

        SettingsSaveData settingsData = new SettingsSaveData();
        settingsData.masterVolume = WorldSoundFXManager.instance.masterVolume;
        settingsData.sfxVolume = WorldSoundFXManager.instance.sfxVolume;
        settingsData.musicVolume = WorldSoundFXManager.instance.musicVolume;
        settingsData.dialogueVolume = WorldSoundFXManager.instance.dialogueVolume;
        saveFileDataWriter.CreateNewSettingsSaveFile(settingsData);
    }

    public void LoadSettingsFile()
    {
        if (WorldSoundFXManager.instance == null)
        {
            Debug.LogWarning("WorldSoundFXManager instance not found. Cannot load settings.");
            return;
        }

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        settingsFileName = "Settings";
        saveFileDataWriter.saveFileName = settingsFileName;

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            Debug.LogWarning($"Settings file '{settingsFileName}' not found. Attempting to create one with defaults.");
            AttemptToCreateNewSettingsFile();
            return;
        }

        SettingsSaveData loadedSettings = saveFileDataWriter.LoadSettingsSaveFile();
        if (loadedSettings == null)
        {
            Debug.LogError($"Nie mo¿na za³adowaæ pliku ustawieñ: {settingsFileName}. Creating new one.");
            AttemptToCreateNewSettingsFile();
            return;
        }
        WorldSoundFXManager.instance.masterVolume = loadedSettings.masterVolume;
        WorldSoundFXManager.instance.sfxVolume = loadedSettings.sfxVolume;
        WorldSoundFXManager.instance.musicVolume = loadedSettings.musicVolume;
        WorldSoundFXManager.instance.dialogueVolume = loadedSettings.dialogueVolume;
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
        SerializableMission serializedMission = new SerializableMission();

        if (mission == null)
        {
            serializedMission.missionName = "";
            return serializedMission;
        }

        serializedMission.missionName = mission.MissionName;

        return serializedMission;
    }
}