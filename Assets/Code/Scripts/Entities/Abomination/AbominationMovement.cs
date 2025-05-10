using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbominationMovement : MonoBehaviour
{
    public ArenaCollider arenaCollider;
    public bool active;
    
    public float movementSpeed;
    
    public GameObject player;


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
    }

    private void OnDisable()
    {
        arenaCollider.onArenaEnter -= ActivateBoss;
    }

    private void Update()
    {
        if (active)
        {
            FollowPlayer();
        }
    }

    public void ActivateBoss(bool b)
    {
        active = b;
    }
    
    void FollowPlayer()
    {
        Vector2 newPosition = player.transform.position;
        newPosition.x = Mathf.Lerp(transform.position.x, player.transform.position.x, movementSpeed * Time.deltaTime);
        transform.position = newPosition;
    }

}
