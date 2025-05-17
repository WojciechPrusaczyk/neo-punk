using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArenaCollider : MonoBehaviour
{
    
    public Action<bool> onArenaEnter, onArenaExit;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            onArenaEnter?.Invoke(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        
        if(other.CompareTag("Player"))
        {
            onArenaExit?.Invoke(false);
        }
    }
    
}
