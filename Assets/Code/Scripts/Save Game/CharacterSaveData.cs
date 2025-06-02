using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Chcemy zeby ten skrypt by³ serializowalny, poniewa¿ bêdziemy go u¿ywaæ do zapisywania danych o kilku postaciach
public class CharacterSaveData
{
    [Header("SCENE INDEX")]
    public int sceneIndex = 1;
    public string sceneName;

    [Header("Character Name")]
    public string characterName = "Character";

    // Czemu nie Vector3? Poniewa¿ nie mo¿emy zapisaæ Vector3 w JSON, poniewa¿ nie jest to typ podstawowy
    [Header("World Coordinates")]
    public float xPosition = 0f;
    public float yPosition = 2f;
    public float zPosition = 0f;

    [Header("Resources")]
    public float currentHealth;

    [Header("Campfires")]
    public SerializableDictionary<int, bool> activeDrones;
    public int lastVisitedDroneIndex = -1;
    public string lastVisitedDroneName = "No drones visited";

    [Header("Events")]
    public EventFlagsSystem.EventFlag[] completedEventFlags;

    [Header("Objectives")]
    public SerializableMission[] serializableMission;
    public SerializableMission currentMission;

    public CharacterSaveData()
    {
        activeDrones = new SerializableDictionary<int, bool>();
        completedEventFlags = new EventFlagsSystem.EventFlag[0];
    }
}
