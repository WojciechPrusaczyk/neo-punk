using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbominationHitbox : MonoBehaviour
{
    public EntityStatus entityStatus;

    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Player"))
        {
            _player.GetComponent<EntityStatus>().DealDamage(entityStatus.AttackDamage, entityStatus.gameObject);
        }
    }
}
