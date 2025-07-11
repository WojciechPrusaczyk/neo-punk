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

    public Toggle tooltipsToggle;

    private void Awake()
    {
        _userInterfaceController = userInterfaceRoot.gameObject.GetComponent<UserInterfaceController>();
    }

    private void OnEnable()
    {
        // Ładujemy UXML
        var uiDocument = GetComponent<UIDocument>();
        var InterfaceRoot = uiDocument.rootVisualElement;
        tooltipsToggle = InterfaceRoot.Q<Toggle>("Toggle");

        tooltipsToggle.value = UserInterfaceController.instance.tooltipsEnabled;

        /*
         * Przyciski
         */
        var gameplayOptionButton = InterfaceRoot.Q<Button>("gameplayOptionButton");
        var soundOptionButton = InterfaceRoot.Q<Button>("soundOptionButton");
        var controlsOptionButton = InterfaceRoot.Q<Button>("controlsOptionButton");
        var displayOptionButton = InterfaceRoot.Q<Button>("displayOptionButton");
        var exitButton = InterfaceRoot.Q<Button>("exitButton");

        // Slidery w zakładce sound tab
        var masterVolumeSlider = InterfaceRoot.Q<Slider>("MasterSlider");
        var sfxVolumeSlider = InterfaceRoot.Q<Slider>("EffectsSlider");
        var musicVolumeSlider = InterfaceRoot.Q<Slider>("MusicSlider");
        var dialogueVolumeSlider = InterfaceRoot.Q<Slider>("DialoguesSlider");

        // slidery defaultowo otrzymują wartość z WorldSoundFXManager.instance.<>
        if (WorldSoundFXManager.instance != null)
        {
            masterVolumeSlider.value = WorldSoundFXManager.instance.masterVolume;
            sfxVolumeSlider.value = WorldSoundFXManager.instance.sfxVolume;
            musicVolumeSlider.value = WorldSoundFXManager.instance.musicVolume;
            dialogueVolumeSlider.value = WorldSoundFXManager.instance.dialogueVolume;

            // Play sound on hover
            gameplayOptionButton.RegisterCallback<MouseEnterEvent>(e => WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonHoverSFX, Enums.SoundType.SFX));
            soundOptionButton.RegisterCallback<MouseEnterEvent>(e => WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonHoverSFX, Enums.SoundType.SFX));
            controlsOptionButton.RegisterCallback<MouseEnterEvent>(e => WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonHoverSFX, Enums.SoundType.SFX));
            displayOptionButton.RegisterCallback<MouseEnterEvent>(e => WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonHoverSFX, Enums.SoundType.SFX));
            exitButton.RegisterCallback<MouseEnterEvent>(e => WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonHoverSFX, Enums.SoundType.SFX));

            tooltipsToggle.RegisterValueChangedCallback(evt =>
            {
                _userInterfaceController.tooltipsEnabled = evt.newValue;
                WorldSaveGameManager.instance.SaveSettings();
            });

            // Dodajemy listenera do sliderów, upewniamy się, że wartość jest w zakresie 0-1
            masterVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                WorldSoundFXManager.instance.masterVolume = Mathf.Clamp(evt.newValue, 0f, 1f);
                WorldSaveGameManager.instance.SaveSettings();
            });

            sfxVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                WorldSoundFXManager.instance.sfxVolume = Mathf.Clamp(evt.newValue, 0f, 1f);
                WorldSaveGameManager.instance.SaveSettings();
            });

            musicVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                WorldSoundFXManager.instance.musicVolume = Mathf.Clamp(evt.newValue, 0f, 1f);
                WorldSaveGameManager.instance.SaveSettings();
            });

            dialogueVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                WorldSoundFXManager.instance.dialogueVolume = Mathf.Clamp(evt.newValue, 0f, 1f);
                WorldSaveGameManager.instance.SaveSettings();
            });
        }
        else
        {
            Debug.Log("Nie znaleziono instancji WorldSoundFXManager. Dźwięki nie będą odtwarzane!");
        }


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
            if (WorldSoundFXManager.instance != null)
                WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonClickSFX, Enums.SoundType.SFX);
        };
        soundOptionButton.clicked += () =>
        {
            _gameplayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.Flex;
            if (WorldSoundFXManager.instance != null)
                WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonClickSFX, Enums.SoundType.SFX);
        };
        controlsOptionButton.clicked += () =>
        {
            _gameplayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.Flex;
            if (WorldSoundFXManager.instance != null)
                WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonClickSFX, Enums.SoundType.SFX);
        };
        displayOptionButton.clicked += () =>
        {
            _gameplayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.Flex;
            if (WorldSoundFXManager.instance != null)
                WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonClickSFX, Enums.SoundType.SFX);
        };
        exitButton.clicked += () =>
        {
            _gameplayTab.style.display = DisplayStyle.None;
            _soundTab.style.display = DisplayStyle.None;
            _controlsTab.style.display = DisplayStyle.None;
            _displayTab.style.display = DisplayStyle.None;
            _userInterfaceController.ActivateInterface(pauseMenu);
            if (WorldSoundFXManager.instance != null)
                WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonBackSFX, Enums.SoundType.SFX);
        };
    }

    private void deactivateAllTabs()
    {
    }
}
