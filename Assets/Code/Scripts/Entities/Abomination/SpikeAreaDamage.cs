using System.Collections;
using UnityEngine;

public class SpikeAreaDamage : MonoBehaviour
{
    [SerializeField] private EntityStatus entityStatus;
    public string AttackName;

    [SerializeField] private float tickInterval = 0.5f;

    private Coroutine damageRoutine;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // 1st tick immediately
        ApplyDamage(other.gameObject);

        // start the repeating ticks
        damageRoutine = StartCoroutine(DamageLoop(other.gameObject));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (damageRoutine != null) StopCoroutine(damageRoutine);
        damageRoutine = null;
    }
    private IEnumerator DamageLoop(GameObject player)
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
            ApplyDamage(player);
        }
    }

    private void ApplyDamage(GameObject player)
    {
        if (entityStatus.isDead) return;
        
        var attack = entityStatus.AttackTypes
            .Find(a => a.AttackName == AttackName);

        int damage = attack?.AttackDamage ?? -1;
        if (damage == -1)
        {
            Debug.Log(AttackName + " Not found");
            return;
        }
        player.GetComponent<EntityStatus>().DealDamage(damage, entityStatus.gameObject);
    }
}