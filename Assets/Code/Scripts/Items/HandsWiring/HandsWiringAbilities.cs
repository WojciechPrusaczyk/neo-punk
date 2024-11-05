using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*handsWiring = new HandsWiring(
    "Hands wiring",
    "Changes basic type of damage to electric.",
    "Produces electric discharge, which deals damage equal 20% of player AD and disables cybernetic enemies for 2 seconds.",
    "Common",
    "Items/HandsWiring/HandsWiring",
    11.0f,
    0.0f, 
    0,
    explosionEffectPrefab,
    explosionRange,
    explosionForce );*/
[System.Serializable]
public class HandsWiringAbilities : ScriptableObject, ItemData.IItemAbility
{
    private float explosionForce;
    private float explosionRange;
    private float damageDealt;
    private GameObject explosionEffectPrefab;
    private Player player;
    private bool isEffectActive;
    private float effectEndTime;
    private int lastElementalType;
    private bool isPassiveGranted;

    public void Initialize(GameObject _explosionEffectPrefab, float _explosionForce, float _explosionRange, float _damageDealt)
    {
        this.explosionEffectPrefab = _explosionEffectPrefab;
        this.explosionForce = _explosionForce;
        this.explosionRange = _explosionRange;
        this.damageDealt = _damageDealt;
        player = null;
    }

    public void Use()
    {
        // Kod aktywnej zdolności
        if (explosionEffectPrefab != null)
        {
            GameObject explosion = Object.Instantiate(explosionEffectPrefab, player.transform.position, Quaternion.identity);
            explosion.transform.parent = player.transform;

            ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();

                Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, explosionRange);
                foreach (Collider2D nearbyObject in colliders)
                {
                    Rigidbody2D rigidbody2D = nearbyObject.GetComponent<Rigidbody2D>();
                    if (rigidbody2D && nearbyObject.gameObject.CompareTag("Enemy"))
                    {
                        Vector3 direction = (nearbyObject.transform.position - player.transform.position).normalized;
                        direction.y = explosionForce * 0.02f;
                        rigidbody2D.AddForce(direction * explosionForce, ForceMode2D.Impulse);

                        EntityStatus entityStatus = nearbyObject.gameObject.GetComponent<EntityStatus>();
                        entityStatus.DealDamage(player.GetComponent<EntityStatus>().GetAttackDamageCount() * damageDealt);
                    }
                }
            }
        }
        else
        {
            Debug.LogError("Explosion prefab is null");
        }
    }

    public void Apply()
    {
        if (null == player)
        {
            this.player = GameObject.FindWithTag("Player").gameObject.GetComponent<Player>();
        }
        
        // Kod pasywnej zdolności
        if (!isPassiveGranted)
        {
            lastElementalType = player.UsedElementalTypeId;
            player.ChangeElementalType(1);
            isPassiveGranted = true;
        }

        // Sprawdzanie zakończenia efektu
        if (isEffectActive && !(Time.time < effectEndTime))
        {
            isEffectActive = false;
            player.ChangeElementalType(lastElementalType);
        }
    }

    public void Remove()
    {
        // Kod usuwania przedmiotu
        if (isEffectActive)
        {
            isEffectActive = false;
            player.ChangeElementalType(lastElementalType);
        }
        isPassiveGranted = false;
    }
}
