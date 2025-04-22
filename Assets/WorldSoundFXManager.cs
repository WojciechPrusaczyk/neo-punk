using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    [HideInInspector] public static WorldSoundFXManager instance;

    [Header("Player Sounds")]
    public AudioClip playerJumpSFX;
    public AudioClip[] playerAttackSFX;

    [Header("Shivern Dog Sounds")]
    public AudioClip[] ShDogAttackSFX;
    public AudioClip[] ShDogDeathSFX;
    public AudioClip[] ShDogWhineSFX;


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
    }

    public void PlaySoundFX(AudioClip clip, float volume, float pitch)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.Play();

        Destroy(audioSource, clip.length);
    }

    public void ChooseRandomSFXFromArray(AudioClip[] clip, float volume, float pitch)
    {
        if (clip.Length == 0)
        {
            Debug.LogWarning($"No audio clips available in the array.");
            return;
        }

        int randomIndex = Random.Range(0, clip.Length);
        AudioClip randomClip = clip[randomIndex];
        PlaySoundFX(randomClip, volume, pitch);
    }
}
