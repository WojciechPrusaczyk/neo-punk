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

    public CharacterSaveData()
    {
        // Tu zapisane b�d� typy zaawansowane (np. Lista, SerializableDictionary)
    }
}
