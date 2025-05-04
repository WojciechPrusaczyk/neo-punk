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

    [Header("Character Slots")]
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllCharacterProfiles();
    }

    public bool HasFreeCharacterSlot()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot1);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
            return true;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot2);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
            return true;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot3);
        if (!saveFileDataWriter.CheckToSeeIfFileExists())
            return true;

        return false;
    }

    public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots characterSlot)
    {
        string fileName = "";
        switch (characterSlot)
        {
            case CharacterSlots.CharacterSlot1:
                fileName = "CharacterSlot1";
                break;
            case CharacterSlots.CharacterSlot2:
                fileName = "CharacterSlot2";
                break;
            case CharacterSlots.CharacterSlot3:
                fileName = "CharacterSlot3";
                break;
            default:
                break;
        }

        return fileName;
    }

    // To bêdzie u¿yte jako alternatywa do aktualnego startowania gry, kiedy ju¿ bêdzie UI do save slotów
    public void AttemptToCreateNewGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        // Sprawdzamy czy mo¿emy stworzyæ nowy plik zapisu (sprawdzamy pierw inne istniej¹ce pliki)
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlots.CharacterSlot1);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            // Jeœli ten slot nie jest zajêty, utwórz nowy przy u¿yciu tego slotu
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

        // Tutaj trzeba bêdzie dodaæ powiadomienie o braku character slotów
        Debug.Log("Nie ma wolnych slotów zapisu gry!");
    }

    private void NewGame()
    {
        SaveGame();
        // Wystartowanie nowej gry poprzez przeniesienie do nowej sceny
        // <tutaj>
        Debug.Log("Startowanie nowej gry.");
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameCoroutine());
    }
    public void SaveGame()
    {
        player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError("Nie znaleziono gracza w bie¿¹cej scenie, nie mo¿na zapisaæ gry!");
            return;
        }

        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;

        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

        currentCharacterData.sceneName = SceneManager.GetActiveScene().name;

        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData, !saveWithoutEncryption);
        Debug.Log($"Zapisano grê do slota: {currentCharacterSlotBeingUsed} (Plik: {saveFileName})");
    }
    private IEnumerator LoadGameCoroutine()
    {
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

        player = FindFirstObjectByType<Player>();

        if (player == null)
        {
            Debug.LogError("Brak gracza w za³adowanej scenie!");
            yield break;
        }

        player.LoadGameDataFromCurrentCharacterData(ref currentCharacterData);
    }

    public void AttemptToCreateNewSettingsFile()
    {
        if (WorldSoundFXManager.instance == null)
            return;

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        settingsFileName = "Settings";
        saveFileDataWriter.saveFileName = settingsFileName;
        
        if (saveFileDataWriter.CheckToSeeIfFileExists())
            return;
        else
            SaveSettings();
    }
    
    public void SaveSettings()
    {
        if (WorldSoundFXManager.instance == null)
            return;

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        settingsFileName = "Settings";
        saveFileDataWriter.saveFileName = settingsFileName;

        SettingsSaveData worldSoundFXManager = new SettingsSaveData();
        worldSoundFXManager.masterVolume = WorldSoundFXManager.instance.masterVolume;
        worldSoundFXManager.sfxVolume = WorldSoundFXManager.instance.sfxVolume;
        worldSoundFXManager.musicVolume = WorldSoundFXManager.instance.musicVolume;
        worldSoundFXManager.dialogueVolume = WorldSoundFXManager.instance.dialogueVolume;
        saveFileDataWriter.CreateNewSettingsSaveFile(worldSoundFXManager);
    }

    public void LoadSettingsFile()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        settingsFileName = "Settings";
        saveFileDataWriter.saveFileName = settingsFileName;
        SettingsSaveData worldSoundFXManager = saveFileDataWriter.LoadSettingsSaveFile();
        if (worldSoundFXManager == null)
        {
            Debug.LogError("Nie mo¿na za³adowaæ pliku ustawieñ.");
            return;
        }
        WorldSoundFXManager.instance.masterVolume = worldSoundFXManager.masterVolume;
        WorldSoundFXManager.instance.sfxVolume = worldSoundFXManager.sfxVolume;
        WorldSoundFXManager.instance.musicVolume = worldSoundFXManager.musicVolume;
        WorldSoundFXManager.instance.dialogueVolume = worldSoundFXManager.dialogueVolume;
        Debug.Log("Za³adowano ustawienia dŸwiêku.");
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

}
