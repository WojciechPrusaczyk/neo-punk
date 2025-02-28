using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventShivernDog : MonoBehaviour
{
    public ShivernDog shivernDog;
    public Animator animator;
    public EnemyAI enemyAI;

    public void Attack()
    {
        shivernDog.DealDamage();
    }

    public void StartAttack()
    {
        enemyAI.FreezeMovement();
        animator.SetBool("isAttacking", true);
        shivernDog.isAttacking = true;
    }
    
    
    public void EndAttack()
    {
        // Debug.Log("animation end");
        shivernDog.isAttacking = false;
        animator.SetBool("isAttacking", false);
        enemyAI.RestoreMovement();
    }
    
}
