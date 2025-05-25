using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialActivator : MonoBehaviour
{
    public TimeTrial timeTrial;
    public GameObject player;
    public GameObject IconParent;
    public float activationDistance = 2f;
    public Sprite activatedSprite;
    public Sprite InteractIcon;
    public bool IsPlayerNear;
    public Animator animator;
    public TimeTrialDeactivator timeTrialDeactivator;
    

    
    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    public void StartTrial()
    {
        timeTrial.StartTrial();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log("Sprite missing");
            return;
        }
        spriteRenderer.sprite = activatedSprite;
        animator.SetTrigger("Flip");
        timeTrialDeactivator.animator.SetTrigger("Flip");
    }
    
    
    private void Update()
    {
        if (Input.GetKeyDown(InputManager.InteractKey))
        {
            if (IsPlayerNear)
            {
                StartTrial();
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (timeTrial.bestRewardReached == false)
        {
            if (collision.CompareTag("Player"))
            {
                SpriteRenderer spriteRenderer = IconParent.GetComponent<SpriteRenderer>();
                spriteRenderer.enabled = true;
                IsPlayerNear = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (timeTrial.bestRewardReached == false)
        {
            if (collision.CompareTag("Player"))
            {
                SpriteRenderer spriteRenderer = IconParent.GetComponent<SpriteRenderer>();
                spriteRenderer.enabled = false;
                IsPlayerNear = false;
            }
        }
    }
}
