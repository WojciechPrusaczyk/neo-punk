using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void PlaySound(int clipIndex)
    {
        if (clipIndex < 0 || clipIndex >= soundList.Count)
        {
            Debug.LogError("Invalid clip index: " + clipIndex);
            return;
        }

        isPlaying = true;
        AudioClip clip = soundList[clipIndex];
        _audioSourceComponent.clip = soundList[clipIndex];
        _audioSourceComponent.Play();
        
        StartCoroutine(ClearClipAfterPlayback(clip.length));
    }

    private IEnumerator ClearClipAfterPlayback(float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        isPlaying = false;
    }
}
