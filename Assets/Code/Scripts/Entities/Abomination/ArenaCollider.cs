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

    private void OnEnable()
    {
        abominationStatus.OnEntityDeath += OpenArena;
    }

    private void OnDisable()
    {
        abominationStatus.OnEntityDeath -= OpenArena;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            onArenaEnter?.Invoke(true);
            CloseArena();
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        
        if(other.CompareTag("Player"))
        {
            onArenaExit?.Invoke(false);
        }
    }

    public void CloseArena()
    {
        foreach (var barrier in barriers)
        {
            barrier.Activate();
        }
    }

    public void OpenArena()
    {
        foreach (var barrier in barriers)
        {
            barrier.Deactivate();
        }
    }
    
}
