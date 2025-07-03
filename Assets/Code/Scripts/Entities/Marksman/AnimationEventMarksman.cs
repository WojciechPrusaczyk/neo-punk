using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventMarksman : MonoBehaviour
{
    public Marksman marksman;
    public Animator animator;
    public EnemyAI enemyAI;
    public ShivernAudioController shivernAudioController;


    public void Push()
    {
        
    }

    public void ThrowGrenade()
    {
        
    }

    public void Shoot()
    {
        
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
