using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationEventDragonFly : MonoBehaviour
{
    public Dragonfly dragonfly;
    public Animator animator;
    public EnemyAI enemyAI;
    
    
    public void ShootBullet()
    {
        dragonfly.PerformShoot();
    }
    
    public void StartAttack()
    {
        enemyAI.FreezeMovement();
        animator.SetBool("isAttacking", true);
        dragonfly.isAttacking = true;
    }
    
    
    public void EndAttack()
    {
        dragonfly.isAttacking = false;
        animator.SetBool("isAttacking", false);
        enemyAI.RestoreMovement();
    }
    
    
    public void Move()
    {
        dragonfly.isAttacking = false;
        animator.SetBool("isAttacking", false);
        enemyAI.RestoreMovement();
    }
}
