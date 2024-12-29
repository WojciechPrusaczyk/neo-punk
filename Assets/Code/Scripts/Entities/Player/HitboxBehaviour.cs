using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitboxBehaviour : MonoBehaviour
{
    public EntityStatus playerEntityStatus;

    private void Awake()
    {
        playerEntityStatus = GetComponentInParent<EntityStatus>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerEntityStatus && collision.CompareTag("Enemy"))
        {
            playerEntityStatus.detectedTargets.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerEntityStatus && collision.CompareTag("Enemy"))
        {
            playerEntityStatus.detectedTargets.Remove(collision.gameObject);
        }
    }
}