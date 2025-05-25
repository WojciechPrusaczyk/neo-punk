using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_CampfireInterface_Controller : UI_InterfaceController
{
    public bool isCampfireUIActive = false;

    public List<Button> activeCampfiresButtons = new List<Button>();

    protected override void OnEnable()
    {
        base.OnEnable();

        rootVisualElement = root.Q<VisualElement>("Campfires");

        activeCampfiresButtons.Clear();

        if (WorldSaveGameManager.instance == null)
            return;

        // Tworzenie przycisków dla aktywnych ognisk
        if (rootVisualElement != null)
        {
            if (WorldObjectManager.instace == null)
                return;

            foreach (InteractableCampfire campfire in WorldObjectManager.instace.interactableCampfires)
            {
                if (campfire == null)
                    continue;

                if (!campfire.isActivated)
                    continue;

                if (rootVisualElement != null)
                {
                    Button campfireButton = new Button()
                    {
                        text = $"Campfire {campfire.ID}",
                        name = $"CampfireButton_{campfire.ID}",
                        style =
                        {
                            fontSize = 32,
                            width = 500,
                            height = 100
                        }
                    };
                    activeCampfiresButtons.Add(campfireButton);
                    campfireButton.clicked += () => player.TeleportPlayerToCampfire(campfire.ID);
                }
                else
                {
                    Debug.LogError("Campfire UI Error, root visual element not found.");
                }

                rootVisualElement.Clear();

                activeCampfiresButtons.Sort((x, y) => x.text.CompareTo(y.text));
                foreach (Button button in activeCampfiresButtons)
                {
                    rootVisualElement.Add(button);
                }
            }
        }
    }

    protected override void Update()
    {
    }

    public override void ActivateInterface(int ID)
    {
        if (MainUiController == null || userInterfaceController == null)
        {
            Debug.LogError("Campfire UI Error, MainUiController or UserInterfaceController is null.");
            return;
        }
        if (interfaceIndex < 0 || interfaceIndex >= userInterfaceController.GetInterfaces().Count)
        {
            Debug.LogError("Campfire UI Error, invalid interface index.");
            return;
        }
        isCampfireUIActive = true;
        userInterfaceController.ActivateInterface(userInterfaceController.GetInterfaces()[interfaceIndex].interfaceRoot);
    }

    public override void DeactivateInterface()
    {
        if (!isCampfireUIActive)
            return;

        if (MainUiController == null || userInterfaceController == null)
        {
            Debug.LogError("Campfire UI Error, MainUiController or UserInterfaceController is null.");
            return;
        }
        if (interfaceIndex < 0 || interfaceIndex >= userInterfaceController.GetInterfaces().Count)
        {
            Debug.LogError("Campfire UI Error, invalid interface index.");
            return;
        }
        isCampfireUIActive = false;
        userInterfaceController.ActivateInterface(userInterfaceController.GetInterfaces()[0].interfaceRoot);
    }
}
