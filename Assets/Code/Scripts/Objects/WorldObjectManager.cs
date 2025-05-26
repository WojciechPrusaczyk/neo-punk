using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldObjectManager : MonoBehaviour
{
    public static WorldObjectManager instance;

    // Klasa nadrzêdna dla wszystkich obiektów z którymi mo¿na wchodziæ w interakcjê
    public List<Interactable> worldObjects;

    // Pozosta³e obiekty z interakcj¹
    public List<InteractableCampfire> interactableCampfires;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // Aktualizacja listy obiektów przy zmianie sceny
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"OnSceneLoaded: {scene.name} - Mode: {mode}");
        if (scene.name != "MainMenu")
        {
            StartCoroutine(RecalculateListsAfterSceneLoadRoutine());
            Debug.Log("Recalculating lists after scene load...");
        }
        else
        {
            interactableCampfires.Clear();
            worldObjects.Clear();
        }
    }

    private IEnumerator RecalculateListsAfterSceneLoadRoutine()
    {
        yield return new WaitForEndOfFrame();
        ReCalculateLists();
    }

    private void Start()
    {
    }

    public void ReCalculateLists()
    {
        interactableCampfires = new List<InteractableCampfire>(FindObjectsByType<InteractableCampfire>(FindObjectsSortMode.None));

        worldObjects.Clear();

        foreach (InteractableCampfire campfire in interactableCampfires)
        {
            worldObjects.Add(campfire);
        }
    }

    public InteractableCampfire GetCampfireByID(int id)
    {
        foreach (InteractableCampfire campfire in interactableCampfires)
        {
            if (campfire.ID == id)
            {
                return campfire;
            }
        }
        return null;
    }
}
