using System.Collections;
using UnityEngine;

[System.Serializable]
public class FlaskAbilities : ItemData.IItemAbility
{
    private Player player;
    private EntityStatus playerStatus;

    public float healPercent;
    public float maxHealthBoostPercent;
    private Coroutine healingCoroutine;

    public void Initialize(float maxHealthBoostPercent, float healPercent)
    {
        this.maxHealthBoostPercent = maxHealthBoostPercent;
        this.healPercent = healPercent;

    }

    public void Use()
    {
        if (playerStatus == null) return;

        if (healingCoroutine != null)
            CoroutineRunner.Instance.StopCoroutine(healingCoroutine);

        healingCoroutine = CoroutineRunner.Instance.StartCoroutine(HealOverTime(2f));
    }

    private IEnumerator HealOverTime(float duration)
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerStatus = player.GetComponent<EntityStatus>();
        
        float maxHp = playerStatus.GetMaxHp();
        float totalHeal = maxHp * healPercent;
        float healed = 0f;
        float rate = totalHeal / duration;

        float startTime = Time.time;

        while (healed < totalHeal && Time.time - startTime < duration)
        {
            float deltaHeal = rate * Time.deltaTime;
            float currentHp = playerStatus.GetHp();
            float newHp = Mathf.Min(currentHp + deltaHeal, maxHp);

            playerStatus.SetHp(newHp);
            healed += deltaHeal;

            yield return null;
        }
    }

    public void Apply()
    {
        if (playerStatus == null)
            playerStatus = GameObject.FindWithTag("Player").GetComponent<EntityStatus>();

        float extraHp = playerStatus.GetBaseMaxHealth() * maxHealthBoostPercent;
        playerStatus.SetMaxHp(playerStatus.GetBaseMaxHealth() + extraHp);
    }

    public void Remove()
    {
        if (playerStatus == null)
            playerStatus = GameObject.FindWithTag("Player").GetComponent<EntityStatus>();

        float extraHp = playerStatus.GetBaseMaxHealth() * maxHealthBoostPercent;
        playerStatus.SetMaxHp(-extraHp);
    }
}
