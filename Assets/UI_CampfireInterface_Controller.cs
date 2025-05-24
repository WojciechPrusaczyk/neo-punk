using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_CampfireInterface_Controller : MonoBehaviour
{
    [SerializeField] private VisualElement root;

    private GameObject MainUi;
    private MainUserInterfaceController MainUiController;
    private UserInterfaceController userInterfaceController;
    private VisualElement rootVisualElement;

    private int interfaceIndex = 0;
    public bool isCampfireUIActive = false;

    public List<Button> activeCampfiresButtons = new List<Button>();

    public void Awake()
    {
        MainUi = GameObject.Find("MainUserInterfaceRoot");
        MainUiController = MainUi.GetComponentInChildren<MainUserInterfaceController>();
        userInterfaceController = MainUi.GetComponent<UserInterfaceController>();
    }

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("Campfire UI Error, no ui document element found.");
            return;
        }
        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Campfire UI Error, no root visual element found.");
            return;
        }

        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        interfaceIndex = userInterfaceController.GetInterfaces().FindIndex(x => x.interfaceRoot == gameObject);
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

    void Update()
    {
    }

    public void ActivateCampfireInterface(int ID)
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

    public void DeactivateCampfireInterface()
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
