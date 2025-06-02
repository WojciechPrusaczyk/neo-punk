using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyAI enemyAI;
    public EntityStatus entityStatus;

    private void Awake()
    {
        if (enemyAI == null)
        {
            enemyAI = GetComponentInChildren<EnemyAI>();
        }
        if (entityStatus == null)
        {
            entityStatus = GetComponentInChildren<EntityStatus>();
        }

        if (enemyAI == null || entityStatus == null)
        {
            Debug.LogError("EnemyManager requires EnemyAI and EntityStatus components.");
            return;
        }
    }
}
