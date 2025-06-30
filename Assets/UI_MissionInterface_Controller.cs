using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_MissionInterface_Controller : UI_InterfaceController
{
    private Label missionTitle;
    private Label missionDescription;

    private VisualElement missionObjectives;
    protected override void OnEnable()
    {
        base.OnEnable();

        missionTitle = root.Q<Label>("MissionTitle");
        missionDescription = root.Q<Label>("MissionDescription");
        missionObjectives = root.Q<VisualElement>("MissionObjectives");

        UpdateCurrentMissionDetails();
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(InputManager.PrevQuest))
        {
            PlayerObjectiveTracker.instance.ActivatePreviousMission();
            UpdateCurrentMissionDetails();
        }
        else if (Input.GetKeyDown(InputManager.NextQuest))
        {
            PlayerObjectiveTracker.instance.ActivateNextMission();
            UpdateCurrentMissionDetails();
        }
    }

    private void UpdateCurrentMissionDetails()
    {
        if (!PlayerObjectiveTracker.instance)
            return;

        if (missionTitle != null)
        {
            missionTitle.text = PlayerObjectiveTracker.instance.currentMission != null ? PlayerObjectiveTracker.instance.currentMission.MissionName : "You currently do not have an active mission.";
        }

        if (missionDescription != null)
        {
            missionDescription.text = PlayerObjectiveTracker.instance.currentMission != null ? PlayerObjectiveTracker.instance.currentMission.MissionDescription : "";
        }

        if (missionObjectives != null)
        {
            missionObjectives.Clear();
            if (PlayerObjectiveTracker.instance.currentMission != null)
            {
                foreach (var objective in PlayerObjectiveTracker.instance.currentMission.objectives)
                {
                    // Create a label with classes "objective" and "objective-uncompleted" or "objective-completed" based on completion status
                    Label objectiveLabel = new Label(objective.ObjectiveName);
                    objectiveLabel.AddToClassList("objective");
                    if (objective.isCompleted)
                    {
                        objectiveLabel.AddToClassList("objective-completed");
                    }
                    else
                    {
                        objectiveLabel.AddToClassList("objective-uncompleted");
                    }
                    missionObjectives.Add(objectiveLabel);
                }
            }
        }
    }
}
