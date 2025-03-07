using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public Sprite deactivatedIndicatorSprite;
    public Sprite activatedIndicatorSprite;
    
    public bool isActivated = false;
    
    public ParticleSystem sparkEffect;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isActivated) return;
            ActivateIndicator();
            EmitSparks();

        }
    }

    public void ActivateIndicator()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = activatedIndicatorSprite;
        isActivated = true;
    }

    public void DeactivateIndicator()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = deactivatedIndicatorSprite;
        isActivated = false;
    }


    public void EmitSparks(int amount = 15)
    {
        if (sparkEffect != null)
        {
            sparkEffect.Emit(amount);
        }
        else
        {
            Debug.LogWarning("Spark particle system not assigned!");
        }
    }
}
