using UnityEngine;
using UnityEngine.UIElements;

public class SettingsMenuController : MonoBehaviour
{
    private Button resolutionButton;
    private Button fullscreenButton;
    private Button soundButton;
    private Button resetButton;
    private Button exitButton;

    private bool isFullscreen;
    private bool isLowRes;

    void OnEnable()
    {
        
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Pobranie referencji do przycisków
        resolutionButton = root.Q<Button>("resolutionButton");
        fullscreenButton = root.Q<Button>("fullscreenButton");
        soundButton = root.Q<Button>("soundButton");
        resetButton = root.Q<Button>("resetButton");
        exitButton = root.Q<Button>("exitButton");

        // Wczytaj ustawienia z PlayerPrefs
        Vector2 currentResolution = OptionsManager.GetResoulution();
        isLowRes = (currentResolution.x == 800 && currentResolution.y == 600);
        isFullscreen = Screen.fullScreen;

        resolutionButton.text = $"{(isLowRes ? "800 x 600" : "1920 x 1080")}";
        fullscreenButton.text = isFullscreen ? "On" : "Off";

        // Przypisanie eventów
        resolutionButton.clicked += ToggleResolution;
        fullscreenButton.clicked += ToggleFullscreen;
        soundButton.clicked += SoundSettings;
        resetButton.clicked += ResetSettings;
        exitButton.clicked += ExitGame;
    }

    void ToggleResolution()
    {
        isLowRes = !isLowRes;
        int width = isLowRes ? 800 : 1920;
        int height = isLowRes ? 600 : 1080;

        OptionsManager.SetResoulution(width, height);
        resolutionButton.text = $"{width} x {height}";
        Debug.Log($"Rozmiar ekranu: {width} x {height}");
    }

    void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        OptionsManager.SetFullScreen(isFullscreen);
        fullscreenButton.text = isFullscreen ? "On" : "Off";
        Debug.Log($"Fullscreen: {(isFullscreen ? "On" : "Off")}");
    }

    void SoundSettings()
    {
        Debug.Log("Wkleic zmiane ekranu na SOUND UI");
    }

    void ResetSettings()
    {
        OptionsManager.ResetToDefault();

       
        isLowRes = true;
        isFullscreen = false;

        resolutionButton.text = "800 x 600";
        fullscreenButton.text = "Off";

        Debug.Log("Reset ustawien");
    }

    public static void ExitGame()
    {
        Debug.Log("Exit button pressed");
        Application.Quit();
    }
}
