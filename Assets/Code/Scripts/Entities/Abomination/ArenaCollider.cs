using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArenaCollider : MonoBehaviour
{
    
    public Action<bool> onArenaEnter, onArenaExit;
    public List<BarrierController> barriers;

    public EntityStatus abominationStatus;

    protected virtual void OnEnable()
    {
        abominationStatus.OnEntityDeath += OpenArena;
    }

    protected virtual void OnDisable()
    {
        abominationStatus.OnEntityDeath -= OpenArena;
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
    
}
