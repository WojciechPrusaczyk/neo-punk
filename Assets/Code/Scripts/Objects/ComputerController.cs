using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerController : MonoBehaviour
{
    private ComputerInterfaceController computerInterfaceController;
    private void Awake()
    {
        computerInterfaceController = GameObject.Find("UserInterface").
            transform.Find("ObjectsInterfaces").
            transform.Find("Computer").
            gameObject.GetComponent<ComputerInterfaceController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(InputManager.InteractKey))  
            {
                computerInterfaceController.ShowComputerInterface();
            }
        }
    }
}