using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UserInterfaceController : MonoBehaviour
{
    [Serializable]
    private class Interface
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

    [SerializeField]
    private int DefaultInterface = 0;
    private bool CanPlayerQuitToDefault = true;

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
        foreach (var interfaceObject in Interfaces)
        {
            if (interfaceObject.interfaceRoot.activeSelf)
            {
                interfaceObject.interfaceRoot.SetActive(false);
            }

            if (interfaceObject.interfaceRoot == interfaceToActivate)
            {
                if (Interfaces.Count > 0 && interfaceObject.ShowsOnTopOfMainUi)
                {
                    Interfaces[0].interfaceRoot.SetActive(true);
                }

                Cursor.visible = interfaceObject.ShowCursor;
                Time.timeScale = interfaceObject.FreezeGame?0:1;
            }
        }
        interfaceToActivate.SetActive(!interfaceToActivate.activeSelf);
    }

    public void ActivateInterface(int interfaceToActivateIndex)
    {
        if (interfaceToActivateIndex < Interfaces.Count)
            ActivateInterface(Interfaces[interfaceToActivateIndex].interfaceRoot);
        else Debug.Log("Invalid interface index.");
    }
}
