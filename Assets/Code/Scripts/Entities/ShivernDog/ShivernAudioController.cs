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
        //soundManager.PlaySound(randomIndex);
        if (WorldSoundFXManager.instance == null) return;
        WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.ShDogAttackSFX, AudioListener.volume, 1f);
    }

    public void PlayDeathSound()
    {
        //int randomIndex = Random.Range(2, 3);
        //soundManager.PlaySound(randomIndex);
        if (WorldSoundFXManager.instance == null) return;
        WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.ShDogDeathSFX, AudioListener.volume, 1f);
    }

    public void PlayDamageTakenSound()
    {
        //soundManager.PlaySound(4);
        if (WorldSoundFXManager.instance == null) return;
        WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.ShDogWhineSFX, AudioListener.volume, 1f);
    }
}
