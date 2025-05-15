using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventShivernDog : MonoBehaviour
{
    public ShivernDog shivernDog;
    public Animator animator;
    public EnemyAI enemyAI;
    public ShivernAudioController shivernAudioController;

    public void Attack()
    {
        shivernDog.DealDamage();
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
        shivernAudioController.PlayAttackSound();
    }

    public void PlayDeathSound()
    {
        shivernAudioController.PlayDeathSound();
    }
}
