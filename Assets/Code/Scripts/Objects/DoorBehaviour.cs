using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorBehaviour : MonoBehaviour
{
    public bool IsOpen = false;
    private GameObject playerObject;
    private Animator animator;

    private void Awake()
    {
        animator = gameObject.GetComponent<Animator>();
        playerObject = GameObject.Find("Player");
    }


    public void OpenDoor()
    {
        IsOpen = true;
        animator.SetBool("IsOpen", IsOpen );
    }

    public void CloseDoor()
    {
        IsOpen = false;
        animator.SetBool("IsOpen", IsOpen );
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsOpen) OpenDoor();
        
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(InputManager.InteractKey))  
            {
                // TODO: Ładowanie odpowiedniej sceny
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        this.CloseDoor();
    }
}