using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LoreObject : MonoBehaviour
{
    public int dialogId;
    private DialogScript dialogInterface;
    public GameObject tooltipImage;

    private void Awake()
    {
        dialogInterface = FindFirstObjectByType<DialogScript>();
        tooltipImage.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown(InputManager.InteractKey) || Input.GetKeyDown(InputManager.PadButtonInteract))
                dialogInterface.StartDialog(dialogId);

            if (tooltipImage)
                tooltipImage.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" && tooltipImage)
        {
            tooltipImage.SetActive(false);
        }
    }
}
