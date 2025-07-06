using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    public TimeTrialActivator timeTrialActivator;
    public TimeTrialDeactivator timeTrialDeactivator;
    private EntityStatus entityStatus;
    public bool bestRewardReached = false;
    private float force = 6f;

    public Light2D lightAfterOpening;
    private GameObject EventsPage;
    private EventFlagsSystem _EventsFlagsSystem;
    
    public List<Reward> rewards = new List<Reward>();

    public MissionObjectiveUpdater objectiveUpdater;

    private void Awake()
    {
        EventsPage = GameObject.Find("EventsFlags");
        _EventsFlagsSystem = EventsPage.GetComponent<EventFlagsSystem>();

        InitializeIndicators();
        player = GameObject.FindGameObjectWithTag("Player");
        entityStatus = player.GetComponent<EntityStatus>();

        objectiveUpdater = GetComponent<MissionObjectiveUpdater>();
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

        if (trialStarted == true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartTrial();
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                ExitTrial();
            }
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
        timeTrialEndScreenInterface.mask.style.backgroundImage = timeTrialEndScreenInterface.DecideMaskSprite().texture;

        timeTrialEndScreenInterface.timerLabel.text = FormatTime(trialTime);
        
        foreach (GameObject indicator in indicators)
        {
            indicator.GetComponent<Indicator>().DeactivateIndicator();
            indicator.SetActive(false);
        }

        if (!_EventsFlagsSystem.IsEventDone("doneFirstTimeTrial"))
            _EventsFlagsSystem.FinishEvent("doneFirstTimeTrial");
        trialStarted = false;
        //Animacje 
        timeTrialActivator.animator.SetTrigger("Breaking");
        timeTrialDeactivator.animator.SetTrigger("Opening");
        //Rewardy za przejscie
        GiveRewards();

        // Finish mission if active
        if (PlayerObjectiveTracker.instance == null)
            return;

        if (PlayerObjectiveTracker.instance.currentMission == null)
            return;

        if (objectiveUpdater == null)
            return;

        objectiveUpdater.SendFinishObjectiveUpdate();
    }
    
    public void ExitTrial()
    {
        if (!trialStarted) return;

        timeTrialInterface.gameObject.SetActive(false);

        timeTrialEndScreenInterface.gameObject.SetActive(true);
        timeTrialEndScreenInterface.mask.style.backgroundImage = timeTrialEndScreenInterface.MaskSprite[3].texture;
        timeTrialEndScreenInterface.timerLabel.text = FormatTime(trialTime);
        
        trialStarted = false;
        trialFinished = false;
        trialTime = 0;
        foreach (GameObject indicator in indicators)
        {
            indicator.GetComponent<Indicator>().DeactivateIndicator();
            indicator.SetActive(false);
            
        }
        timeTrialActivator.animator.SetTrigger("Breaking");
        timeTrialDeactivator.animator.SetTrigger("Breaking");

    }
    public string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60);
        float remainingSeconds = seconds % 60;
        
        return $"{minutes}:{remainingSeconds:00.0}";
    }

    public void RestartTrial()
    {
        Vector2 pos = timeTrialActivator.transform.position;
        player.transform.position = pos;
        trialTime = 0;
        StartTrial();
    }
    
    public void InitializeIndicators()
    {
        foreach (Indicator child in transform.GetComponentsInChildren<Indicator>())
        {
            indicators.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }
    }

    public void GiveRewards()
    {
        if(medalTimes[0]>trialTime)
        {
            bestRewardReached = true;
            
            StartCoroutine(PlayAnimation(rewards[0]));
        }
        else if(medalTimes[1]>trialTime)
        {
            StartCoroutine(PlayAnimation(rewards[1]));
        }
        else if(medalTimes[2]>trialTime)
        {
            StartCoroutine(PlayAnimation(rewards[2]));
        }
    }
    IEnumerator PlayAnimation(Reward reward)
    {
        yield return new WaitForSeconds(1.5f);
        var verPostion = timeTrialDeactivator.transform.position;
        var newpos = new Vector3(verPostion.x, verPostion.y+2, verPostion.z);
        if (reward.items.Count != 0)
        {
            foreach (var item in reward.items)
            {
                GameObject itemGiven = Instantiate(item, newpos, Quaternion.identity);
                var rigidBody = itemGiven.GetComponent<Rigidbody2D>();
                rigidBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            }
        }
        entityStatus.AddGold(reward.goldAmount);
        lightAfterOpening.enabled = true;
    }
}
