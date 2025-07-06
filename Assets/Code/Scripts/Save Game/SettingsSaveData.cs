using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Chcemy zeby ten skrypt by³ serializowalny, poniewa¿ bêdziemy go u¿ywaæ do zapisywania danych o kilku postaciach
public class SettingsSaveData
{
    [Header("Game SFX Volume Settings")]
    public float masterVolume = .5f;
    public float sfxVolume = .5f;
    public float musicVolume = .5f;
    public float dialogueVolume = .5f;

    public bool tooltipsEnabled = true;

    public SettingsSaveData()
    {
        // Tu zapisane bêd¹ typy zaawansowane (np. Lista, SerializableDictionary)
    }
}
