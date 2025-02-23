using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public Sprite deactivatedIndicatorSprite;
    public Sprite activatedIndicatorSprite;
    
    public bool isActivated = false;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateIndicator();
        }
    }

    public void ActivateIndicator()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = activatedIndicatorSprite;
        isActivated = true;
    }
}
