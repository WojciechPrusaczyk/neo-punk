using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class HealthBurController : MonoBehaviour
{
    public float CurrentHealthPercent = 0;
    private GameObject playerObject;
    private EntityStatus playerStatus;
    public GameObject healthDisplay;
    private Image healthDisplayImage;
    private float initialWidth;

    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject)
        {
            playerStatus = playerObject.GetComponent<EntityStatus>();
            if (healthDisplay)
            {
                healthDisplayImage = healthDisplay.GetComponent<Image>();
                initialWidth = healthDisplayImage.rectTransform.sizeDelta.x;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Bieżące aktualizowanie HP
        if (playerObject)
        {
            CurrentHealthPercent = playerStatus.GetHp() / playerStatus.GetMaxHp();

            // aktualizacja paska hp
            if (healthDisplay)
            {
                healthDisplayImage.rectTransform.sizeDelta = new Vector2(CurrentHealthPercent * initialWidth,
                    healthDisplayImage.rectTransform.sizeDelta.y);
            }
        }
    }
}