using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_DroneInterface_Controller : UI_InterfaceController
{
    public bool isDroneUIActive = false;

    public List<Button> activeDroneUIButtons = new List<Button>();

    private float width = 500f;
    private float height = 250f;

    protected override void OnEnable()
    {
        base.OnEnable();

        rootVisualElement = root.Q<VisualElement>("Drones");

        activeDroneUIButtons.Clear();

        if (WorldSaveGameManager.instance == null)
            return;

        // Tworzenie przycisk�w dla aktywnych ognisk
        if (rootVisualElement != null)
        {
            if (WorldObjectManager.instance == null)
                return;

            CreateDroneInterfaceUI();
        }
    }

    protected override void Update()
    {
    }

    private void CreateDroneInterfaceUI()
    {
        if (rootVisualElement != null)
            rootVisualElement.Clear();
        activeDroneUIButtons.Clear();

        int activeDroneAmount = 0;

        foreach (InteractableDrone drone in WorldObjectManager.instance.interactableDrones)
        {
            if (drone == null || !drone.isActivated)
                continue;
            activeDroneAmount++;
        }

        // Zmiana wielkości przycisk�w w zale�no�ci od ilo�ci aktywnych ognisk
        switch (activeDroneAmount)
        {
            case > 16:
                width = 300f;
                height = 150f;
                break;
            case > 9:
                width = 400f;
                height = 200f;
                break;
        }

        foreach (InteractableDrone drone in WorldObjectManager.instance.interactableDrones)
        {
            if (drone == null)
                continue;

            if (!drone.isActivated)
                continue;

            if (rootVisualElement != null)
            {
                Button droneButton = new Button()
                {
                    text = $"",
                    name = $"DroneButton_{drone.ID}",
                    style =
                        {
                            width = width,
                            height = height,
                            backgroundImage = drone.backgroundImage != null ? new StyleBackground(drone.backgroundImage) : new StyleBackground(Texture2D.whiteTexture),
                        }
                };

                droneButton.AddToClassList("drone-button");

                DroneIdentifier thisDroneIdentifier = new DroneIdentifier { sceneName = drone.gameObject.scene.name, droneID = drone.ID };

                if (thisDroneIdentifier != WorldSaveGameManager.instance.currentCharacterData.lastVisitedDrone)
                {
                    droneButton.clicked += () => player.TeleportPlayerToDrone(drone.ID, drone.droneName);

                    // Dodanie dźwięków do przycisków (enter, click)
                    droneButton.RegisterCallback<MouseEnterEvent>(evt =>
                    {
                        if (WorldSoundFXManager.instance != null && WorldSoundFXManager.instance.gameState != Enums.GameState.Paused)
                        {
                            WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonHoverSFX, Enums.SoundType.SFX);
                        }
                    });

                    droneButton.AddToClassList("drone-button-enabled");

                    // Dodanie frame'a do przycisku, który nie jest aktualnie używanym dronem
                    VisualElement droneFrame = new VisualElement();
                    droneFrame.AddToClassList("drone-button-border");
                    droneFrame.style.width = width;
                    droneFrame.style.height = height;
                    droneButton.Add(droneFrame);

                    Label droneNameLabel = new Label(drone.droneName);
                    droneNameLabel.AddToClassList("drone-button-label");
                    droneButton.Add(droneNameLabel);
                }
                else
                {
                    droneButton.AddToClassList("drone-button-disabled");

                    // Wyłączamy możliwość focusu, aby klawiatura nie mogła nawigować do tego przycisku
                    droneButton.focusable = false;

                    // Dodanie frame'a do przycisku, który jest aktualnie używanym dronem
                    VisualElement droneFrame = new VisualElement();
                    droneFrame.AddToClassList("drone-button-border-this");
                    droneFrame.style.width = width;
                    droneFrame.style.height = height;
                    droneButton.Add(droneFrame);

                    Label droneNameLabel = new Label($"{drone.droneName} - CURRENT");
                    droneNameLabel.AddToClassList("drone-button-label");
                    droneButton.Add(droneNameLabel);
                }

                activeDroneUIButtons.Add(droneButton);
            }
            else
            {
                Debug.LogError("Drone UI Error, root visual element not found.");
            }
        }

        activeDroneUIButtons.Sort((a, b) =>
        {
            int idA = int.Parse(a.name.Split('_')[1]);
            int idB = int.Parse(b.name.Split('_')[1]);
            return idA.CompareTo(idB);
        });

        foreach (Button button in activeDroneUIButtons)
        {
            rootVisualElement.Add(button);
        }
    }

    public override void ActivateInterface(int ID)
    {
        if (MainUiController == null || userInterfaceController == null)
        {
            Debug.LogError("Drone UI Error, MainUiController or UserInterfaceController is null.");
            return;
        }
        if (interfaceIndex < 0 || interfaceIndex >= userInterfaceController.GetInterfaces().Count)
        {
            Debug.LogError("Drone UI Error, invalid interface index.");
            return;
        }
        isDroneUIActive = true;
        userInterfaceController.ActivateInterface(userInterfaceController.GetInterfaces()[interfaceIndex].interfaceRoot);
    }

    public override void DeactivateInterface()
    {
        if (!isDroneUIActive)
            return;

        if (MainUiController == null || userInterfaceController == null)
        {
            Debug.LogError("Drone UI Error, MainUiController or UserInterfaceController is null.");
            return;
        }
        if (interfaceIndex < 0 || interfaceIndex >= userInterfaceController.GetInterfaces().Count)
        {
            Debug.LogError("Drone UI Error, invalid interface index.");
            return;
        }
        isDroneUIActive = false;
        userInterfaceController.ActivateInterface(userInterfaceController.GetInterfaces()[0].interfaceRoot);
    }
}