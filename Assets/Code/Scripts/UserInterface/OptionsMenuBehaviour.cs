using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class OptionsMenuBehaviour : MonoBehaviour
{
    private UserInterfaceController _userInterfaceController;
    private VisualElement _gameplayTab;
    private VisualElement _soundTab;
    private VisualElement _controlsTab;
    private VisualElement _displayTab;

    public GameObject userInterfaceRoot;
    public GameObject pauseMenu;

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
         * Przyciski
         */
        var gameplayOptionButton = InterfaceRoot.Q<Button>("gameplayOptionButton");
        var soundOptionButton = InterfaceRoot.Q<Button>("soundOptionButton");
        var controlsOptionButton = InterfaceRoot.Q<Button>("controlsOptionButton");
        var displayOptionButton = InterfaceRoot.Q<Button>("displayOptionButton");
        var exitButton = InterfaceRoot.Q<Button>("exitButton");

        deactivateAllTabs();

        /*
         * Zakładki
         */
        _gameplayTab = InterfaceRoot.Q<VisualElement>("TabGameplay");
        _soundTab = InterfaceRoot.Q<VisualElement>("TabSound");
        _controlsTab = InterfaceRoot.Q<VisualElement>("TabControls");
        _displayTab = InterfaceRoot.Q<VisualElement>("TabDisplay");

        gameplayOptionButton.clicked += () =>
        {
            _gameplayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.None;
            _gameplayTab.style.display = DisplayStyle.Flex;
        };
        soundOptionButton.clicked += () =>
        {
            _gameplayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.Flex;
        };
        controlsOptionButton.clicked += () =>
        {
            _gameplayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.Flex;
        };
        displayOptionButton.clicked += () =>
        {
            _gameplayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.Flex;
        };
        exitButton.clicked += () =>
        {
            _gameplayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.None;
            _userInterfaceController.ActivateInterface(pauseMenu);
        };
    }

    private void deactivateAllTabs()
    {
    }
}
