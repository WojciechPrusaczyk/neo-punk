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
        int randomIndex = Random.Range(0, 1);
        
        soundManager.PlaySound(randomIndex);
    }

    public void PlayDeathSound()
    {
        int randomIndex = Random.Range(2, 3);
        soundManager.PlaySound(randomIndex);
    }

    public void PlayDamageTakenSound()
    {
        soundManager.PlaySound(4);
    }
}
