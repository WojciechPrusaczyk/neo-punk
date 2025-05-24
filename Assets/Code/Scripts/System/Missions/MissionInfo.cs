using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMissionInfo", menuName = "NeoPunk/MissionInfo")]
public class MissionInfo : ScriptableObject
{
    [Header("Must be unique")]
    public string MissionName;

    [Header("If it was finished")]
    public bool isFinished;

    [TextArea(3, 10)]
    public string MissionDescription;
}