using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShivernAudioController : MonoBehaviour
{
    SoundManager soundManager;

    private void Start()
    {
        soundManager = GetComponent<SoundManager>();
    }


    public void PlayAttackSound()
    {
        //int randomIndex = Random.Range(0, 1);
        //soundManager.PlaySound(randomIndex, Enums.SoundType.SFX);
        if (WorldSoundFXManager.instance == null) return;
        float randomPitch = Random.Range(0.8f, 1.2f);
        WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.ShDogAttackSFX, Enums.SoundType.SFX, randomPitch);
    }

    public void PlayDeathSound()
    {
        //int randomIndex = Random.Range(2, 3);
        //soundManager.PlaySound(randomIndex);
        if (WorldSoundFXManager.instance == null) return;
        float randomPitch = Random.Range(0.8f, 1.2f);
        WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.ShDogDeathSFX, Enums.SoundType.SFX, randomPitch);
    }

    public void PlayDamageTakenSound()
    {
        //soundManager.PlaySound(4);
        if (WorldSoundFXManager.instance == null) return;
        float randomPitch = Random.Range(0.8f, 1.2f);
        WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.ShDogWhineSFX, Enums.SoundType.SFX, randomPitch);
    }
}
