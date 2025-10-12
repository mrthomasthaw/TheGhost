using MrThaw;
using System.Collections;
using UnityEngine;

public class EliminateThreatGoal : GGoal
{
    public override void SetUp(BlackBoardManager blackBoard)
    {
        base.SetUp(blackBoard);
        minPriority = 0;
        maxPriority = 10;


        EndGoal.Add(AIWorldStateKey.AssaultTarget.ToString(), true);
    }

    public override int CalculatePriority()
    {
        SelectedThreatInfoData primaryThreat = blackBoardManager.GetOneDataByKey<SelectedThreatInfoData>(BlackBoardKey.SelectedPrimaryThreat);
        if (primaryThreat != null && primaryThreat.IsStillValid)
        {
            Priority = 10;
        }
        else
        {
            Priority = 0;
        }

        Priority = Mathf.Clamp(Priority, minPriority, maxPriority);
        return Priority;
    }
}
