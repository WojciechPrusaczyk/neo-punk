using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeTrialEndScreenInterface : MonoBehaviour
{
    
    private VisualElement panel;
    private Button exitButton;
    private Button repeatButton;
    private Label timerLabel;
    private Label resultLabel;

    
    public TimeTrial timeTrial;
    
    private void OnEnable()
    {
        
        // Pobieramy referencję do UI Document
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("Brak komponentu UIDocument na obiekcie!");
            return;
        }
        Debug.Log("UIDocument na obiekcie!");
        panel = uiDocument.rootVisualElement;
        
        // Pobieramy referencje do przycisków z UXML
        exitButton = panel.Q<Button>("ExitButton");
        repeatButton = panel.Q<Button>("RepeatButton");
        timerLabel = panel.Q<Label>("Time");
        timerLabel = panel.Q<Label>("Result");
        
        
        // Przypisujemy funkcje do przycisków
        if (exitButton != null) exitButton.clicked += () => Debug.Log("wylacz ekran wojtek");
        if (repeatButton != null) repeatButton.clicked += () => Debug.Log("bedzie w becie");
        
        if (timerLabel != null) timerLabel.text = timeTrial.trialTime.ToString();
        
        if (timeTrial.trialFinished)
        {
            resultLabel.text = "WIN";
            resultLabel.style.backgroundColor = Color.green;
        }
        else if (timeTrial.trialFinished == false)
        {
            resultLabel.text = "FAIL";
            resultLabel.style.backgroundColor = Color.red;
        }
    }
    
    private void OnDisable()
    {
        // Usuwamy eventy, aby uniknąć memory leak
        if (exitButton != null) exitButton.clicked -= () => Debug.Log("exit trial");
        if (repeatButton != null) repeatButton.clicked -= () => Debug.Log("repeat trial");
    }
}
