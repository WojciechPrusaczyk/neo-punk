using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrial : MonoBehaviour
{
    public List<int> medalTimes;
    
    public bool trialStarted = false;
    public bool trialFinished = false;
    
    public float trialTime = 0;

    public List<GameObject> indicators = new List<GameObject>();

    public GameObject finishCollider;
    public GameObject player;
    
    public TimeTrialInterface timeTrialInterface;
    public TimeTrialEndScreenInterface timeTrialEndScreenInterface;


    /*
     * Event system
     */
    private GameObject EventsPage;
    private EventFlagsSystem _EventsFlagsSystem;

    private void Awake()
    {
        EventsPage = GameObject.Find("EventsFlags");
        _EventsFlagsSystem = EventsPage.GetComponent<EventFlagsSystem>();

        InitializeIndicators();
        player = GameObject.FindGameObjectWithTag("Player");
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitTrial();
        }
    }

    public void StartTrial()
    {
        trialStarted = true;
        foreach (GameObject indicator in indicators)
        {
            indicator.GetComponent<Indicator>().DeactivateIndicator();
            indicator.SetActive(true);
        }
        timeTrialInterface.gameObject.SetActive(true);
    }

    public void FinishTrial()
    {
        trialFinished = true;

        timeTrialInterface.gameObject.SetActive(false);
        
        timeTrialEndScreenInterface.gameObject.SetActive(true);
        timeTrialEndScreenInterface.timerLabel.text = FormatTime(trialTime);
        
        foreach (GameObject indicator in indicators)
        {
            indicator.GetComponent<Indicator>().DeactivateIndicator();
            indicator.SetActive(false);
        }

        if (!_EventsFlagsSystem.IsEventDone("doneFirstTimeTrial"))
            _EventsFlagsSystem.FinishEvent("doneFirstTimeTrial");
    }
    
    public void ExitTrial()
    {
        if (!trialStarted) return;

        timeTrialInterface.gameObject.SetActive(false);
        Debug.Log("ExitTrial");

        timeTrialEndScreenInterface.gameObject.SetActive(true);
        Debug.Log("ExitTrial2");

        timeTrialEndScreenInterface.timerLabel.text = FormatTime(trialTime);
        
        trialStarted = false;
        trialFinished = false;
        trialTime = 0;
        foreach (GameObject indicator in indicators)
        {
            indicator.GetComponent<Indicator>().DeactivateIndicator();
            indicator.SetActive(false);
            
        }
    }
    public string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        float remainingSeconds = seconds % 60;
        
        return $"{minutes}:{remainingSeconds:00.0}";
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
