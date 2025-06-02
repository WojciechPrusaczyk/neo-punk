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
                            backgroundImage = drone.backgroundImage.texture,
                        }
                };

                droneButton.focusable = false;
                droneButton.AddToClassList("drone-button");

                if (drone.ID != WorldSaveGameManager.instance.currentCharacterData.lastVisitedDroneIndex)
                {
                    droneButton.clicked += () => player.TeleportPlayerToDrone(drone.ID, drone.droneName);
                    droneButton.RegisterCallback<MouseEnterEvent>(evt =>
                    {
                        droneButton.style.unityBackgroundImageTintColor = new StyleColor(new Color(1f, 1f, 1f, .99f));
                    });

                    droneButton.RegisterCallback<MouseLeaveEvent>(evt =>
                    {
                        droneButton.style.unityBackgroundImageTintColor = new StyleColor(new Color(1f, 1f, 1f, 1f));
                    });

                    droneButton.AddToClassList("drone-button-enabled");

                }
                else
                {
                    droneButton.AddToClassList("drone-button-disabled");
                }

                activeDroneUIButtons.Add(droneButton);

                Label droneNameLabel = new Label(drone.droneName);
                droneNameLabel.AddToClassList("drone-button-label");
                droneButton.Add(droneNameLabel);

                VisualElement droneFrame = new VisualElement();
                droneFrame.AddToClassList("drone-frame");
                droneButton.Add(droneFrame);
            }
            else
            {
                Debug.LogError("Drone UI Error, root visual element not found.");
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
