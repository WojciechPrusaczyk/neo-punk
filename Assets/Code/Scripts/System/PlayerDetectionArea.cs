using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectionArea : MonoBehaviour
{
    public EnemyAI enemyAI; 
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemyAI.HasLineOfSight())
            {
                enemyAI.state = EnemyAI.EnemyState.Chasing;
                enemyAI.enemyStatus.SetIsAlerted(true);

                enemyAI.playerAreaCollider.size = enemyAI.alertedAreaSize;
                enemyAI.playerAreaCollider.offset = enemyAI.alertedAreaOffset;
                
                if(!enemyAI.enemyStatus.detectedTargets.Contains(other.gameObject))
                    enemyAI.enemyStatus.detectedTargets.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            enemyAI.state = EnemyAI.EnemyState.Wandering;
            enemyAI.enemyStatus.SetIsAlerted(false);

            enemyAI.playerAreaCollider.size = enemyAI.idleAreaSize;
            enemyAI.playerAreaCollider.offset = enemyAI.idleAreaOffset;
            
            enemyAI.enemyStatus.detectedTargets.Remove(other.gameObject);

        }
    }
}
