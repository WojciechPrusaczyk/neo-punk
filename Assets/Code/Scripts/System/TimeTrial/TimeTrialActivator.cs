using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrialActivator : MonoBehaviour
{
    public TimeTrial timeTrial;
    public GameObject player;
    public float activationDistance = 2f;


    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    public void StartTrial()
    {
        timeTrial.StartTrial();
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
