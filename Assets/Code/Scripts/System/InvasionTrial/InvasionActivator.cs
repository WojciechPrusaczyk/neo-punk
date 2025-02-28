using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvasionActivator : MonoBehaviour
{
    public InvasionTrial invasion;
    public GameObject player;
    public float activationDistance = 2f;


    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    public void StartTrial()
    {
        Debug.Log("StartTrial");
        invasion.StartTrial();
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputManager.InteractKey))
        {
            if (Vector2.Distance(player.transform.position, transform.position) < activationDistance)
            {
                StartTrial();
            }
        }
    }
}
