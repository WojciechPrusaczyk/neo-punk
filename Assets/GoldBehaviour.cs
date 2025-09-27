using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoldBehaviour : MonoBehaviour
{
    public GameObject goldParent;
    private EntityStatus playerStatus;

    private void Awake()
    {
        GameObject player = WorldGameManager.instance.player.GameObject();
        playerStatus = player.GetComponent<EntityStatus>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            playerStatus.AddGold(1);
            Destroy(goldParent);
        }
    }
}
