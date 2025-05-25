using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_FireplaceInterface_Controller : MonoBehaviour
{
    [SerializeField] private VisualElement root;

    private GameObject MainUi;
    private MainUserInterfaceController MainUiController;
    private UserInterfaceController userInterfaceController;
    private VisualElement rootVisualElement;

    //private int 


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
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Campfire UI Error, no root visual element found.");
        }
    }
}
