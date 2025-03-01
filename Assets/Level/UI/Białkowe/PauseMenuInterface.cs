using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
public class MenuPauseUI : MonoBehaviour
{
    private VisualElement root;
    private Button resumeButton;
    private Button settingsButton;
    private Button quitMenuButton;
    private Button quitButton;

    private void OnEnable()
    {
        // Pobieramy referencję do UI Document
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("Brak komponentu UIDocument na obiekcie!");
            return;
        }

        root = uiDocument.rootVisualElement;

        // Pobieramy referencje do przycisków z UXML
        resumeButton = root.Q<Button>("resumeButton");
        settingsButton = root.Q<Button>("settingsButton");
        quitMenuButton = root.Q<Button>("quitMenuButton");
        quitButton = root.Q<Button>("quitButton");

        // Przypisujemy funkcje do przycisków
        if (resumeButton != null) resumeButton.clicked += () => Debug.Log("wkleic komende do resume");
        if (settingsButton != null) settingsButton.clicked += () => Debug.Log("wkleic komende do Settings");
        if (quitMenuButton != null) quitMenuButton.clicked += () => Debug.Log("wkleic komende do Quit to Menu");
        if (quitButton != null) quitButton.clicked += () => SettingsMenuController.ExitGame();
    }

    private void OnDisable()
    {
        // Usuwamy eventy, aby uniknąć memory leak
        if (resumeButton != null) resumeButton.clicked -= () => Debug.Log("wkleic komende do resume");
        if (settingsButton != null) settingsButton.clicked -= () => Debug.Log("Kliknięto Settings");
        if (quitMenuButton != null) quitMenuButton.clicked -= () => Debug.Log("Kliknięto Quit to Menu");
        if (quitButton != null) quitButton.clicked -= () => Debug.Log("wkleic komende do Quit Game");
    }
}
