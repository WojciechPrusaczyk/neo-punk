using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeTrialInterface : MonoBehaviour
{
    private VisualElement panel;
    private Button exitButton;
    private Label timerLabel;

    public Label goldTimeLabel;
    public Label silverTimeLabel;
    public Label bronzeTimeLabel;


    
    public TimeTrial timeTrial;

    public void OnEnable()
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
        exitButton = panel.Q<Button>("exitButton");
        timerLabel = panel.Q<Label>("timerLabel");
        
        goldTimeLabel = panel.Q<Label>("GoldTime");
        silverTimeLabel = panel.Q<Label>("SilverTime");
        bronzeTimeLabel = panel.Q<Label>("BronzeTime");
        
        
        // Przypisujemy funkcje do przycisków
        if (exitButton != null) exitButton.clicked += () => timeTrial.ExitTrial();
    }
    
    private void OnDisable()
    {
        // Usuwamy eventy, aby uniknąć memory leak
        if (exitButton != null) exitButton.clicked -= () => timeTrial.ExitTrial();
    }

    private void Update()
    {
        string formatedTime = timeTrial.FormatTime(timeTrial.trialTime);
        timerLabel.text = formatedTime;
    }
}
