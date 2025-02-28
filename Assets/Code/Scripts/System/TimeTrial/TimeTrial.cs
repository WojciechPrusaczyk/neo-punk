using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrial : MonoBehaviour
{
    public bool trialStarted = false;
    public bool trialFinished = false;
    
    public float trialTime = 0;

    public List<GameObject> indicators = new List<GameObject>();

    public GameObject finishCollider;
    public GameObject player;


    private void Start()
    {
        InitializeIndicators();
        player = GameObject.FindGameObjectWithTag("Player");

    }

    public void StartTrial()
    {
        trialStarted = true;
        foreach (GameObject indicator in indicators)
        {
            indicator.SetActive(true);
        }
    }

    public void FinishTrial()
    {
        trialFinished = true;
        foreach (GameObject indicator in indicators)
        {
            indicator.SetActive(false);
        }
    }

    
    private void Update()
    {
        if (trialStarted && !trialFinished)
        {
            trialTime += Time.deltaTime;
            
            if(Vector2.Distance(player.transform.position, finishCollider.transform.position) < 2)
            {
                FinishTrial();
            }
        }
    }

    
    public void InitializeIndicators()
    {
        foreach (Indicator child in transform.GetComponentsInChildren<Indicator>())
        {
            indicators.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }
}
