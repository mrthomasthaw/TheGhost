using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleGoal : GGoal
{
    public override int CalculatePriority()
    {
        SelectedThreatInfoData primaryThreat = blackBoardManager.GetOneDataByKey<SelectedThreatInfoData>(BlackBoardKey.SelectedPrimaryThreat);
        if (primaryThreat != null && primaryThreat.IsStillValid) Priority = 0;
        else Priority = 1;
        
        return Priority;
    }
}
