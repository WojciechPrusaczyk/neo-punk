using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class TimeTrialEndScreenInterface : MonoBehaviour
{
    public Sprite[] MaskSprite;
    
    private VisualElement panel;
    private Button exitButton;
    public Label timerLabel;
    private Label resultLabel;
    public VisualElement mask;

    
    public TimeTrial timeTrial;
    
    private void OnEnable()
    {
        Cursor.visible = true;
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
        timerLabel = panel.Q<Label>("Time");
        resultLabel = panel.Q<Label>("Result");
        mask = panel.Q<VisualElement>("Mask");
        
        
        // Przypisujemy funkcje do przycisków
        if (exitButton != null) exitButton.clicked += () => gameObject.SetActive(false);
        
        if (timerLabel != null) timerLabel.text = "00:00.0";
        
        mask.style.backgroundImage = DecideMaskSprite().texture;
        
        if (timeTrial.trialFinished)
        {
            resultLabel.text = "WIN";
            resultLabel.style.color = Color.green;
        }
        else if (timeTrial.trialFinished == false)
        {
            resultLabel.text = "FAIL";
            resultLabel.style.color = new Color(110 / 255f, 24 / 255f, 37 / 255f, 1f);
        }
    }
    
    private void OnDisable()
    {
        if (exitButton != null) exitButton.clicked -= () => Debug.Log("exit trial");
        Cursor.visible = false;
    }

    public Sprite DecideMaskSprite()
    {
        if (timeTrial.trialTime < timeTrial.medalTimes[0])
        {
            return MaskSprite[0];
        }
        if (timeTrial.trialTime < timeTrial.medalTimes[1])
        {
            return MaskSprite[1];
        }
        if (timeTrial.trialTime < timeTrial.medalTimes[2])
        {
            return MaskSprite[2];
        }
        
        return MaskSprite[3];
    }
}
