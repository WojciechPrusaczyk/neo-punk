using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_InterfaceController : MonoBehaviour
{
    [SerializeField] protected VisualElement root;

    protected GameObject MainUi;
    protected MainUserInterfaceController MainUiController;
    protected UserInterfaceController userInterfaceController;
    protected VisualElement rootVisualElement;
    protected int interfaceIndex = 0;

    protected Player player;

    protected virtual void Awake()
    {
        MainUi = GameObject.Find("MainUserInterfaceRoot");
        MainUiController = MainUi.GetComponentInChildren<MainUserInterfaceController>();
        userInterfaceController = MainUi.GetComponent<UserInterfaceController>();
    }

    protected virtual void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("Interface UI Error, no ui document element found.");
            return;
        }
        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("InterfaceUI Error, no root visual element found.");
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        interfaceIndex = userInterfaceController.GetInterfaces().FindIndex(x => x.interfaceRoot == gameObject);
    }

    protected virtual void Update()
    {
    }

    public virtual void ActivateInterface(int ID)
    {
        if (MainUiController == null || userInterfaceController == null)
        {
            Debug.LogError("Interface UI Error, MainUiController or UserInterfaceController is null.");
            return;
        }
        if (interfaceIndex < 0 || interfaceIndex >= userInterfaceController.GetInterfaces().Count)
        {
            Debug.LogError("Interface UI Error, invalid interface index.");
            return;
        }

        userInterfaceController.ActivateInterface(userInterfaceController.GetInterfaces()[interfaceIndex].interfaceRoot);
    }

    public virtual void DeactivateInterface()
    {
        if (MainUiController == null || userInterfaceController == null)
        {
            Debug.LogError("Interface UI Error, MainUiController or UserInterfaceController is null.");
            return;
        }
        if (interfaceIndex < 0 || interfaceIndex >= userInterfaceController.GetInterfaces().Count)
        {
            Debug.LogError("Interface UI Error, invalid interface index.");
            return;
        }

        userInterfaceController.ActivateInterface(userInterfaceController.GetInterfaces()[0].interfaceRoot);
    }
}
