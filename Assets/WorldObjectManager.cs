using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour
{
    // Klasa nadrzêdna dla wszystkich obiektów z którymi mo¿na wchodziæ w interakcjê
    public List<Interactable> worldObjects;

    // Pozosta³e obiekty z interakcj¹
    public List<InteractableCampfire> interactableCampfires;

    private void Start()
    {
        interactableCampfires = new List<InteractableCampfire>(FindObjectsByType<InteractableCampfire>(FindObjectsSortMode.None));

        foreach (InteractableCampfire campfire in interactableCampfires)
        {
            worldObjects.Add(campfire);
        }
    }
}
