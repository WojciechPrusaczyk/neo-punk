using System;
using System.Collections.Generic;
using UnityEngine;

// PROSZÊ NIE PYTAÆ JAK DZIA£A TA KLASA
// NIE WIEM TEGO JA, NIE WIE TEGO MÓJ BRAT, NAWET PEWNIE SAM BÓG INTERNETU NIE WIE

[Serializable]
public abstract class ObjectiveRequirementBase
{
    public abstract bool IsMet { get; }
    public abstract string Description { get; }
    public abstract string ProgressString { get; }

    public abstract void Initialize();
    public abstract void OnEvent(ObjectiveEventBase gameEvent, MissionInfo.MissionObjective parentObjective, PlayerObjectiveTracker tracker);

    public abstract ObjectiveRequirementSaveData GetSaveData();
    public abstract void LoadProgress(ObjectiveRequirementSaveData saveData);
}

[Serializable]
public class KillEnemyRequirement : ObjectiveRequirementBase
{
    public Enums.EnemyType TargetEnemyType;
    public int RequiredAmount;
    public int CurrentAmount;

    public override bool IsMet => CurrentAmount >= RequiredAmount;
    public override string Description => $"Defeat {RequiredAmount} {TargetEnemyType.ToString()}(s)";
    public override string ProgressString => $"{CurrentAmount} / {RequiredAmount}";

    public override void Initialize()
    {
        CurrentAmount = 0;
    }

    public override void OnEvent(ObjectiveEventBase gameEvent, MissionInfo.MissionObjective parentObjective, PlayerObjectiveTracker tracker)
    {
        if (parentObjective.isCompleted) return;

        if (gameEvent is EnemyKilledEvent enemyKilledEvent)
        {
            if (enemyKilledEvent.KilledEnemyType == TargetEnemyType)
            {
                CurrentAmount++;
                if (CurrentAmount > RequiredAmount) CurrentAmount = RequiredAmount;
                tracker.CheckObjectiveCompletion(parentObjective);
            }
        }
    }

    public override ObjectiveRequirementSaveData GetSaveData()
    {
        return new KillEnemyRequirementSaveData { currentAmount = this.CurrentAmount };
    }

    public override void LoadProgress(ObjectiveRequirementSaveData saveData)
    {
        if (saveData is KillEnemyRequirementSaveData killSaveData)
        {
            this.CurrentAmount = killSaveData.currentAmount;
        }
    }
}


public abstract class ObjectiveEventBase { }

public class EnemyKilledEvent : ObjectiveEventBase
{
    public Enums.EnemyType KilledEnemyType { get; }
    public EnemyKilledEvent(Enums.EnemyType killedEnemyType)
    {
        KilledEnemyType = killedEnemyType;
    }
}


[Serializable]
public abstract class ObjectiveRequirementSaveData { }

[Serializable]
public class KillEnemyRequirementSaveData : ObjectiveRequirementSaveData
{
    public int currentAmount;
}