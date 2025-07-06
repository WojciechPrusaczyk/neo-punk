using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBolt : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 2f;
    public float damage = 0;

    private Vector2 moveDirection;
    private GameObject targetEnemy;
    public GameObject graphics;
    private Vector2 movement;

    public void Initialize(Vector2 direction, float damageAmount)
    {
        moveDirection = direction;
        damage = damageAmount;
        
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (!targetEnemy)
        {
            movement = moveDirection.normalized;
            transform.Translate(movement * speed * Time.deltaTime);
        }
        else
        {
            
            movement = (targetEnemy.transform.position - transform.position).normalized;
            transform.Translate(movement * speed * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, targetEnemy.transform.position) < 0.1f)
            {
                EntityStatus enemyStatus = targetEnemy.GetComponentInChildren<EntityStatus>();
                enemyStatus.DealDamage(damage);
                Destroy(gameObject);
            }
        }
        
        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg - 45f;
        graphics.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!targetEnemy && other.CompareTag("Enemy"))
        {
            targetEnemy = other.gameObject;
        }
        
    }
}
