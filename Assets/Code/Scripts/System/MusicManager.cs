using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Level Music Tracks")]
    public AudioClip MainMenuTrack;
    public AudioClip Level1Track;

    [Header("Boss Music Tracks")]
    public AudioClip Boss1Track;

    [Header("Low Pass Filter Settings")]
    [SerializeField] private float normalCutoffFrequency = 22000f;
    [SerializeField] private float filteredCutoffFrequency = 700f;
    [SerializeField] private float filterTransitionDuration = 0.3f;
    private AudioLowPassFilter _lowPassFilter;
    private Coroutine _filterCoroutine;

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

        // On scene load
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {

    }

    public void PlaySong(AudioClip song)
    {
        if (_audioSourceComponent == null)
            return;

        if (_audioSourceComponent.isPlaying)
            _audioSourceComponent.Stop();

        _audioSourceComponent.clip = song;
        _audioSourceComponent.volume = WorldSoundFXManager.instance == null ? AudioListener.volume : Mathf.Clamp01(WorldSoundFXManager.instance.musicVolume * WorldSoundFXManager.instance.masterVolume);
        _audioSourceComponent.loop = true;

        _audioSourceComponent.Play();
    }

    public void RestartSong()
    {
        if (_audioSourceComponent == null || _audioSourceComponent.clip == null)
            return;
        _audioSourceComponent.Stop();
        _audioSourceComponent.time = 0f;
        _audioSourceComponent.Play();
    }

    public void ApplyLowPassFilter(bool applyFilter)
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

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyLowPassFilter(false);
        switch (scene.name)
        {
            case "MainMenu":
                PlaySong(MainMenuTrack);
                break;
            case "InitialLevel":
                PlaySong(Level1Track);
                break;
            default:
                Debug.LogWarning("No music track assigned for scene: " + scene.name);
                break;
        }
    }
}