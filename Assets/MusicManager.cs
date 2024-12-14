using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Music tracks list")]
    public List<AudioClip> musicList;

    [Header("Force of turning volume up and down, when changing song.")]
    [Range(0.0f, 0.1f)]
    public float audioChangeForce;

    private bool isChangingSong = false;
    private List<AudioClip> songsQueue;
    private AudioSource _audioSourceComponent;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        _audioSourceComponent = GetComponent<AudioSource>();
        if (null == _audioSourceComponent)
        {
            Debug.LogError("Audio source component is missing in music manager.");
        }
        else
        {
            _audioSourceComponent.volume = AudioListener.volume;
        }

        songsQueue = new List<AudioClip>();
    }

    private void Update()
    {
        if (_audioSourceComponent && !isChangingSong)
        {
            _audioSourceComponent.volume = AudioListener.volume;

            float endThreshold = Mathf.Lerp(2.0f, 0.1f, audioChangeForce / 0.1f);
            
            if (_audioSourceComponent.clip != null && _audioSourceComponent.time >= _audioSourceComponent.clip.length - endThreshold)
            {
                PlayNextSongFromQueue();
            }
        }
    }

    public void PlaySong(int id)
    {
        if (_audioSourceComponent != null)
        {
            if (id >= 0 && id < musicList.Count)
            {
                StartCoroutine(ChangeSong(musicList[id]));
            }
            else
            {
                Debug.LogError("Invalid track id: " + id.ToString() + ".");
            }
        }
    }

    public void QueueSong(int id)
    {
        if (_audioSourceComponent != null)
        {
            if (id >= 0 && id < musicList.Count)
            {
                songsQueue.Add(musicList[id]);
            }
            else
            {
                Debug.LogError("Invalid track id: " + id.ToString() + ".");
            }
        }
    }

    public void ClearQueue()
    {
        songsQueue.Clear();
    }

    public void StopSong()
    {
        _audioSourceComponent.Stop();
        _audioSourceComponent.clip = null;
    }

    private IEnumerator ChangeSong(AudioClip nextSong, bool startImmediately = false)
    {
        isChangingSong = true;
        var initialVolume = AudioListener.volume;

        if ( !startImmediately )
        {
            while (AudioListener.volume > 0)
            {
                yield return new WaitForSeconds(0.1f);
                AudioListener.volume -= audioChangeForce;
            }
        }
        
        _audioSourceComponent.clip = nextSong;
        _audioSourceComponent.Play();

        while (AudioListener.volume < initialVolume)
        {
            yield return new WaitForSeconds(0.1f);
            AudioListener.volume += audioChangeForce;
        }

        AudioListener.volume = initialVolume;
        isChangingSong = false;
    }

    private void PlayNextSongFromQueue()
    {
        if (songsQueue.Count > 0)
        {
            var nextSong = songsQueue[0];
            songsQueue.RemoveAt(0);
            StartCoroutine(ChangeSong(nextSong));
        }
        else
        {
            Debug.Log("No more songs in the queue.");
        }
    }
}