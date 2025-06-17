using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
    public enum SoundType
    {
        Master,
        SFX,
        Music,
        Dialogue
    }

    public enum CharacterSlots
    {
        CharacterSlot1,
        CharacterSlot2,
        CharacterSlot3
    }

    public enum GameState
    {
        Paused,
        Unpaused
    }

    public enum ElementalType
    {
        Normal,
        Bloody,
        Storm,
        Flame,
        Air,
        Water
    }

    public enum EntityType
    {
        Human,
        Cyber,
        Demon
    }

    public enum EnemyType
    {
        ShivernDog,
        DragonFly,
        BossAbomination
    }

    public enum ItemRarity
    {
        Common,
        Rare,
        Quantum
    }
}
