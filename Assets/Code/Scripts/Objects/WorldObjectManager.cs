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
    public List<InteractableDrone> interactableDrones;

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
            interactableDrones.Clear();
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
        interactableDrones = new List<InteractableDrone>(FindObjectsByType<InteractableDrone>(FindObjectsSortMode.None));

        worldObjects.Clear();

        foreach (InteractableDrone drone in interactableDrones)
        {
            worldObjects.Add(drone);
        }
    }

    public InteractableDrone GetDroneByID(int id)
    {
        foreach (InteractableDrone drone in interactableDrones)
        {
            if (drone.ID == id)
            {
                return drone;
            }
        }
        return null;
    }
}
