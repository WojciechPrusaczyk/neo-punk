using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaCollider : MonoBehaviour
{
    
    public Action<bool> onArenaEnter;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            onArenaEnter?.Invoke(true);
        }
    }
    
    
}
