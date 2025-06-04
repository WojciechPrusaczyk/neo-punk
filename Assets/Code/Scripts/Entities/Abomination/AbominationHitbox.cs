using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbominationHitbox : MonoBehaviour
{
    public EntityStatus entityStatus;
    public string AttackName;

    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Contains("Player"))
        {
            var attack = entityStatus.AttackTypes
                .Find(a => a.AttackName == AttackName);

            int damage = attack?.AttackDamage ?? -1;
            if (damage == -1)
            {
                Debug.Log(AttackName + " Not found");
                return;
            }
            _player.GetComponent<EntityStatus>().DealDamage(damage, entityStatus.gameObject);
        }
    }
}
