using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainUserInterfaceController : MonoBehaviour
{

    private Label hpLabel;
    private EntityStatus playerStatus;

    private void Awake()
    {
        var playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerStatus = playerGameObject.GetComponent<EntityStatus>();
    }

    private void OnEnable()
    {
        // Ładujemy UXML
        var uiDocument = GetComponent<UIDocument>();
        var InterfaceRoot = uiDocument.rootVisualElement;

        /*
         * Przyciski menu
         */
        hpLabel = InterfaceRoot.Q<Label>("HpText");
    }

    private void Update()
    {
        hpLabel.text = $"HP {playerStatus.entityMaxHelath}/{playerStatus.entityHealthPoints}";
    }
}
