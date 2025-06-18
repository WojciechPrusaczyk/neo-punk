using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbominationHitbox : MonoBehaviour
{
    public EntityStatus entityStatus;
    public string AttackName;

    public Vector2 pushForce;

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
            if (attack.AttackName == "StingAttack")
            {
                Player playerS = _player.GetComponent<Player>();
                float facingSign = entityStatus.isFacedRight ? 1f : -1f;
        
                Vector2 dir = new Vector2(pushForce.x * facingSign, pushForce.y);
                StartCoroutine(playerS.ApplyKnockback(dir, 0.2f));
            }
        }
    }

}
