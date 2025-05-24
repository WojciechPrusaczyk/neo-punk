using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        interactableCampfires = new List<InteractableCampfire>(FindObjectsByType<InteractableCampfire>(FindObjectsSortMode.None));

        foreach (InteractableCampfire campfire in interactableCampfires)
        {
            worldObjects.Add(campfire);
        }
    }

    public void ReCalculateLists()
    {   
        interactableCampfires.Clear();
        interactableCampfires = new List<InteractableCampfire>(FindObjectsByType<InteractableCampfire>(FindObjectsSortMode.None));
        worldObjects.Clear();
        worldObjects.AddRange(interactableCampfires);
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
