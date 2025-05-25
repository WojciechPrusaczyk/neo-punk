using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    public bool isOpen;
    public Collider2D collider2D;
    public Animator animator;
    public SpriteRenderer spriteRenderer;


    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        spriteRenderer.enabled = true;
        animator.enabled = true;
        collider2D.enabled = true;
        isOpen = false;
    }

    public void Deactivate()
    {
        spriteRenderer.enabled = false;
        animator.enabled = false;
        collider2D.enabled = false;
        isOpen = true;
    }
}
