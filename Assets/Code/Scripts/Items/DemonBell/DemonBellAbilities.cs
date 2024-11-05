using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*demonBell = new DemonBell(
    "Demon bell",
    "Player takes 35% more damage, but also deals 25% more.",
    "Overrides damage type to Bloody for 10 seconds.",
    "Rare",
    "Items/DemonBell/DemonBell",
    40.0f,
    0.0f );*/
public class DemonBellAbilities : ScriptableObject, ItemData.IItemAbility
{
    public EntityStatus playerStatus;
    public Player player;

    private float effectDuration; // Czas trwania efektu w sekundach

    private bool isEffectActive = false; // Czy efekt jest aktywny
    private float effectEndTime = 0.0f; // Czas zakończenia efektu
    private int lastElementalType; // ostatnio używany element

    private float additionalDamage;
    private float addedDamage = 0.0f;

    private float defenceLoweringPercent;
    private float loweredDefence = 0.0f;
    private bool isDamageBonusGranted;

    public void Initialize(float _additionalDamage, float _defenceLoweringPercent, float _effectDuration)
    {
        additionalDamage = _additionalDamage;
        defenceLoweringPercent = _defenceLoweringPercent;
        effectDuration = _effectDuration;
    }

    // Implementacja zdolności aktywnej
    public void Use()
    {
        if (!isEffectActive)
        {
            // Rozpoczynamy efekt zwiększenia obrażeń
            isEffectActive = true;
            effectEndTime = Time.time + effectDuration;
            lastElementalType = player.UsedElementalTypeId;
            player.ChangeElementalType(5);
        }
    }

    // Implementacja zdolności pasywnej
    public void Apply()
    {
        if (null == player)
        {
            player = GameObject.FindWithTag("Player").gameObject.GetComponent<Player>();
        }

        if (null == playerStatus)
        {
            playerStatus = GameObject.FindWithTag("Player").gameObject.GetComponent<EntityStatus>();
        }

        /*
         * Zainicjowanie pasywnych bonusów
         */
        if (!isDamageBonusGranted)
        {
            addedDamage = playerStatus.GetBaseAttackDamage() * additionalDamage;
            loweredDefence = playerStatus.incomingDamagePercent * defenceLoweringPercent;

            playerStatus.AttackDamage = playerStatus.GetBaseAttackDamage() + addedDamage;
            playerStatus.incomingDamagePercent += loweredDefence;

            isDamageBonusGranted = true;
        }

        // część UseItem
        if (isEffectActive && !(Time.time < effectEndTime))
        {
            // po zakończeniu UseItem
            player.ChangeElementalType(lastElementalType);

            isEffectActive = false; // Wyłączamy flagę aktywności efektu
        }
    }

    // Implementacja usuwania przedmiotu
    public void Remove()
    {
        playerStatus.AttackDamage = playerStatus.AttackDamage - addedDamage;
        playerStatus.incomingDamagePercent -= loweredDefence;

        isDamageBonusGranted = false;
        player.ChangeElementalType(lastElementalType);
    }
}