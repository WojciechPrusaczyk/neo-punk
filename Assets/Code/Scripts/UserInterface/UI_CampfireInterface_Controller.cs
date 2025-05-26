using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_CampfireInterface_Controller : UI_InterfaceController
{
    public bool isCampfireUIActive = false;

    public List<Button> activeCampfiresButtons = new List<Button>();

    private float width = 500f;
    private float height = 250f;

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
            if (WorldObjectManager.instance == null)
                return;

            CreateCampfireInterfaceUI();
        }
    }

    protected override void Update()
    {
    }

    private void CreateCampfireInterfaceUI()
    {
        int activeCampfireAmount = 0;

        foreach (InteractableCampfire campfire in WorldObjectManager.instance.interactableCampfires)
        {
            if (campfire == null || !campfire.isActivated)
                continue;
            activeCampfireAmount++;
        }

        // Zmiana wielkoœci przycisków w zale¿noœci od iloœci aktywnych ognisk
        switch (activeCampfireAmount)
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

        foreach (InteractableCampfire campfire in WorldObjectManager.instance.interactableCampfires)
        {
            if (campfire == null)
                continue;

            if (!campfire.isActivated)
                continue;
            if (rootVisualElement != null)
            {
                Button campfireButton = new Button()
                {
                    text = $"",
                    name = $"CampfireButton_{campfire.ID}",
                    style =
                        {
                            width = width,
                            height = height,
                            backgroundImage = campfire.backgroundImage.texture,
                            justifyContent = Justify.FlexEnd,
                            alignItems = Align.FlexEnd,
                            flexDirection = FlexDirection.Column,
                        }
                };
                activeCampfiresButtons.Add(campfireButton);

                campfireButton.clicked += () => player.TeleportPlayerToCampfire(campfire.ID);

                Label campfireNameLabel = new Label(campfire.campfireName)
                {
                    style =
                        {
                            fontSize = 24,
                            color = Color.white,
                            unityTextAlign = TextAnchor.MiddleCenter,
                            backgroundColor = new Color(0, 0, 0, 0.5f),
                            borderBottomLeftRadius = 5,
                            borderBottomRightRadius = 5,
                            borderTopLeftRadius = 5,
                            borderTopRightRadius = 5,
                            paddingLeft = 20,
                            paddingRight = 20,
                            paddingTop = 8,
                            paddingBottom = 8,
                        }
                };
                campfireButton.Add(campfireNameLabel);

                campfireButton.RegisterCallback<MouseEnterEvent>(evt =>
                {
                    campfireButton.style.unityBackgroundImageTintColor = new StyleColor(new Color(1f, 1f, 1f, .99f));
                });

                campfireButton.RegisterCallback<MouseLeaveEvent>(evt =>
                {
                    campfireButton.style.unityBackgroundImageTintColor = new StyleColor(new Color(1f, 1f, 1f, 1f));
                });
            }
            else
            {
                Debug.LogError("Campfire UI Error, root visual element not found.");
            }

            rootVisualElement.Clear();

            activeCampfiresButtons.Sort((a, b) =>
            {
                int idA = int.Parse(a.name.Split('_')[1]);
                int idB = int.Parse(b.name.Split('_')[1]);
                return idA.CompareTo(idB);
            });

            foreach (Button button in activeCampfiresButtons)
            {
                rootVisualElement.Add(button);
            }
        }
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
