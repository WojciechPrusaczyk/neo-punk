using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoodooDollAbilities : ItemData.IItemAbility
{
    private float damageIncreasePercentage;
    private float effectDuration;
    private float needleStacks;
    private EntityStatus playerStatus;
    private PlayerInventory playerInventory;
    private float lastNoticedPlayerHp;

    public void Initialize(float damageIncreasePercentage, float effectDuration)
    {
        this.damageIncreasePercentage = damageIncreasePercentage;
        this.effectDuration = effectDuration;
        this.needleStacks = 0;
        this.lastNoticedPlayerHp = 0;
    }

    public void Use()
    {
        if (playerStatus == null)
        {
            playerStatus = GameObject.FindWithTag("Player").GetComponent<EntityStatus>();
        }

        float baseDamage = playerStatus.GetBaseAttackDamage();
        playerStatus.SetAttackDamageCount(baseDamage * (1.0f + damageIncreasePercentage));

        CoroutineRunner.Instance.StartCoroutine(ResetDamageAfterDuration(playerStatus, baseDamage));
    }

    private IEnumerator ResetDamageAfterDuration(EntityStatus playerStatus, float baseDamage)
    {
        yield return new WaitForSeconds(effectDuration);
        playerStatus.SetAttackDamageCount(baseDamage);
    }

    public void Apply()
    {
        if (playerStatus == null)
        {
            playerStatus = GameObject.FindWithTag("Player").gameObject.GetComponent<EntityStatus>();
        }

        if (playerInventory == null)
        {
            playerInventory = GameObject.FindWithTag("Player").gameObject.GetComponent<PlayerInventory>();
        }

        if (lastNoticedPlayerHp == 0 && playerStatus != null)
        {
            lastNoticedPlayerHp = playerStatus.GetHp();
        }

        if (playerStatus.GetHp() < lastNoticedPlayerHp)
        {
            needleStacks += 1;
            lastNoticedPlayerHp = playerStatus.GetHp();

            playerInventory.SetImageAtSlotByIndex("Items/VoodooDoll/VoodooDoll_" + needleStacks.ToString(), "Voodoo Doll");

            if (needleStacks > 3)
            {
                playerStatus.PlayerDeathEvent();
            }
        }
    }

    public void Remove()
    {
        if (playerStatus != null)
        {
            playerStatus.SetAttackDamageCount(playerStatus.GetBaseAttackDamage());
        }
    }
}