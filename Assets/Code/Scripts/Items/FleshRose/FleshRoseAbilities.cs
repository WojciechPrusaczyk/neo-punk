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
    private bool isSubscribed;
    
    public void Initialize(float thornsPercent, float bloodBoltDamagePercent, float selfDamagePercent)
    {
        this.thornsPercent = thornsPercent;
        this.bloodBoltDamagePercent = bloodBoltDamagePercent;
        this.selfDamagePercent = selfDamagePercent;

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

        if (!isSubscribed)
        {
            playerStatus.OnPlayerDamageTaken += OnPlayerDamagedBy;
            isSubscribed = true;
        }
    }

    private void OnPlayerDamagedBy(GameObject entity, float damageTaken)
    {
        if (entity == null) return;

        EntityStatus entityStatus = entity.GetComponentInChildren<EntityStatus>();
        if (entityStatus == null) return;
        
        entityStatus.DealDamage(damageTaken * thornsPercent);
        Debug.Log("damageDelt");
    }

    public void Remove()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player").GetComponent<Player>();

        if (playerStatus == null)
            playerStatus = player.GetComponent<EntityStatus>();

        if (isSubscribed)
        {
            playerStatus.OnPlayerDamageTaken -= OnPlayerDamagedBy;
            isSubscribed = false;
        }
    }

}
