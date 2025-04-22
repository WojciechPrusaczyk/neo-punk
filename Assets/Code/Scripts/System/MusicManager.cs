using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Music tracks list")]
    public List<AudioClip> musicList;

    [Header("Force of turning volume up and down, when changing song.")]
    [Range(0.0f, 0.1f)]
    public float audioChangeForce;

    [Header("Low Pass Filter Settings")]
    [SerializeField] private float normalCutoffFrequency = 22000f;
    [SerializeField] private float filteredCutoffFrequency = 700f;
    [SerializeField] private float filterTransitionDuration = 0.3f;
    private AudioLowPassFilter _lowPassFilter;
    private Coroutine _filterCoroutine;

    private bool isChangingSong = false;
    private List<AudioClip> songsQueue;
    private AudioSource _audioSourceComponent;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        _audioSourceComponent = GetComponent<AudioSource>();
        if (null == _audioSourceComponent)
        {
            Debug.LogError("Audio source component is missing in music manager.");
        }
        else
        {
            _audioSourceComponent.volume = WorldSoundFXManager.instance == null ? AudioListener.volume : Mathf.Clamp01(WorldSoundFXManager.instance.musicVolume * WorldSoundFXManager.instance.masterVolume);
        }

        _lowPassFilter = GetComponent<AudioLowPassFilter>();
        if (_lowPassFilter == null)
        {
            _lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
        }
        _lowPassFilter.cutoffFrequency = normalCutoffFrequency;

        songsQueue = new List<AudioClip>();
    }

    private void Update()
    {
        if (_audioSourceComponent != null && !isChangingSong)
        {
            if (WorldSoundFXManager.instance != null)
            {
                float masterSetting = WorldSoundFXManager.instance.masterVolume;
                float musicSetting = WorldSoundFXManager.instance.musicVolume;
                float targetVolume = Mathf.Clamp01(masterSetting * musicSetting);
                _audioSourceComponent.volume = targetVolume;
            }

            float endThreshold = 0.1f;
            if (_audioSourceComponent.clip != null &&
                _audioSourceComponent.isPlaying &&
                _audioSourceComponent.time >= _audioSourceComponent.clip.length - endThreshold)
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
                StartCoroutine(ChangeSong(musicList[id], true));
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
        float fadeDuration = 0.75f;
        float startingSourceVolume = _audioSourceComponent.volume;
        float targetVolume = 0f;

        if (WorldSoundFXManager.instance != null)
        {
            targetVolume = Mathf.Clamp01(WorldSoundFXManager.instance.masterVolume * WorldSoundFXManager.instance.musicVolume);
        }
        else
        {
            targetVolume = Mathf.Clamp01(_audioSourceComponent.volume);
            Debug.LogWarning("WorldSoundFXManager instance missing, using current AudioSource volume as target.");
        }

        if (!startImmediately && _audioSourceComponent.isPlaying)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                _audioSourceComponent.volume = Mathf.Lerp(startingSourceVolume, 0f, timer / fadeDuration);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            _audioSourceComponent.volume = 0f;
            _audioSourceComponent.Stop();
        }
        else if (!startImmediately)
        {
            _audioSourceComponent.Stop();
            _audioSourceComponent.volume = 0f;
        }
        _audioSourceComponent.clip = nextSong;
        _audioSourceComponent.volume = 0f;
        _audioSourceComponent.Play();

        float fadeInStartVolume = 0f;
        float timerFadeIn = 0f;
        if (WorldSoundFXManager.instance != null)
        {
            targetVolume = Mathf.Clamp01(WorldSoundFXManager.instance.masterVolume * WorldSoundFXManager.instance.musicVolume);
        }
        else
        {
            targetVolume = 1.0f;
        }

        while (timerFadeIn < fadeDuration)
        {
            _audioSourceComponent.volume = Mathf.Lerp(fadeInStartVolume, targetVolume, timerFadeIn / fadeDuration);
            timerFadeIn += Time.unscaledDeltaTime;
            yield return null;
        }

        _audioSourceComponent.volume = targetVolume;

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

    public void PauseCurrentSong(bool applyFilter) // Changed parameter name for clarity
    {
        if (_lowPassFilter == null)
        {
            Debug.LogError("AudioLowPassFilter component is missing, cannot change filter state.");
            return;
        }

        float targetFrequency = applyFilter ? filteredCutoffFrequency : normalCutoffFrequency;

        if (_filterCoroutine != null)
        {
            StopCoroutine(_filterCoroutine);
        }

        if (filterTransitionDuration <= 0f || _lowPassFilter.cutoffFrequency == targetFrequency)
        {
            _lowPassFilter.cutoffFrequency = targetFrequency;
            _filterCoroutine = null;
        }
        else
        {
            _filterCoroutine = StartCoroutine(TransitionFilter(targetFrequency));
        }
    }

    private IEnumerator TransitionFilter(float targetFrequency)
    {
        float startFrequency = _lowPassFilter.cutoffFrequency;
        float timer = 0f;

        while (timer < filterTransitionDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer / filterTransitionDuration);
            _lowPassFilter.cutoffFrequency = Mathf.Lerp(startFrequency, targetFrequency, progress);
            yield return null;
        }

        _lowPassFilter.cutoffFrequency = targetFrequency;
        _filterCoroutine = null;
    }
}