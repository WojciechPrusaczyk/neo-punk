using System;
using UnityEngine;

public class AnimationEventMarksman : MonoBehaviour
{
    public Marksman marksman;
    public Animator animator;
    public EnemyAI enemyAI;
    public EntityStatus entityStatus;
    public MarksmanAudioController marksmanudioController;
    public Vector2 pushForce;



    public void Push()
    {
        GameObject player = WorldGameManager.instance.player.gameObject;
        Player playerScript = player.GetComponent<Player>();
        float facingSign = entityStatus.isFacedRight ? 1f : -1f;
        Vector2 dir = new Vector2(70 * facingSign, 5);
        StartCoroutine(playerScript.ApplyKnockback(dir, 0.2f));
        Debug.Log(dir);
    }

    public void ThrowGrenade()
    {
        Vector3 spawnPosition = transform.position + Vector3.up * 1f;
        
        GameObject grenade = Instantiate(marksman.grenadePrefab, spawnPosition, Quaternion.identity);
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = entityStatus.isFacedRight ? Vector2.right : Vector2.left;

        Vector2 throwDirection = (shootDirection + Vector2.up).normalized;
        float throwForce = 8f;
        
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
    }

    public void Shoot()
    {
        GameObject projectile = Instantiate(marksman.bulletPrefab, marksman.bulletSpawn.position, Quaternion.identity);
        BulletBehaviour bulletBehaviour = projectile.GetComponent<BulletBehaviour>();
        if (bulletBehaviour != null)
        {
            bulletBehaviour.SetShooter(enemyAI.gameObject);
        }
        
        Rigidbody2D projectileRigidbody = projectile.GetComponent<Rigidbody2D>();
        Vector2 shootDirection = entityStatus.isFacedRight ? Vector2.right : Vector2.left;
        projectileRigidbody.velocity = shootDirection * 10;

        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, shootDirection) * Quaternion.Euler(0, 0, 90);
        projectile.transform.rotation = rotation;
        Destroy(projectile, 1f);
    }
    
    
    public void StartAttack()
    {
        enemyAI.FreezeMovement();
        animator.SetBool("isAttacking", true);
    }
    
    
    public void EndAttack()
    {
        // Debug.Log("animation end");
        animator.SetBool("isAttacking", false);
        enemyAI.RestoreMovement();
    }

    public void PlayAttackSound()
    {
    }

    public void PlayDeathSound()
    {
    }
}
