using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PauseMenuBehaviour : MonoBehaviour
{
    // Start is called before the first frame update

    private UserInterfaceController _userInterfaceController;
    public GameObject userInterfaceRoot;
    public GameObject optionsMenu;

    private void Awake()
    {
        _userInterfaceController = userInterfaceRoot.gameObject.GetComponent<UserInterfaceController>();
    }

    private void OnEnable()
    {
        // Ładujemy UXML
        var uiDocument = GetComponent<UIDocument>();
        var InterfaceRoot = uiDocument.rootVisualElement;

        /*
         * Przyciski menu
         */
        var resumeButton = InterfaceRoot.Q<Button>("resumeButton");
        var settingsButton = InterfaceRoot.Q<Button>("settingsButton");
        var quitMenuButton = InterfaceRoot.Q<Button>("quitMenuButton");
        var quitButton = InterfaceRoot.Q<Button>("quitButton");

        resumeButton.clicked += buttonResume;
        settingsButton.clicked += buttonOptions;
        quitMenuButton.clicked += buttonQuitToMenu;
        quitButton.clicked += buttonQuitGame;

        // Sound on hover
        if (WorldSoundFXManager.instance != null)
        {
            resumeButton.RegisterCallback<MouseEnterEvent>(e => WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonHoverSFX));
            settingsButton.RegisterCallback<MouseEnterEvent>(e => WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonHoverSFX));
            quitMenuButton.RegisterCallback<MouseEnterEvent>(e => WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonHoverSFX));
            quitButton.RegisterCallback<MouseEnterEvent>(e => WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonHoverSFX));
        }
    }

    public void buttonResume()
    {
        _userInterfaceController.ActivateInterface(0);

        if (WorldSoundFXManager.instance != null)
            WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonBackSFX);
    }

    public void buttonOptions()
    {
        _userInterfaceController.ActivateInterface(optionsMenu);

        if (WorldSoundFXManager.instance != null)
            WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonClickSFX);
    }

    public void buttonQuitToMenu()
    {
        if (WorldSoundFXManager.instance != null)
            WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonClickSFX);

        SceneManager.LoadScene("MainMenu");
    }

    public void buttonQuitGame()
    {
        if (WorldSoundFXManager.instance != null)
            WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonClickSFX);

        Application.Quit();
    }
}