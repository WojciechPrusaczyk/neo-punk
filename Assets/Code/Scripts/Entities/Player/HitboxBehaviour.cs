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
            var targetStatus =  collision.GetComponent<EntityStatus>()          ??
                                collision.GetComponentInParent<EntityStatus>()  ??
                                collision.GetComponentInChildren<EntityStatus>();

            if (targetStatus)                 // null-check is important!
                playerEntityStatus.detectedTargets.Add(targetStatus.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerEntityStatus && collision.CompareTag("Enemy"))
        {
            var targetStatus = collision.GetComponent<EntityStatus>() ??
                                collision.GetComponentInParent<EntityStatus>() ??
                                collision.GetComponentInChildren<EntityStatus>();

            if (targetStatus) // null-check is important!
                playerEntityStatus.detectedTargets.Remove(targetStatus.gameObject);
        }
    }
}