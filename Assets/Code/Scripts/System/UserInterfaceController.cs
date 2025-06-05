using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static Enums;

public class UserInterfaceController : MonoBehaviour
{
    public static UserInterfaceController instance;

    public bool isUIMenuActive
    {
        get
        {
            if (Interfaces.Count == 0)
                return true;
            return !Interfaces[DefaultInterface].interfaceRoot.activeSelf;
        }
    }

    [Serializable]
    public class Interface
    {
        public GameObject interfaceRoot;
        public KeyCode KeyboardTrigger;
        public KeyCode ControlerTrigger;
        public bool FreezeGame;
        public bool ShowsOnTopOfMainUi;
        public bool ShowCursor = false;
    }

    // Pierwszym elementem ui musi ZAWSZE być main ui
    [SerializeField]
    private List<Interface> Interfaces = new List<Interface>();

    public List<Interface> GetInterfaces()
    {
        return Interfaces;
    }

    [SerializeField]
    private int DefaultInterface = 0;
    private bool CanPlayerQuitToDefault = true;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (Interfaces.Count == 0)
        {
            Debug.LogError("No interfaces defined in UserInterfaceController.");
            return;
        }
        if (DefaultInterface < 0 || DefaultInterface >= Interfaces.Count)
        {
            Debug.LogError("Default interface index is out of range.");
            DefaultInterface = 0;
        }
    }

    private void Start()
    {
        foreach (var interfaceObject in Interfaces)
        {
            interfaceObject.interfaceRoot.SetActive(false);
        }
        ActivateInterface(DefaultInterface);
    }

    void Update()
    {
        if ( !Interfaces[DefaultInterface].interfaceRoot.activeSelf && CanPlayerQuitToDefault && (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.Escape)))
        {
            ActivateInterface(DefaultInterface);
            if (WorldGameManager.instance.player.isInDialogue)
                WorldGameManager.instance.player.isInDialogue = false;

        }
        else
        {
            foreach (var interfaceObject in Interfaces)
            {
                if ((interfaceObject.KeyboardTrigger != KeyCode.None && Input.GetKeyDown(interfaceObject.KeyboardTrigger)) ||
                    (interfaceObject.ControlerTrigger != KeyCode.None && Input.GetKeyDown(interfaceObject.ControlerTrigger)))
                    ActivateInterface(interfaceObject.interfaceRoot);
            }
        }
    }

    public void ActivateInterface(GameObject interfaceToActivate)
    {
        if (interfaceToActivate == null)
        {
            Debug.LogError("Provided argument to UserInterfaceController.ActivateInterface is null.");
            return;
        }

        // Najpierw wyłącz wszystkie inne
        foreach (var interfaceObject in Interfaces)
        {
            if (interfaceObject.interfaceRoot == null)
            {
                Debug.LogError("interfaceRoot is null in Interfaces list.");
                continue;
            }

            if (interfaceObject.interfaceRoot != interfaceToActivate)
                interfaceObject.interfaceRoot.SetActive(false);
        }

        // Znajdź konfigurację interfejsu, który aktywujemy
        foreach (var interfaceObject in Interfaces)
        {
            if (interfaceObject.interfaceRoot == interfaceToActivate)
            {
                if (interfaceObject.ShowsOnTopOfMainUi && Interfaces.Count > 0 && Interfaces[0].interfaceRoot != null)
                    Interfaces[0].interfaceRoot.SetActive(true);

                Cursor.visible = interfaceObject.ShowCursor;
                Time.timeScale = interfaceObject.FreezeGame ? 0 : 1;

                if (WorldSoundFXManager.instance != null)
                    WorldSoundFXManager.instance.gameState = interfaceObject.FreezeGame ? GameState.Paused : GameState.Unpaused;

                if (MusicManager.instance != null)
                    MusicManager.instance.ApplyLowPassFilter(interfaceObject.FreezeGame);

                interfaceObject.interfaceRoot.SetActive(true);
                break;
            }
        }
    }

    public void ActivateInterface(int interfaceToActivateIndex)
    {
        if (interfaceToActivateIndex < Interfaces.Count)
            ActivateInterface(Interfaces[interfaceToActivateIndex].interfaceRoot);
        else Debug.Log("Invalid interface index.");
    }
}
