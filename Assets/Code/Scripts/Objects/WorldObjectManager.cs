using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldObjectManager : MonoBehaviour
{
    public static WorldObjectManager instace;

    // Klasa nadrz�dna dla wszystkich obiekt�w z kt�rymi mo�na wchodzi� w interakcj�
    public List<Interactable> worldObjects;

    // Pozosta�e obiekty z interakcj�
    public List<InteractableCampfire> interactableCampfires;

    private void Awake()
    {
        if (instace == null)
        {
            instace = this;
            DontDestroyOnLoad(gameObject);
            // Aktualizacja listy obiekt�w przy zmianie sceny
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
        if (instace == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"OnSceneLoaded: {scene.name} - Mode: {mode}");
        if (scene.name != "MainMenu")
        {
            StartCoroutine(RecalculateListsAfterSceneLoadRoutine(scene));
            Debug.Log("Recalculating lists after scene load...");
        }
        else
        {
            interactableCampfires.Clear();
            worldObjects.Clear();
        }
    }

    private IEnumerator RecalculateListsAfterSceneLoadRoutine(Scene scene)
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
