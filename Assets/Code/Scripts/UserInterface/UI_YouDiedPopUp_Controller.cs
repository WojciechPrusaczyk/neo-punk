using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_YouDiedPopUp_Controller : UI_InterfaceController
{
    private Label youDiedText;
    protected override void OnEnable()
    {
        base.OnEnable();

        rootVisualElement = root.Q<VisualElement>("Campfires");
        youDiedText = root.Q<Label>("YouDiedText");
    }
}
