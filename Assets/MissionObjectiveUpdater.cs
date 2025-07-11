using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObjectiveUpdater : MonoBehaviour
{
    [Tooltip("The objective updater will only update task if the mission here is equal to current mission")]
    [SerializeField] private MissionInfo missionInfo;
    [SerializeField] private int objectiveId;

    [Tooltip("If this is set, finishing objective will also update this eventFlag to true")]
    [SerializeField] private string eventFlagToFinish;
    public MissionInfo MissionInfo => missionInfo;
    public int ObjectiveId => objectiveId;

    private void Awake()
    {
        if (!PlayerObjectiveTracker.instance)
            return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<Player>())
            return;

        if (PlayerObjectiveTracker.instance.currentMission == null)
            return;

        if (PlayerObjectiveTracker.instance.currentMission != missionInfo)
            return;

        var currentObjective = PlayerObjectiveTracker.instance.currentMission.objectives[ObjectiveId];

        PlayerObjectiveTracker.instance.ChangeMissionObjectiveStatus(objectiveId, true);


        if (!string.IsNullOrEmpty(eventFlagToFinish))
            EventFlagsSystem.instance.FinishEvent(eventFlagToFinish);

        if (currentObjective.isCompleted)
            gameObject.SetActive(false);

        float RandomPitch = Random.Range(0.9f, 1.15f);
        WorldSoundFXManager.instance.PlaySoundFX(WorldSoundFXManager.instance.buttonClickSFX, Enums.SoundType.SFX, RandomPitch);
    }

    public void SendFinishObjectiveUpdate(bool disableObject = false)
    {
        if (PlayerObjectiveTracker.instance.currentMission == null)
            return;
        if (PlayerObjectiveTracker.instance.currentMission != missionInfo)
            return;

        var currentObjective = PlayerObjectiveTracker.instance.currentMission.objectives[ObjectiveId];
        PlayerObjectiveTracker.instance.ChangeMissionObjectiveStatus(objectiveId, true);
        if (currentObjective.isCompleted && disableObject)
            gameObject.SetActive(false);
    }
}