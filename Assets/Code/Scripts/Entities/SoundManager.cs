using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public List<AudioClip> soundList;
    private AudioSource _audioSourceComponent;
    public bool isPlaying;

    private void Awake()
    {
        _audioSourceComponent = GetComponent<AudioSource>();

        if (!_audioSourceComponent)
        {
            Debug.LogError("Not found audio source component in: " + gameObject.name);
        }
    }

    private void Update()
    {
        _audioSourceComponent.volume = AudioListener.volume;
    }

    public void PlaySound(int clipIndex, Enums.SoundType soundType = Enums.SoundType.Master)
    {
        if (clipIndex < 0 || clipIndex >= soundList.Count)
        {
            Debug.LogError("Invalid clip index: " + clipIndex);
            return;
        }

        isPlaying = true;
        AudioClip clip = soundList[clipIndex];
        _audioSourceComponent.clip = soundList[clipIndex];

        var sfxManager = WorldSoundFXManager.instance;

        switch (soundType)
        {
            case Enums.SoundType.Master:
                _audioSourceComponent.volume = sfxManager.masterVolume;
                break;
            case Enums.SoundType.SFX:
                _audioSourceComponent.volume = sfxManager.masterVolume * sfxManager.sfxVolume;
                break;
            case Enums.SoundType.Music:
                _audioSourceComponent.volume = sfxManager.masterVolume * sfxManager.musicVolume;
                break;
            case Enums.SoundType.Dialogue:
                _audioSourceComponent.volume = sfxManager.masterVolume * sfxManager.dialogueVolume;
                break;
            default:
                _audioSourceComponent.volume = sfxManager.masterVolume;
                break;
        }

        _audioSourceComponent.Play();
        
        StartCoroutine(ClearClipAfterPlayback(clip.length));
    }

    private IEnumerator ClearClipAfterPlayback(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        isPlaying = false;
    }
}
