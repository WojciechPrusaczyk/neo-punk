using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Chcemy zeby ten skrypt by� serializowalny, poniewa� b�dziemy go u�ywa� do zapisywania danych o kilku postaciach
public class CharacterSaveData
{
    [Header("SCENE INDEX")]
    public int sceneIndex = 1;
    public string sceneName;

    [Header("Character Name")]
    public string characterName = "Character";

    // Czemu nie Vector3? Poniewa� nie mo�emy zapisa� Vector3 w JSON, poniewa� nie jest to typ podstawowy
    [Header("World Coordinates")]
    public float xPosition = 0f;
    public float yPosition = 2f;
    public float zPosition = 0f;

    [Header("Resources")]
    public float currentHealth;

    [Header("Items")]
    public SerializableDictionary<string, int> inventoryItems; // <name, slot [0-3]>
    public SerializableDictionary<int, bool> itemsPickedUp; // <itemID, hasBeenPickedUp>

    [Header("Drones")]
    public SerializableDictionary<string, SerializableIntList> activatedDronesByScene;
    public DroneIdentifier lastVisitedDrone;
    public string lastVisitedDroneName = "No drones visited";

    [Header("Events")]
    public EventFlagsSystem.EventFlag[] completedEventFlags;

    [Header("Objectives")]
    public SerializableMission[] serializableMission;
    public SerializableMission currentMission;

    [Header("Tutorial")]
    public SerializableDictionary<int, bool> tutorialTexts;

    public CharacterSaveData()
    {
        activatedDronesByScene = new SerializableDictionary<string, SerializableIntList>();
        lastVisitedDrone = new DroneIdentifier { sceneName = "", droneID = -1 };
        completedEventFlags = new EventFlagsSystem.EventFlag[0];

        tutorialTexts = new SerializableDictionary<int, bool>();
        itemsPickedUp = new SerializableDictionary<int, bool>();
        inventoryItems = new SerializableDictionary<string, int>();
    }
}