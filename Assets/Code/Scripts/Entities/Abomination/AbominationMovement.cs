using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class AbominationMovement : MonoBehaviour
{
    public ArenaCollider arenaCollider;
    public bool active;
    
    public float movementSpeed;
    
    public GameObject player;

    public Rigidbody2D rb;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        /*
         * For debugging only
         */
        var abominationStatus = gameObject.transform.parent.GetComponent<EntityStatus>();
        var mainUi = GameObject.Find("MainUserInterface").GetComponent<MainUserInterfaceController>();
        mainUi.ShowBossBar(abominationStatus);
    }

    private void OnEnable()
    {
        arenaCollider.onArenaEnter += ActivateBoss;
        arenaCollider.onArenaExit += DeactivateBoss;
    }

    private void OnDisable()
    {
        arenaCollider.onArenaEnter -= ActivateBoss;
        arenaCollider.onArenaExit -= DeactivateBoss;
    }

    
    private void FixedUpdate()
    {
        if (active)
        {
            MoveHorizontallyToPlayer();
        }
    }

    public void ActivateBoss(bool b)
    {
        active = b;
    }
    
    public void DeactivateBoss(bool b)
    {
        active = b;
    }
    
    void FollowPlayer()
    {
        Vector2 newPosition = player.transform.position;
        newPosition.x = Mathf.Lerp(transform.position.x, player.transform.position.x, movementSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

    void MoveHorizontallyToPlayer()                             // <- its own function
    {
        Vector2 pos = rb.position;

        // constant-speed step toward the player’s X
        pos.x = Mathf.MoveTowards(pos.x, 
            player.transform.position.x, 
            movementSpeed * Time.fixedDeltaTime);

        rb.MovePosition(pos);           // keeps physics happy
    }

}
