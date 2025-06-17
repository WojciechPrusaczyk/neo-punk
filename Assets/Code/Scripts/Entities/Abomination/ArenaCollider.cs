using System;
using System.Collections;
using System.Collections.Generic;
using Inactive;
using Unity.VisualScripting;
using UnityEngine;

public class ArenaCollider : MonoBehaviour
{
    
    public Action<bool> onArenaEnter, onArenaExit;
    public List<BarrierController> barriers;

    public EnemySpawner bossSpawner;

    public EntityStatus bossStatus;
    public AbominationMovement abominationMovement;

    protected virtual void OnEnable()
    {
        bossSpawner.OnSpawn += OnBossSpawn;

    }

    protected virtual void OnDisable()
    {
        bossSpawner.OnSpawn -= OnBossSpawn;
        bossStatus.OnEntityDeath -= OpenArena;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            onArenaEnter?.Invoke(true);
            CloseArena();
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        
        if(other.CompareTag("Player"))
        {
            onArenaExit?.Invoke(false);
        }
    }

    public virtual void CloseArena()
    {
        foreach (var barrier in barriers)
        {
            barrier.Activate();
        }
    }

    public virtual void OpenArena()
    {
        foreach (var barrier in barriers)
        {
            barrier.Deactivate();
        }
    }

    public void OnBossSpawn(GameObject boss)
    {
        bossStatus = boss.GetComponentInChildren<EntityStatus>();
        abominationMovement = boss.GetComponentInChildren<AbominationMovement>();
        abominationMovement.arenaCollider = this;
        abominationMovement.SubscribeToArena();
        bossStatus.OnEntityDeath += OpenArena;
    }
    
}
