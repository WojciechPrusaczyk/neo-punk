using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InvasionInterface : MonoBehaviour
{
    private VisualElement panel;
    private Button exitButton;
    public Label WaveState;
    public Label EnemiesState;
    
    InvasionTrial invasionTrial;
    
    private void OnEnable()
    {
        
        // Pobieramy referencję do UI Document
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("Brak komponentu UIDocument na obiekcie!");
            return;
        }
        panel = uiDocument.rootVisualElement;
        
        // Pobieramy referencje do przycisków z UXML
        WaveState = panel.Q<Label>("WaveState");
        EnemiesState = panel.Q<Label>("EnemiesState");
        
        
        // Przypisujemy funkcje do przycisków
        if (exitButton != null) exitButton.clicked += () => Debug.Log("Something something");
        
        
    }
    
    private void OnDisable()
    {
        // Usuwamy eventy, aby uniknąć memory leak
        if (exitButton != null) exitButton.clicked -= () => Debug.Log("Something something");
    }

    
}
