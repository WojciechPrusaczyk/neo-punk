using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleshRoseAbilities : ItemData.IItemAbility
{
    
    private EntityStatus playerStatus;
    private Player player;
    private float thornsPercent;
    private float bloodBoltDamagePercent;
    private float selfDamagePercent;
    
    private int activeBloodBolts = 0;
    private bool isSubscribedDelt;
    private bool isSubscribedTaken;

    public GameObject bloodBoltPrefab;
    
    public void Initialize(float thornsPercent, float bloodBoltDamagePercent, float selfDamagePercent, GameObject bloodBolt)
    {
        this.thornsPercent = thornsPercent;
        this.bloodBoltDamagePercent = bloodBoltDamagePercent;
        this.selfDamagePercent = selfDamagePercent;
        this.bloodBoltPrefab = bloodBolt;

    }
    
    public void Use()
    {
        activeBloodBolts = 3;
    }

    public void Apply()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<Player>();

        if (playerStatus == null)
            playerStatus = player.GetComponent<EntityStatus>();

        if (!isSubscribedDelt)
        {
            playerStatus.OnPlayerDamageTaken += OnPlayerDamagedBy;
            isSubscribedDelt = true;
        }

        if (!isSubscribedTaken)
        {
            player.OnPlayerAttack += OnPlayerAttack;
            isSubscribedTaken = true;
        }
    }

    private void OnPlayerDamagedBy(GameObject entity, float damageTaken)
    {
        if (entity == null) return;

        EntityStatus entityStatus = entity.GetComponentInChildren<EntityStatus>();
        if (entityStatus == null) return;
        
        entityStatus.DealDamage(damageTaken * thornsPercent);
    }

    private void OnPlayerAttack()
    {
        if (activeBloodBolts > 0)
        {
            Vector2 direction = playerStatus.isFacedRight ? Vector2.right : Vector2.left;
            float damage = playerStatus.AttackDamage + bloodBoltDamagePercent;
        
            GameObject bloodBolt = GameObject.Instantiate(bloodBoltPrefab, player.transform.position, Quaternion.identity);
            BloodBolt bloodBoltScript = bloodBolt.GetComponent<BloodBolt>();
            bloodBoltScript.Initialize(direction, damage);
            float selfDamage = playerStatus.GetMaxHp() * selfDamagePercent;
            playerStatus.DealDamage(selfDamage);
            activeBloodBolts--;
        }
        
    }

    public void Remove()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<Player>();

        if (playerStatus == null)
            playerStatus = player.GetComponent<EntityStatus>();

        if (isSubscribedDelt)
        {
            playerStatus.OnPlayerDamageTaken -= OnPlayerDamagedBy;
            isSubscribedDelt = false;
        }

        if (!isSubscribedTaken)
        {
            player.OnPlayerAttack -= OnPlayerAttack;
            isSubscribedDelt = false;
        }
    }

}
