using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoodooDollAbilities : ItemData.IItemAbility
{
    private float damageIncreasePercentage;
    private float effectDuration;
    private int needleStacks;
    private EntityStatus playerStatus;
    private Player player;
    private PlayerInventoryInterface playerInventory;
    private float lastNoticedPlayerHp;
    public Sprite itemIconOneStack;
    public Sprite itemIconTwoStacks;
    public Sprite itemIconThreeStacks;

    public void Initialize(float damageIncreasePercentage, float effectDuration, Sprite _itemIconOneStack, Sprite _itemIconTwoStacks, Sprite _itemIconThreeStacks)
    {
        this.damageIncreasePercentage = damageIncreasePercentage;
        this.effectDuration = effectDuration;
        this.needleStacks = 0;
        this.lastNoticedPlayerHp = 0;
        itemIconOneStack = _itemIconOneStack;
        itemIconTwoStacks = _itemIconTwoStacks;
        itemIconThreeStacks = _itemIconThreeStacks;
    }

    public void Use()
    {
        if (playerStatus == null)
        {
            playerStatus = GameObject.FindWithTag("Player").GetComponent<EntityStatus>();
            player = playerStatus.gameObject.GetComponent<Player>();
        }

        if (player == null)
        {
            player = playerStatus.gameObject.GetComponent<Player>();
        }

        float baseDamage = playerStatus.GetBaseAttackDamage();
        playerStatus.SetAttackDamageCount(baseDamage * (1.0f + damageIncreasePercentage));

        player.ChangeElementalType(5);

        CoroutineRunner.Instance.StartCoroutine(ResetDamageAfterDuration(playerStatus, baseDamage));
    }

    private IEnumerator ResetDamageAfterDuration(EntityStatus playerStatus, float baseDamage)
    {
        yield return new WaitForSeconds(effectDuration);
        player.ChangeElementalType(0);
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
            playerInventory = GameObject.Find("MainUserInterfaceRoot").transform.Find("EquipmentInterface").gameObject.GetComponent<PlayerInventoryInterface>();
        }

        if (lastNoticedPlayerHp == 0 && playerStatus != null)
        {
            lastNoticedPlayerHp = playerStatus.GetHp();
        }

        if (playerStatus.GetHp() < lastNoticedPlayerHp)
        {
            needleStacks += 1;
            lastNoticedPlayerHp = playerStatus.GetHp();

            if (needleStacks == 1)
                playerInventory.SetImageAtSlotByIndex(itemIconOneStack, "Voodoo Doll");

            else if (needleStacks == 2)
                playerInventory.SetImageAtSlotByIndex(itemIconTwoStacks, "Voodoo Doll");

            else if (needleStacks == 3)
                playerInventory.SetImageAtSlotByIndex(itemIconThreeStacks, "Voodoo Doll");

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