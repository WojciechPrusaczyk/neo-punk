using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_MissionCompletePopUpController : UI_InterfaceController
{
    private Label missionCompleteText;
    protected override void OnEnable()
    {
        base.OnEnable();

        rootVisualElement = root.Q<VisualElement>("Drones");
        missionCompleteText = root.Q<Label>("MissionName");

        if (PlayerObjectiveTracker.instance == null || PlayerObjectiveTracker.instance.currentMission == null)
            return;

        SetMissionCompletedName(PlayerObjectiveTracker.instance.currentMission.MissionName);
    }

    void SetMissionCompletedName(string missionName)
    {
        if (missionCompleteText != null)
        {
            missionCompleteText.text = missionName;
        }
    }
}
