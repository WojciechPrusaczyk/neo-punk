using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectManager : MonoBehaviour
{
    // Klasa nadrz�dna dla wszystkich obiekt�w z kt�rymi mo�na wchodzi� w interakcj�
    public List<Interactable> worldObjects;

    // Pozosta�e obiekty z interakcj�
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
