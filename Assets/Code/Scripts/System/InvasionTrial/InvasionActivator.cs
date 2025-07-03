using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvasionActivator : MonoBehaviour
{
    public InvasionTrial invasion;
    public GameObject player;
    public float activationDistance = 2f;
    public Sprite activatedSprite;
    public GameObject IconParent;


    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void StartTrial()
    {
        invasion.StartTrial();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.Log("Sprite missing");
            return;
        }
        spriteRenderer.sprite = activatedSprite;
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputManager.InteractKey))
        {
            if (Vector2.Distance(player.transform.position, transform.position) < activationDistance)
            {
                StartTrial();
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(!invasion.trialStarted)
            {
                SpriteRenderer spriteRenderer = IconParent.GetComponent<SpriteRenderer>();
                spriteRenderer.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SpriteRenderer spriteRenderer = IconParent.GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
        }
    }
}
