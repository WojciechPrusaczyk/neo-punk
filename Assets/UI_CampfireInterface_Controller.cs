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

        interfaceIndex = userInterfaceController.GetInterfaces().FindIndex(x => x.interfaceRoot == gameObject);
        rootVisualElement = root.Q<VisualElement>("Campfires");

        // Add a new button to the root visual element
        // Text in button: Campfire
        // Button id: Campfire1
        // Font size: 32
        if (rootVisualElement != null)
        {
            Button campfireButton = new Button(ActivateCampfireInterface)
            {
                text = "Campfire",
                name = "Campfire1",
                style =
                {
                    fontSize = 32,
                    width = 200,
                    height = 50
                }
            };
            rootVisualElement.Add(campfireButton);
        }
        else
        {
            Debug.LogError("Campfire UI Error, root visual element not found.");
        }
    }

    void Update()
    {
    }

    public void ActivateCampfireInterface()
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
