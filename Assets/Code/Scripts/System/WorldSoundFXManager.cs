using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class WorldSoundFXManager : MonoBehaviour
{
    [HideInInspector] public static WorldSoundFXManager instance;
    [HideInInspector] public GameState gameState = GameState.Unpaused;

    [Header("Game SFX Volume Settings")]
    [Range(0.0f, 1.0f)]
    public float masterVolume = .5f;    
    [Range(0.0f, 1.0f)]
    public float sfxVolume = .5f;
    [Range(0.0f, 1.0f)]
    public float musicVolume = .5f;
    [Range(0.0f, 1.0f)]
    public float dialogueVolume = .5f;

    [Header("UI")]
    public AudioClip buttonHoverSFX;
    public AudioClip buttonClickSFX;
    public AudioClip buttonBackSFX;

    [Header("Player Sounds")]
    public AudioClip playerJumpSFX;
    public AudioClip playerParrySFX;
    public AudioClip dashSFX;
    public AudioClip[] playerBlockSFX;
    public AudioClip[] playerAttackSFX;

    [Header("Shivern Dog Sounds")]
    public AudioClip[] ShDogAttackSFX;
    public AudioClip[] ShDogDeathSFX;
    public AudioClip[] ShDogWhineSFX;

    [Header("Dragonfly")]
    public AudioClip[] dragonflyAttackSFX;
    public AudioClip attackSerie;
    public AudioClip[] dragonflyDeathSFX;


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

        Debug.Log("Enabled the instance");
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        WorldSaveGameManager.instance.AttemptToCreateNewSettingsFile();
        WorldSaveGameManager.instance.LoadSettingsFile();
    }

    private void Update()
    {
        masterVolume = Mathf.Clamp01(masterVolume);
        AudioListener.volume = masterVolume;
    }

    public void PlaySoundFX(AudioClip clip, Enums.SoundType soundType = Enums.SoundType.Master, float pitch = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;

        switch (soundType)
        {
            case Enums.SoundType.Master:
                audioSource.volume = masterVolume;
                break;
            case Enums.SoundType.SFX:
                audioSource.volume = masterVolume * sfxVolume;
                break;
            case Enums.SoundType.Music:
                audioSource.volume = masterVolume * musicVolume;
                break;
            case Enums.SoundType.Dialogue:
                audioSource.volume = masterVolume * dialogueVolume;
                break;
            default:
                audioSource.volume = masterVolume;
                break;
        }

        audioSource.pitch = pitch;

        audioSource.Play();

        Destroy(audioSource, clip.length);
    }

    public void ChooseRandomSFXFromArray(AudioClip[] clip, Enums.SoundType soundType = Enums.SoundType.Master, float pitch = 1f)
    {
        if (clip.Length == 0)
        {
            Debug.LogWarning($"No audio clips available in the array.");
            return;
        }

        int randomIndex = Random.Range(0, clip.Length);
        AudioClip randomClip = clip[randomIndex];
        PlaySoundFX(randomClip, soundType, pitch);
    }
}
